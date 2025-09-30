// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using ThinkSharp.FeatureTouring.Controls;
using ThinkSharp.FeatureTouring.Helper;
using ThinkSharp.FeatureTouring.Logging;

namespace ThinkSharp.FeatureTouring
{
    internal class WindowActivationChangedEventArgs : EventArgs
    {
        public WindowActivationChangedEventArgs(Guid windowId, bool showPoup)
        {
            ShowPopup = showPoup;
            WindowId = windowId;
        }
        public bool ShowPopup { get; set; }
        public Guid WindowId { get; private set; }
    }
    internal interface IPlacementAware
    {
        /// <summary>
        /// Gets or sets the desired <see cref="Placement"/> of the pop-up.
        /// </summary>
        Placement Placement { get; set; }

        /// <summary>
        /// Gets or sets the actual <see cref="Placement"/>. The framework may change the desired placement (e.g. if the pop-up can not be placed on left-side).
        /// </summary>
        Placement ActualPlacement { get; set; }
    }

    internal interface IVisualElementManager
    {
        /// <summary>
        /// Gets all <see cref="VisualElement"/> objects managed by the <see cref="VisualElementManager"/>.
        /// </summary>
        /// <param name="includeUnloaded">
        /// true to return also unloaded (currently not visible) element; false to return only loaded (current visible) elements</param>
        /// <returns></returns>
        IEnumerable<VisualElement> GetVisualElements(bool includeUnloaded);

        /// <summary>
        /// Gets the <see cref="VisualElement"/> with the specified elementID or null if the element is nor available.
        /// </summary>
        /// <param name="elementId">
        /// The element ID to get the <see cref="VisualElement"/> for.
        /// </param>
        /// <param name="includeUnloaded">
        /// true if also unloaded (not yet visible) elements should be returned; otherwise false.
        /// </param>
        /// <returns></returns>
        VisualElement GetVisualElement(string elementId, bool includeUnloaded);
    }

    internal class VisualElementManager : IVisualElementManager
    {
        private readonly IWindowManager m_myWindowManager;
        private readonly List<VisualElement> m_myVisualElements = new List<VisualElement>();
        
        public VisualElementManager(IWindowManager windowManager)
        {
            m_myWindowManager = windowManager;
            m_myWindowManager.WindowRemoved += (_, eargs) => m_myVisualElements.RemoveAll(ve => ve.WindowId == eargs.WindowId);
        }

        IEnumerable<VisualElement> IVisualElementManager.GetVisualElements(bool includeUnloaded)
        {
            foreach (var visualElement in m_myVisualElements.ToArray())
            {
                if (visualElement.TryGetElement(out var element))
                {
                    // Do only return loaded elements but do not remove unloaded elements
                    // because it is possible that elements get temporary unloaded (e.g. on
                    // a tab of a TabControl.
                    if (element.IsLoaded || includeUnloaded)
                        yield return visualElement;
                }
            }
        }

        VisualElement IVisualElementManager.GetVisualElement(string elementId, bool includeUnloaded)
        {
            return (this as IVisualElementManager).GetVisualElements(includeUnloaded).FirstOrDefault(e => e.ElementId == elementId);
        }

        
        internal void ElementAdded(FrameworkElement element)
        {
            var visualElement = GetVisualElement(element);
            m_myVisualElements.Add(visualElement);
        }

        internal void ElementPropertyChanged(UIElement element, Action<UIElement, VisualElement> propertySetter)
        {
            var elementId = TourHelper.GetElementId(element);
            if (string.IsNullOrEmpty(elementId))
                return;

            var visualElement = m_myVisualElements.FirstOrDefault(e => e.ElementId == elementId);
            if (visualElement != null)
                propertySetter(element, visualElement);
        }

        private VisualElement GetVisualElement(FrameworkElement element)
        {
            var elementId = TourHelper.GetElementId(element);
            var placement = TourHelper.GetPlacement(element);
            var transtionBehavior = TourHelper.GetWindowTransisionBehavior(element);

            RemoveElement(elementId);

            return new VisualElement(element)
            {
                Placement = placement,
                ElementId = elementId,
                WindowTransisionBehavior = transtionBehavior,
                WindowId = m_myWindowManager.GetWindowId(element, elementId)
            };
        }

        private void RemoveElement(string elementId)
        {
            foreach (var visualElement in m_myVisualElements.ToArray())
            {
                if (!visualElement.TryGetElement(out _) || visualElement.ElementId == elementId)
                    m_myVisualElements.Remove(visualElement);
            }
        }
    }

