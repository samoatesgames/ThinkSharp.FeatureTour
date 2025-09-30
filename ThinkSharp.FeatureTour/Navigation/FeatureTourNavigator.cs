// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using ThinkSharp.FeatureTouring.Logging;
using ThinkSharp.FeatureTouring.Models;
using ThinkSharp.FeatureTouring.ViewModels;

namespace ThinkSharp.FeatureTouring.Navigation
{
    /// <summary>
    /// Abstracts the navigation object for controlling the current tour.
    /// </summary>
    public interface IFeatureTourNavigator
    {
        /// <summary>
        /// Returns an object providing navigation (next step, previous step, close tour) if the current step has the specified step ID.
        /// </summary>
        /// <param name="stepId">
        /// The step ID</param>
        /// <returns>
        /// <see cref="ITourNavigator"/> object that provides navigation methods.
        /// </returns>
        ITourNavigator IfCurrentStepEquals(string stepId);

        /// <summary>
        /// Returns an object for attaching doable actions for the specified step ID.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourDoable"/> object for attaching doable action.
        /// </returns>
        ITourDoable ForStep(string stepId);

        /// <summary>
        /// Returns an object for executing custom logic before the step with the specified step ID was entered.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        ITourExecution OnStepEntering(string stepId);

        /// <summary>
        /// Returns an object for executing custom logic after the step with the specified step ID was entered.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        ITourExecution OnStepEntered(string stepId);

        /// <summary>
        /// Returns an object for executing custom logic after the step with the specified step ID was left.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        ITourExecution OnStepLeft(string stepId);

        /// <summary>
        /// Returns an object for executing custom logic if the tour was closed.
        /// </summary>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        ITourExecution OnClosed();

        /// <summary>
        /// Closes the current tour.
        /// </summary>
        /// <returns>
        /// true if the current run was active and has been closed; otherwise false.
        /// </returns>
        bool Close();
    }

    /// <summary>
    /// Entry point for controlling the tour programmatically.
    /// Use <see cref="GetNavigator"/> to get the <see cref="IFeatureTourNavigator"/> object.
    /// </summary>
    public class FeatureTour : IFeatureTourNavigator
    {
        private static ITourRun _theCurrentTourRun;

        private static readonly ITourNavigator theNullNavigator = new NullTourNavigator();
        private static readonly ITourExecution theNullExecution = new NullTourExecution();
        private static readonly ITourDoable theNullDoable = new NullTourDoable();

        private static readonly ActionRepository theExecutionRepository = new ActionRepository();
        private static readonly ActionRepository theDoableRepository = new ActionRepository();

        private const string c_stepEntered = "OnStepEnterd";
        private const string c_stepEntering = "OnStepEntering";
        private const string c_stepLeaved = "OnStepLeaved";
        private const string c_doable = "Doable";
        private const string c_closed = "OnClosed";

        internal static Func<ITourRun, TourViewModel> ViewModelFactoryMethod { get; private set; }


        // public interface
        // //////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the <see cref="IFeatureTourNavigator"/> object that can be used to control navigation.
        /// </summary>
        /// <returns></returns>
        public static IFeatureTourNavigator GetNavigator()
        {
            return new FeatureTour();
        }

        /// <summary>
        /// Sets a factory method to use sub-classed <see cref="TourViewModel"/>s with additional
        /// properties / behaviors. May be meaningful in combination with custom templates.
        /// </summary>
        /// <param name="factoryMethod">
        /// The factory method to create view model or null to use the default one.
        /// </param>
        public static void SetViewModelFactoryMethod(Func<ITourRun, TourViewModel> factoryMethod)
        {
            ViewModelFactoryMethod = factoryMethod;
        }

        /// <summary>
        /// Returns an object providing navigation (next step, previous step, close tour) if the current step has the specified ste ID.
        /// </summary>
        /// <param name="stepId">
        /// The step ID</param>
        /// <returns>
        /// <see cref="ITourNavigator"/> object that provides navigation methods.
        /// </returns>
        public ITourNavigator IfCurrentStepEquals(string stepId)
        {
            var currentRun = _theCurrentTourRun;
            if (currentRun == null || currentRun.CurrentStep == null || currentRun.CurrentStep.Id != stepId) 
                return theNullNavigator;
            return new TourNavigator(currentRun);
        }

