// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ThinkSharp.FeatureTouring.Helper;
using ThinkSharp.FeatureTouring.Models;
using ThinkSharp.FeatureTouring.ViewModels;

namespace ThinkSharp.FeatureTouring.Test
{
    public class VisualElementManagerMock : IVisualElementManager
    {
        public VisualElementManagerMock()
        {
            VisualElements = new List<VisualElement>();
        }

        public VisualElementManagerMock(IEnumerable<Step> stepsToUseForInitialize)
            : this()
        {
            foreach (var step in stepsToUseForInitialize)
                VisualElements.Add(new VisualElement(null)
                {
                    ElementId = step.ElementId,
                    WindowId = Guid.Parse("df575070-f243-4e6d-bc14-a5196294cedf"),
                });
        }

        #region IVisualElementManager

        IEnumerable<VisualElement> IVisualElementManager.GetVisualElements(bool includeUnloaded)
        {
            return VisualElements;
        }

        VisualElement IVisualElementManager.GetVisualElement(string elementId, bool includeUnloaded)
        {
            return VisualElements.FirstOrDefault(e => e.ElementId == elementId);
        }
        

        #endregion

        // API for unit tests
        internal List<VisualElement> VisualElements { get; private set; }
    }

    internal class WindowManagerMock : IWindowManager
    {
#pragma warning disable 0067
        public event EventHandler<WindowActivationChangedEventArgs> WindowRemoved;
#pragma warning restore 0067
        public Guid GetWindowId(UIElement element, string elementId)
        {
            return Guid.Parse("df575070-f243-4e6d-bc14-a5196294cedf");
        }

        internal int WindowActivatedHandlersCount { get; set; }
        internal int WindowDeactivatedHandlersCount { get; set; }
        internal void ActivateWindow(Guid windowId, bool showPopup)
        {
            MyWindowActivatedEvent?.Invoke(this, new WindowActivationChangedEventArgs(windowId, showPopup));
        }

        internal void DeactivateWindow(Guid windowId, bool showPopup)
        {
            MyWindowDeactivated?.Invoke(this, new WindowActivationChangedEventArgs(windowId, showPopup));
        }

        public Guid GetActiveWindowId()
        {
            return Guid.Parse("df575070-f243-4e6d-bc14-a5196294cedf");
        }

        public bool IsParentWindow(Guid parentId, Guid childId)
        {
            return true;
        }

        private event EventHandler<WindowActivationChangedEventArgs> MyWindowActivatedEvent;
        public event EventHandler<WindowActivationChangedEventArgs> WindowActivated
        {
            add
            {
                WindowActivatedHandlersCount += 1;
                MyWindowActivatedEvent += value;
            }
            remove
            {
                WindowActivatedHandlersCount -= 1;
                MyWindowActivatedEvent -= value;
            }
        }

        private event EventHandler<WindowActivationChangedEventArgs> MyWindowDeactivated;
        public event EventHandler<WindowActivationChangedEventArgs> WindowDeactivated
        {
            add
            {
                WindowDeactivatedHandlersCount += 1;
                MyWindowDeactivated += value;
            }
            remove
            {
                WindowDeactivatedHandlersCount -= 1;
                MyWindowDeactivated -= value;
            }
        }
    }

    internal class PopupNavigatorMock : IPopupNavigator
    {
        internal TourViewModel ViewModel { get; private set; }
        internal bool Reseted { get; set; }

        public IDisposable MovePopupTo(VisualElement element)
        {
            return new DisposableAction(() => { });
        }

        public void StartTour(IPlacementAware viewModel)
        {
            ViewModel = viewModel as TourViewModel;
        }

        public void ShowPopup()
        {
        }

        public void HidePopup()
        {
        }

        public void ExitTour()
        {
            Reseted = true;
        }

        public void UpdatePopupPosition()
        {
        }
    }
}