    internal interface IPopupNavigator
    {
        IDisposable MovePopupTo(VisualElement element);
        void StartTour(IPlacementAware viewModel);
        void ShowPopup();
        void HidePopup();
        void ExitTour();
        void UpdatePopupPosition();
    }

    internal class PupupNavigator : IPopupNavigator
    {
        private FeatureTourPopup m_myPopup;

        private class FeatureTourPopup
        {
            private readonly IPlacementAware m_myViewModel;
            private readonly Popup m_myPopup;
            
            private FrameworkElement m_myElement;

            private CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
            => PlacementHelper.CustomPopupPlacementCallback(popupSize, targetSize, m_myViewModel?.Placement ?? Placement.TopLeft);

            public FeatureTourPopup(IPlacementAware viewModel)
            {
                m_myViewModel = viewModel;

                var tourControl = new TourControl();
                m_myPopup = new Popup
                {
                    AllowsTransparency = true,
                    Child = tourControl,
                    Placement = PlacementMode.Custom
                };

                tourControl.DataContext = m_myViewModel;

                m_myPopup.CustomPopupPlacementCallback += CustomPopupPlacementCallback;
                m_myPopup.Opened += (_, _) => UpdatePopupPosition();
            }

            public void Open()
            {
                UpdatePopupPosition();
                m_myPopup.IsOpen = true;   
            }
            public void Close() => m_myPopup.IsOpen = false;

            public void UpdatePopupPosition()
            {
                // Workaround to move the popup if the window is moved.
                var offset = m_myPopup.HorizontalOffset;
                m_myPopup.HorizontalOffset = offset + 1;
                m_myPopup.HorizontalOffset = offset;
                var vm = m_myViewModel;
                if (vm != null)
                {
                    if (m_myPopup.PlacementTarget is FrameworkElement { IsLoaded: true })
                    {
                        vm.ActualPlacement = m_myPopup.GetActualPlacement(vm.Placement);
                    }
                    else
                    {
                        m_myPopup.IsOpen = false;
                    }
                }
            }

            internal void UpdatePlacementTarget(FrameworkElement element)
            {
                DetachEventHandlers();

                m_myElement = element;

                AttachEventHandlers();

                if (m_myElement != null)
                    m_myPopup.PlacementTarget = m_myElement;
                else
                    Close();
            }

            private void DetachEventHandlers()
            {
                var element = m_myElement;
                if (element == null) return;

                element.Loaded -= ElementOnLoaded;
                element.Unloaded -= ElementOnUnloaded;

                m_myElement = null;
            }

            private void AttachEventHandlers()
            {
                var element = m_myElement;
                if (element == null) return;

                element.Loaded += ElementOnLoaded;
                element.Unloaded += ElementOnUnloaded;
            }

            private void ElementOnUnloaded(object sender, RoutedEventArgs routedEventArgs) => Close();
            private void ElementOnLoaded(object sender, RoutedEventArgs routedEventArgs) => Open();

            internal void Release()
            {
                Close();
                DetachEventHandlers();
            }
        }

        public IDisposable MovePopupTo(VisualElement visualElement)
        {
            HidePopup();
            Log.Debug($"MovePopupTo: {visualElement.ElementId}");
            if (visualElement.TryGetElement(out var element))
            {
                m_myPopup?.UpdatePlacementTarget(element);
            }
            else
            {
                Log.Warn($"MovePopupTo: Could not find placement target with element ID: {visualElement.ElementId}");
            }
            return new DisposableAction(ShowPopup);
        }

        public void StartTour(IPlacementAware viewModel)
        {
            m_myPopup?.Release();
            m_myPopup = new FeatureTourPopup(viewModel);
        }

        public void ExitTour()
        {
            if (m_myPopup == null)
            {
                Log.Warn($"{nameof(ExitTour)}: Unable to exit tour - tour has not been started.");
            }
            else
            {
                m_myPopup?.Release();
                m_myPopup = null;
            }
        }

        public void ShowPopup()
        {
            m_myPopup?.Open();
        }

        public void HidePopup()
        {
            m_myPopup?.Close();
        }

        public void UpdatePopupPosition()
        {
            m_myPopup?.UpdatePopupPosition();
        }
    }

    internal interface IWindowManager
    {
        Guid GetActiveWindowId();
        bool IsParentWindow(Guid parentId, Guid childId);
        Guid GetWindowId(UIElement element, string elementId);

