// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ThinkSharp.FeatureTouring.Logging;
using ThinkSharp.FeatureTouring.Models;
using ThinkSharp.FeatureTouring.Navigation;
using ThinkSharp.FeatureTouring.ViewModels;

namespace ThinkSharp.FeatureTouring
{
    /// <summary>
    /// Interface for the TourViewModel to access the run.
    /// </summary>
    public interface ITourRun
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeUnloaded"></param>
        /// <returns></returns>
        bool NextStep(bool includeUnloaded);
        bool PreviousStep();
        void Close();
        void DoIt();

        bool CanNextStep();
        bool CanDoIt();

        Step CurrentStep { get; }
    }

    internal class TourRun : ITourRun
    {
        private readonly Tour m_myTour;
        private readonly IVisualElementManager m_myVisualElementManager;
        private readonly IWindowManager m_myWindowManager;
        private readonly IPopupNavigator m_myPopupNavigator;
        private TourViewModel m_myTourViewModel;
        private StepNode m_myCurrentStepNode;
        private Guid m_myCurrentWindowId;

        enum HandleWindowTransitionResult { ShowPopup, HidePopup, DoNothing }

        //  .ctor
        // ////////////////////////////////////////////////////////////////////

        internal TourRun(Tour tour, IVisualElementManager visualElementManager, IWindowManager windowManager, IPopupNavigator popupNavigator)
        {
            if (tour == null) throw new ArgumentNullException(nameof(tour));
            if (tour.Steps == null) throw new ArgumentNullException(nameof(tour.Steps));
            if (tour.Steps.Length == 0) throw new ArgumentException("Unable to start tour without steps");
            if (tour.Steps.Any(s => s == null)) throw new ArgumentException("Steps must not be null");
            if (tour.Steps.Any(s => s.ElementId == null)) throw new ArgumentException("Step.ElementID must not be null");

            m_myTour = tour;
            m_myVisualElementManager = visualElementManager ?? throw new ArgumentNullException(nameof(visualElementManager));
            m_myWindowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
            m_myPopupNavigator = popupNavigator ?? throw new ArgumentNullException(nameof(popupNavigator));

            windowManager.WindowActivated += WindowActivated;
            windowManager.WindowDeactivated += WindowDeactivated;
            m_myCurrentWindowId = windowManager.GetActiveWindowId();
            InitStepNodes();
        }
        
        // Methods
        // //////////////////////////////////////////////////////////////////////
        
        private void InitStepNodes()
        {
            StepNode prevStepNode = null;
            var counter = 1;
            foreach (var step in m_myTour.Steps)
            {
                var stepNode = new StepNode(step)
                {
                    Previous = prevStepNode,
                    StepNo = counter++
                };
                if (prevStepNode == null)
                    m_myCurrentStepNode = stepNode;
                else
                    prevStepNode.Next = stepNode;
                prevStepNode = stepNode;
            }
        }

        private void WindowDeactivated(object sender, WindowActivationChangedEventArgs e)
        {
        }

        private void WindowActivated(object sender, WindowActivationChangedEventArgs e)
        {
            // Window changed
            var previousWindowId = m_myCurrentWindowId;
            if (e.WindowId != m_myCurrentWindowId)
            {
                m_myCurrentWindowId = e.WindowId;
                var result = HandleWindowTransitionChange(previousWindowId);
                switch (result)
                {
                    case HandleWindowTransitionResult.HidePopup:
                        e.ShowPopup = false;
                        break;
                    case HandleWindowTransitionResult.ShowPopup:
                        e.ShowPopup = true;
                        break;
                }
            }
        }

        private HandleWindowTransitionResult HandleWindowTransitionChange(Guid previousWindowId)
        {
            // If current element is already on the new window, we don't need to go.
            // CASE: Window is just reactivated
            var currentElement = m_myVisualElementManager.GetVisualElement(m_myCurrentStepNode.Step.ElementId, false);
            if (currentElement == null)
            {
                Log.Warn($"Could not find visual element with ElementID '{m_myCurrentStepNode.Step.ElementId}'");
                return HandleWindowTransitionResult.DoNothing;
            }
            if (currentElement.WindowId == m_myCurrentWindowId)
                return HandleWindowTransitionResult.ShowPopup;

            var behavior = currentElement.WindowTransisionBehavior;
            if (behavior == WindowTransisionBehavior.None)
            {
                return HandleWindowTransitionResult.DoNothing;
            }
            if (behavior == WindowTransisionBehavior.Automatic)
            {
                var wasPreviousParent = m_myWindowManager.IsParentWindow(previousWindowId, m_myCurrentWindowId);
                behavior = wasPreviousParent ? WindowTransisionBehavior.NextHide : WindowTransisionBehavior.NextPreviousHide;
            }

            if (behavior == WindowTransisionBehavior.NextHide ||
                behavior == WindowTransisionBehavior.NextPreviousHide)
            {
                // otherwise, we will try to to the next element on the new window
                // CASE: Open modal dialog with elements
                var nextStep = m_myCurrentStepNode.NextStep;
                if (nextStep != null)
                {
                    var nextElement = m_myVisualElementManager.GetVisualElement(nextStep.ElementId, true);
                    // next element belongs to the new window
                    if (nextElement != null && nextElement.WindowId == m_myCurrentWindowId)
                    {
                        NextStep(true);
                        return HandleWindowTransitionResult.ShowPopup;
                    }
                }
            }

            if (behavior == WindowTransisionBehavior.PreviousHide ||
                behavior == WindowTransisionBehavior.NextPreviousHide)
            {
                // otherwise we will try to go to the nears previous step for the current window
                // CASE: Open modal dialog but do not pass all steps on that dialog and close the dialog
                StepNode prevStepNode = m_myCurrentStepNode;
                while ((prevStepNode = prevStepNode.Previous) != null)
                {
                    var prevElement = m_myVisualElementManager.GetVisualElement(prevStepNode.Step.ElementId, true);
                    if (prevElement == null || prevElement.WindowId != m_myCurrentWindowId)
                        continue;
                    SetStep(prevStepNode);
                    return HandleWindowTransitionResult.ShowPopup;
                }
            }

            // Otherwise do not show the popup because we have no meaningful content for the current window.
            return HandleWindowTransitionResult.HidePopup;
        }

        internal bool Start()
        {
            var factoryMethod = FeatureTour.ViewModelFactoryMethod;
            m_myTourViewModel = factoryMethod == null 
                ? new TourViewModel(this) 
                : factoryMethod(this);
            
            m_myPopupNavigator.StartTour(m_myTourViewModel);

            // Go to first step
            var success = SetStep(m_myCurrentStepNode);
            if (!success) Close();
            return success;
        }

        private string GetSteps()
        {
            return $"Step {m_myCurrentStepNode.StepNo}/{m_myTour.Steps.Length}";
        }

        private bool SetStep(StepNode nextStep, bool includeUnloaded = false)
        {
            if (nextStep == null)
            {
                Log.Debug("SetStep: nextStep is null");
                return false;
            }
            Log.Debug($"SetStep: '{nextStep.Step.Id}'");

            if (m_myCurrentStepNode != nextStep)
            {
                FeatureTour.OnStepLeaved(m_myCurrentStepNode.Step);
                m_myCurrentStepNode = nextStep;
            }
            var step = m_myCurrentStepNode.Step;
            FeatureTour.OnStepEntering(step);

            var app = Application.Current;

            VisualElement element;
            if (app == null)
                element = m_myVisualElementManager.GetVisualElement(step.ElementId, includeUnloaded);
            else
                element = app.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Func<object>(() => m_myVisualElementManager.GetVisualElement(step.ElementId, includeUnloaded))) as VisualElement;
            if (element == null)
            {
                LogWarningCouldNotFindElementFor(step);
                return false;
            }

            using (m_myPopupNavigator.MovePopupTo(element))
            {
                InitializeViewModel(step, element);

                // required to ensure that the view is updated before the popup is shown
                // otherwise the update is visible in the popup (which looks ugly)
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
                
                FeatureTour.OnStepEntered(step);
            }

            return true;
        }

        private static void LogWarningCouldNotFindElementFor(Step step)
        {
            var msg = $"Could not find visual element with ElementID '{step.ElementId}'. Popup may not occur.";
            msg += " Ensure that the visual element is available in the current view.";
            Log.Warn(msg);
        }

        private void InitializeViewModel(Step step, VisualElement element)
        {
            Debug.Assert(m_myTourViewModel != null);
            m_myTourViewModel.Header = step.Header;
            m_myTourViewModel.Content = step.Content;
            m_myTourViewModel.ContentTemplate = element.GetTemplate(step.ContentDataTemplateKey);
            m_myTourViewModel.HeaderTemplate = element.GetTemplate(step.HeaderDataTemplateKey);
            m_myTourViewModel.Steps = GetSteps();
            m_myTourViewModel.CurrentStepNo = m_myCurrentStepNode.StepNo;
            m_myTourViewModel.TotalStepsCount = m_myTour.Steps.Length;
            m_myTourViewModel.ShowDoIt = ShowDoIt();
            m_myTourViewModel.ShowNext = step.ShowNextButton ?? m_myTour.ShowNextButtonDefault;

            if (m_myCurrentStepNode.Next == null)
                m_myTourViewModel.SetCloseText();
            m_myTourViewModel.Placement = element.Placement;
        }

        // Properties (for testing)
        // ////////////////////////////////////////////////////////////////////

        public Step CurrentStep => m_myCurrentStepNode.Step;
        internal int CurrentStepNo => m_myCurrentStepNode.StepNo;

        // Interface implementations
        // ////////////////////////////////////////////////////////////////////

        #region ITutorialRun

        public bool NextStep(bool includeUnloaded = false)
        {
            return SetStep(m_myCurrentStepNode.Next, includeUnloaded);
        }

        public bool PreviousStep()
        {
            return SetStep(m_myCurrentStepNode.Previous);
        }

        public void Close()
        {
            m_myPopupNavigator.ExitTour();
            m_myWindowManager.WindowActivated -= WindowActivated;
            m_myWindowManager.WindowDeactivated -= WindowDeactivated;

            FeatureTour.OnStepLeaved(m_myCurrentStepNode.Step);
            FeatureTour.OnClosed(m_myCurrentStepNode.Step);
            FeatureTour.SetCurrentRunNull();
        }

        internal bool CanPreviousStep()
        {
            return CanGoToStep(m_myCurrentStepNode.PreviousStep);
        }

        public bool CanNextStep()
        {
            return CanGoToStep(m_myCurrentStepNode.NextStep);
        }

        private bool CanGoToStep(Step step)
        {
            if (step == null)
                return false;

            if (m_myTour.EnableNextButtonAlways)
                return true;

            // step entering is usually used to create a state where the visual element is available.
            if (FeatureTour.HasStepEnteringAttached(step))
                return true;

            var visualElement = m_myVisualElementManager.GetVisualElement(step.ElementId, true);
            if (visualElement == null || visualElement.WindowId != m_myCurrentWindowId)
                return false;

            return true;
        }

        public void DoIt()
        {
            FeatureTour.Do(m_myCurrentStepNode.Step);
        }

        public bool CanDoIt()
        {
            return FeatureTour.CanDo(m_myCurrentStepNode.Step);
        }

        public bool ShowDoIt()
        {
            return FeatureTour.HasDoableAttached(m_myCurrentStepNode.Step);
        }

        #endregion
    }
}
