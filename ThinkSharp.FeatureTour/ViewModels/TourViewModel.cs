// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Input;

namespace ThinkSharp.FeatureTouring.ViewModels
{
    /// <summary>
    /// ViewModel behind the Popup.
    /// </summary>
    public class TourViewModel : ViewModelBase, IPlacementAware
    {
        private readonly string m_myClose = TextLocalization.Close;
        private readonly string m_myNext = TextLocalization.Next;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="tourRun">
        /// The <see langword="sealed"/>
        /// ITourRun object to be used by the view model to interact with the tour.
        /// May not be null.
        /// </param>
        protected internal TourViewModel(ITourRun tourRun)
        {
            var myTourRun = tourRun ?? throw new ArgumentNullException(nameof(tourRun));
            CmdDoIt = new RelayCommand(_ => myTourRun.DoIt(), _ => myTourRun.CanDoIt());
            CmdClose = new RelayCommand(_ => myTourRun.Close());
            CmdNext = new RelayCommand(_ =>
            {
                if (ButtonText == m_myClose)
                    myTourRun.Close();
                else
                    myTourRun.NextStep(false);
            }, _ => ButtonText == m_myClose || myTourRun.CanNextStep());
            ButtonText = m_myNext;
        }

        #region Content

        private object m_myContent;
        public object Content
        {
            get => m_myContent;
            set => SetValue("Content", ref m_myContent, value);
        }

        #endregion
        
        #region Header

        private object m_myHeader;
        public object Header
        {
            get => m_myHeader;
            set => SetValue("Header", ref m_myHeader, value);
        }

        #endregion

        #region Steps

        private string m_mySteps;
        public string Steps
        {
            get => m_mySteps;
            set => SetValue("Steps", ref m_mySteps, value);
        }

        #endregion

        #region Placement

        private Placement m_myPlacement;
        public Placement Placement
        {
            get => m_myPlacement;
            set
            {
                if (SetValue("Placement", ref m_myPlacement, value))
                    ActualPlacement = value;
            }
        }

        #endregion

        #region ActualPlacement

        private Placement m_myActualPlacement;
        public Placement ActualPlacement
        {
            get => m_myActualPlacement;
            set => SetValue("ActualPlacement", ref m_myActualPlacement, value);
        }

        #endregion

        #region ContentTemplate

        private DataTemplate m_myContentTemplate;
        public DataTemplate ContentTemplate
        {
            get => m_myContentTemplate;
            set => SetValue("ContentTemplate", ref m_myContentTemplate, value);
        }

        #endregion

        #region ContentTemplate

        private DataTemplate m_myHeaderTemplate;
        public DataTemplate HeaderTemplate
        {
            get => m_myHeaderTemplate;
            set => SetValue("HeaderTemplate", ref m_myHeaderTemplate, value);
        }

        #endregion


        #region ButtonText

        private string m_myButtonText;
        public string ButtonText
        {
            get => m_myButtonText;
            set => SetValue("ButtonText", ref m_myButtonText, value);
        }

        internal void SetCloseText()
        {
            ButtonText = m_myClose;
        }

        #endregion


        #region ShowDoIt

        private bool m_myShowDoIt;
        public bool ShowDoIt
        {
            get => m_myShowDoIt;
            set => SetValue("ShowDoIt", ref m_myShowDoIt, value);
        }

        #endregion

        #region ShowNext

        private bool m_myShowNext;
        public bool ShowNext
        {
            get => m_myShowNext || ButtonText == m_myClose;
            set => SetValue("ShowNext", ref m_myShowNext, value);
        }

        #endregion

        #region CurrentStepNo

        private int m_myCurrentStepNo = 1;
        /// <summary>
        /// Gets or sets the current step number (1-based) of the current tour.
        /// </summary>
        public int CurrentStepNo
        {
            get => m_myCurrentStepNo;
            internal set
            {
                if (SetValue("CurrentStepNo", ref m_myCurrentStepNo, value))
                    HasTourFinished = CurrentStepNo == TotalStepsCount;
            }
        }

        #endregion

        #region TotalStepCount

        private int m_myTotalStepsCount = 1;
        /// <summary>
        /// Gets or sets the number of total steps of the current tour.
        /// </summary>
        public int TotalStepsCount
        {
            get => m_myTotalStepsCount;
            internal set => SetValue("TotalStepsCount", ref m_myTotalStepsCount, value);
        }

        #endregion

        #region HasTourFinished

        private bool m_myHasTourFinished;
        /// <summary>
        /// Gets a value that indicates if the current step is the last one.
        /// </summary>
        public bool HasTourFinished
        {
            get => m_myHasTourFinished;
            internal set => SetValue("HasTourFinished", ref m_myHasTourFinished, value);
        }

        #endregion

        public ICommand CmdClose { get; }
        public ICommand CmdNext { get; }
        public ICommand CmdDoIt { get; }
    }
}