        event EventHandler<WindowActivationChangedEventArgs> WindowActivated;
        event EventHandler<WindowActivationChangedEventArgs> WindowDeactivated;
        event EventHandler<WindowActivationChangedEventArgs> WindowRemoved;
    }

    internal class WindowManager : IWindowManager
    {
        private readonly IPopupNavigator m_myPopupNavigator;
        private readonly Dictionary<Window, Guid> m_myReferencedWindows = new Dictionary<Window, Guid>();

        public event EventHandler<WindowActivationChangedEventArgs> WindowActivated;
        public event EventHandler<WindowActivationChangedEventArgs> WindowDeactivated;
        public event EventHandler<WindowActivationChangedEventArgs> WindowRemoved;

        public WindowManager(IPopupNavigator popupNavigator)
        {
            m_myPopupNavigator = popupNavigator;
        }

        public Guid GetActiveWindowId()
        {
            var current = Application.Current;

            var activeWindow = current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (activeWindow == null)
                return Guid.Empty;

            if (!m_myReferencedWindows.TryGetValue(activeWindow, out var windowId))
                return Guid.Empty;

            return windowId;
        }

        public bool IsParentWindow(Guid parentId, Guid childId)
        {
            var idxParent = m_myReferencedWindows.Values.ToList().IndexOf(parentId);
            var idxChild = m_myReferencedWindows.Values.ToList().IndexOf(childId);

            if (idxParent < 0 || idxChild < 0)
                return false;

            // parent window must be added first and therefore has a lower index.
            return idxParent < idxChild;
        }

        public Guid GetWindowId(UIElement element, string elementId)
        {
            var window = Window.GetWindow(element);
            if (window == null)
            {
                var lastWindow = m_myReferencedWindows.LastOrDefault();
                if (lastWindow.Key == null)
                    return Guid.Empty;
                return lastWindow.Value;
            }

            if (!m_myReferencedWindows.TryGetValue(window, out var guid))
            {
                guid = IsMainWindow(window) ? Guid.Empty : Guid.NewGuid();
                m_myReferencedWindows.Add(window, guid);
                window.SizeChanged += WindowSizeChanged;
                window.LocationChanged += WindowLocationChanged;
                window.Deactivated += WinDeactivated;
                window.Activated += WinActivated;
                window.Closed += WindowClosed;
            }
            return guid;
        }

        private static bool IsMainWindow(Window window)
        {
            var app = Application.Current;
            return app != null && app.MainWindow == window;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            RemoveReferencedWindow(sender as Window);
        }

        private void RemoveReferencedWindow(Window window)
        {
            if (window != null)
            {
                // remove event handler
                window.SizeChanged -= WindowSizeChanged;
                window.LocationChanged -= WindowLocationChanged;
                window.Deactivated -= WinDeactivated;
                window.Activated -= WinActivated;
                window.Closed -= WindowClosed;

                // remove references (element and window)
                if (m_myReferencedWindows.Remove(window, out var guid))
                {
                    WindowRemoved?.Invoke(this, new WindowActivationChangedEventArgs(guid, false));
                }
            }
        }

        private void WindowLocationChanged(object sender, EventArgs e)
        {
            m_myPopupNavigator.UpdatePopupPosition();
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_myPopupNavigator.UpdatePopupPosition();
        }

        private void WinDeactivated(object sender, EventArgs e)
        {
            // close popup because it is on top (even of windows of other applications)
            var open = OnWindowActivationChanged(sender, WindowDeactivated, false);
            if (open)
                m_myPopupNavigator.ShowPopup();
            else
                m_myPopupNavigator.HidePopup();
        }

        private void WinActivated(object sender, EventArgs e)
        {
            var open = OnWindowActivationChanged(sender, WindowActivated, true);
            if (open)
                m_myPopupNavigator.ShowPopup();
            else
                m_myPopupNavigator.HidePopup();
        }

        private bool OnWindowActivationChanged(object sender, EventHandler<WindowActivationChangedEventArgs> handler, bool showPopup)
        {
            var h = handler;
            if (h == null)
                return false;

            if (sender is not Window window)
                return false;

            if (m_myReferencedWindows.TryGetValue(window, out var guid))
            {
                var args = new WindowActivationChangedEventArgs(guid, showPopup);
                h(this, args);
                return args.ShowPopup;
            }
            return showPopup;
        }
    }
}
