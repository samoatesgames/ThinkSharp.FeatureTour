// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace ThinkSharp.FeatureTouring.Helper
{
    /// <summary>
    /// Interface or releasable objects.
    /// </summary>
    public interface IReleasable
    {
        /// <summary>
        /// Releases the object.
        /// </summary>
        void Release();
    }
}
