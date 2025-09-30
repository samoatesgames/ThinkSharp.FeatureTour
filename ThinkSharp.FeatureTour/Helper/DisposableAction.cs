// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace ThinkSharp.FeatureTouring.Helper
{
    internal class DisposableAction : IDisposable
    {
        private readonly Action myAction;

        /// <summary>
        /// </summary>
        /// <param name="action">The action that is invoked when dispose is called.
        /// </param>
        public DisposableAction(Action action)
        {
            myAction = action;
        }

        public void Dispose()
        {
            if (myAction != null)
                myAction();
        }
    }
}
