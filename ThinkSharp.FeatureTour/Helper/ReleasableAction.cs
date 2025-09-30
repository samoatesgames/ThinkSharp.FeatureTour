// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace ThinkSharp.FeatureTouring.Helper
{
    internal class ReleasableAction : IReleasable
    {
        private static readonly IReleasable theEmpty = new ReleasableAction(null);
        private readonly Action m_myAction;

        /// <summary>
        /// </summary>
        /// <param name="action">The action that is invoked when dispose is called.
        /// </param>
        public ReleasableAction(Action action)
        {
            m_myAction = action;
        }

        public void Release()
        {
            m_myAction?.Invoke();
        }

        public static IReleasable Empty => theEmpty;
    }
}
