// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using ThinkSharp.FeatureTouring.Logging;

namespace ThinkSharp.FeatureTouring
{
    /// <summary>
    /// A <see cref="VisualElement"/> is a wrapper around a <see cref="FrameworkElement"/>, a feature tour step is attached to.
    /// </summary>
    internal class VisualElement
    {
        private readonly WeakReference myElement;
        public VisualElement(FrameworkElement element)
        {
            myElement = new WeakReference(element);
        }

        public string ElementID { get; set; }
        public Placement Placement { get; set; }
        public WindowTransisionBehavior WindowTransisionBehavior { get; set; }
        public Guid WindowID { get; set; }

        public DataTemplate GetTemplate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (TryGetElement(out var element))
            {
                var template = element.TryFindResource(name) as DataTemplate;
                if (template == null)
                    Log.Warn($"Could not find data template '{name}' relative to element with ID '{ElementID}'.");
                return template;
            }
            Log.Warn($"Unable to find element with ID '{ElementID}' (Maybe not visible anymore?). Data template '{name}' can not be applied.");
            return null;
        }

        public bool TryGetElement(out FrameworkElement element)
        {
            element = myElement.Target as FrameworkElement;
            return myElement.IsAlive;
        }
    }
}
