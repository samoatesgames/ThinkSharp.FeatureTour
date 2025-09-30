// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace ThinkSharp.FeatureTouring.Navigation
{
    /// <summary>
    /// Interface for an object that allows to control the navigation of the tour.
    /// </summary>
    public interface ITourNavigator
    {
        /// <summary>
        /// Navigates to the next step.
        /// </summary>
        /// <returns>
        /// true if navigation succeeded; otherwise false.
        /// </returns>
        bool GoNext();

        /// <summary>
        /// Navigates to the previous step.
        /// </summary>
        /// <returns>
        /// true if navigation succeeded; otherwise false.
        /// </returns>
        bool GoPrevious();

        /// <summary>
        /// Closes the active tour.
        /// </summary>
        void Close();
    }
    internal class NullTourNavigator : ITourNavigator
    {
        public bool GoNext() { return false; }
        public bool GoPrevious() { return false; }
        public void Close() { }
    }
    internal class TourNavigator : ITourNavigator
    {
        private readonly ITourRun m_myRun;

        public TourNavigator(ITourRun run)
        {
            m_myRun = run;
        }

        public bool GoNext()
        {
            return m_myRun.NextStep(false);
        }

        public bool GoPrevious()
        {
            return m_myRun.PreviousStep();
        }

        public void Close()
        {
            m_myRun.Close();
        }
    }
}
