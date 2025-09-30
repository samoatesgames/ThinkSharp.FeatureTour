// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace ThinkSharp.FeatureTouring.Helper
{
    internal class DisposableAction : IDisposable
    {
        private readonly Action m_myAction;

        /// <summary>
        /// </summary>
        /// <param name="action">The action that is invoked when dispose is called.
        /// </param>
        public DisposableAction(Action action)
        {
            m_myAction = action;
        }

        public void Dispose()
        {
            m_myAction?.Invoke();
        }
    }
}