        /// <summary>
        /// Returns an object for attaching doable actions for the specified step ID.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourDoable"/> object for attaching doable action.
        /// </returns>
        public ITourDoable ForStep(string stepId)
        {
            if (string.IsNullOrEmpty(stepId))
                return theNullDoable;
            return new TourDoable(theDoableRepository, GetName(stepId, c_doable));
        }

        /// <summary>
        /// Returns an object for executing custom logic before the step with the specified step ID was entered.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        public ITourExecution OnStepEntering(string stepId)
        {
            if (string.IsNullOrEmpty(stepId))
                return theNullExecution;
            return new TourExecution(theExecutionRepository, GetName(stepId, c_stepEntering));
        }

        /// <summary>
        /// Returns an object for executing custom logic after the step with the specified step ID was entered.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        public ITourExecution OnStepEntered(string stepId)
        {
            if (string.IsNullOrEmpty(stepId))
                return theNullExecution;
            return new TourExecution(theExecutionRepository, GetName(stepId, c_stepEntered));
        }

        /// <summary>
        /// Returns an object for executing custom logic after the step with the specified step ID was left.
        /// </summary>
        /// <param name="stepId">
        /// The step ID.
        /// </param>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        public ITourExecution OnStepLeft(string stepId)
        {
            if (string.IsNullOrEmpty(stepId))
                return theNullExecution;
            return new TourExecution(theExecutionRepository, GetName(stepId, c_stepLeaved));
        }

        /// <summary>
        /// Returns an object for executing custom logic if the tour was closed.
        /// </summary>
        /// <returns>
        /// <see cref="ITourExecution"/> object for executing custom logic.
        /// </returns>
        public ITourExecution OnClosed()
        {
            return new TourExecution(theExecutionRepository, c_closed);
        }
        
        /// <summary>
        /// Closes the current tour.
        /// </summary>
        /// <returns>
        /// true if the current run was active and has been closed; otherwise false.
        /// </returns>
        public bool Close()
        {
            var currentRun = _theCurrentTourRun;
            if (currentRun == null) return false;
            currentRun.Close();
            return true;
        }


        // internal interface
        // //////////////////////////////////////////////////////////////////////

        internal static bool HasStepEnteringAttached(Step step)
        {
            return theExecutionRepository.Contains(GetName(step.Id, c_stepEntering));
        }

        internal static void SetTourRun(ITourRun run)
        {
            var currentRun = _theCurrentTourRun;
            currentRun?.Close();
            _theCurrentTourRun = run;
        }

        internal static Step CurrentStep
        {
            get
            {
                var currentRun = _theCurrentTourRun;
                if (currentRun == null) return null;
                return currentRun.CurrentStep;
            }
        }

        internal static void OnStepEntering(Step step)
        {
            if (step == null) return;
            var name = GetName(step.Id, c_stepEntering);
            Log.Debug($"OnStepEntering: '{name}'");
            theExecutionRepository.Execute(name, step);
        }

        internal static void OnStepEntered(Step step)
        {
            if (step == null) return;
            var name = GetName(step.Id, c_stepEntered);
            Log.Debug($"OnStepEntered: '{name}'");
            theExecutionRepository.Execute(name, step);
        }

        internal static void OnStepLeaved(Step step)
        {
            if (step == null) return;
            var name = GetName(step.Id, c_stepLeaved);
            Log.Debug($"OnStepLeaved: '{name}'");
            theExecutionRepository.Execute(name, step);
        }

        internal static void OnClosed(Step step)
        {
            Log.Debug("OnClosed");
            theExecutionRepository.Execute(c_closed, step);
        }

        internal static void SetCurrentRunNull()
        {
            _theCurrentTourRun = null;
        }

        private static string GetName(string stepId, string postfix)
        {
            return string.Format("stepChange_{0}_{1}", stepId, postfix);
        }

        internal static void Do(Step step)
        {
            var name = GetName(step.Id, c_doable);
            Log.Debug($"Do: '{name}'");
            theDoableRepository.Execute(name, step);
        }

        internal static bool CanDo(Step step)
        {
            return theDoableRepository.CanExecute(GetName(step.Id, c_doable), step);
        }

        internal static bool HasDoableAttached(Step step)
        {
            return theDoableRepository.Contains(GetName(step.Id, c_doable));
        }
    }
}
