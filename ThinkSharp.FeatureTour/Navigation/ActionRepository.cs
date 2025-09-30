// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using ThinkSharp.FeatureTouring.Helper;
using ThinkSharp.FeatureTouring.Logging;
using ThinkSharp.FeatureTouring.Models;

namespace ThinkSharp.FeatureTouring.Navigation
{
    using ExecuteAction = Action<Step>;
    using CanExecuteAction = Func<Step, bool>;

    internal class ActionRepository
    {
        private readonly Dictionary<string, ExecuteAction> m_myExecuteActions = new Dictionary<string, ExecuteAction>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, CanExecuteAction> m_myCanExecuteActions = new Dictionary<string, CanExecuteAction>(StringComparer.InvariantCultureIgnoreCase);

        public IReleasable AddAction(string name, Action<Step> executeAction)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));

            var execute = new ExecuteAction(executeAction);

            if (m_myExecuteActions.ContainsKey(name))
                Log.Warn($"Action with name '{name}' already exists. Will be overwritten!");

            m_myExecuteActions[name] = execute;

            Log.Debug($"ActionRepository: Action '{name}' added.");

            return new ReleasableAction(() =>
            {
                if (m_myExecuteActions.TryGetValue(name, out var executeStored) && executeStored == execute)
                    m_myExecuteActions.Remove(name);
            });
        }

        public IReleasable AddAction(string name, Action<Step> executeAction, Func<Step, bool> canExecuteAction)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));
            if (canExecuteAction == null)
                throw new ArgumentNullException(nameof(canExecuteAction));

            var execute = new ExecuteAction(executeAction);
            m_myExecuteActions[name] = execute;
            var canExecute = new CanExecuteAction(canExecuteAction);
            m_myCanExecuteActions[name] = canExecute;

            Log.Debug($"ActionRepository: Action '{name}' added (with CanExecute).");

            return new ReleasableAction(() =>
            {
                if (m_myExecuteActions.TryGetValue(name, out var executeStored) && executeStored == execute)
                    m_myExecuteActions.Remove(name);

                if (m_myCanExecuteActions.TryGetValue(name, out var canExecuteStored) && canExecuteStored == canExecute)
                    m_myCanExecuteActions.Remove(name);
            });
        }

        public bool Contains(string name)
        {
            return m_myExecuteActions.ContainsKey(name);
        }

        internal void Clear()
        {
            m_myExecuteActions.Clear();
            m_myCanExecuteActions.Clear();
        }

        public void Execute(string name, Step step)
        {
            if (!m_myExecuteActions.TryGetValue(name, out var action))
            {
                Log.Debug($"ActionRepository: Action '{name}' not available.");
                return;
            }

            action.Invoke(step);
        }

        public bool CanExecute(string name, Step step)
        {
            if (!m_myCanExecuteActions.TryGetValue(name, out var canAction))
                return false;

            return canAction.Invoke(step);
        }

        // for unit testing
        internal int ExecuteCount => m_myExecuteActions.Count;
        internal int CanExecuteCount => m_myExecuteActions.Count;
    }
}
