// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using ThinkSharp.FeatureTouring.Models;

namespace ThinkSharp.FeatureTouring.Navigation
{
    /// <summary>
    /// Interface for an object that allows to execute actions.
    /// </summary>
    public interface ITourExecution
    {
        /// <summary>
        /// Executes the specified action.
        /// </summary>
        /// <param name="action">
        /// The action to execute.
        /// </param>
        void Execute(Action<Step> action);
    }
    internal class NullTourExecution : ITourExecution
    {
        public void Execute(Action<Step> action)
        {
        }
    }
    internal class TourExecution : ITourExecution
    {
        private readonly ActionRepository m_myActionRepository;
        private readonly string m_myName;

        public TourExecution(ActionRepository actionRepository, string name)
        {
            m_myActionRepository = actionRepository;
            m_myName = name;
        }

        public void Execute(Action<Step> action)
        {
            m_myActionRepository.AddAction(m_myName, action);
        }
    }
}
