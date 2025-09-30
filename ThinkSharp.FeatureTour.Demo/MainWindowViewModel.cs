// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ThinkSharp.FeatureTouring.Navigation;
using ThinkSharp.FeatureTouring.Touring;

namespace ThinkSharp.FeatureTouring
{
    public class MainWindowViewModel : ObservableObject
    {
        private ICommand m_cmdStartPositioning;
        private ICommand m_cmdStartIntroduction;
        private ICommand m_cmdStartActiveTour;
        private ICommand m_cmdStartDialogTour;
        private ICommand m_cmdStartCustomViewTour;
        private ICommand m_cmdStartOverView;
        private ICommand m_cmdOpenDialog;
        private ICommand m_cmdClear;
        private Placement m_placement;
        private int m_colorSchemaIndex;
        private int m_tabIndex;
        private string m_path;
        private string m_result;
        private int m_selectedIndex;
        private string m_styleText;

        private readonly PopupStyle m_myPopupStyle = new PopupStyle();


        // .ctor

        private MainWindowViewModel()
        {
            FeatureTour.SetViewModelFactoryMethod(tourRun => new CustomTourViewModel(tourRun));

            var navigator = FeatureTour.GetNavigator();

            navigator.ForStep(ElementId.TextBoxPath).AttachDoable(_ => Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            navigator.ForStep(ElementId.ComboBoxOption).AttachDoable(_ => SelectedIndex = 1);
            navigator.ForStep(ElementId.TextBoxPath).AttachDoable(_ => Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

            PopupStyle.PropertyChanged += (_, _) => StyleText = GetStyleText();
            StyleText = GetStyleText();
        }

        private string GetStyleText()
        {
            var sb = new StringBuilder();

            sb.AppendLine("...");
            sb.AppendLine("xmlns:featureTouringControls=\"clr -namespace:ThinkSharp.FeatureTouring.Controls;assembly=ThinkSharp.FeatureTour\"");
            sb.AppendLine("...");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("<Style TargetType=\"featureTouringControls: TourControl\">");
            sb.AppendLine($"    <Setter Property=\"Background\" Value=\"{m_myPopupStyle.BackgroundColor}\"/>");
            sb.AppendLine($"    <Setter Property=\"BorderBrush\" Value=\"{m_myPopupStyle.BorderBrushColor}\"/>");
            sb.AppendLine($"    <Setter Property=\"Foreground\" Value=\"{m_myPopupStyle.ForegroundColor}\"/>");
            sb.AppendLine($"    <Setter Property=\"FontSize\" Value=\"{m_myPopupStyle.FontSize:0}\"/>");
            sb.AppendLine($"    <Setter Property=\"CornerRadius\" Value=\"{m_myPopupStyle.CornerRadius:0}\"/>");
            sb.AppendLine($"    <Setter Property=\"BorderThickness\" Value=\"{m_myPopupStyle.BorderThickness.Top:0}\"/>");
            sb.AppendLine("</Style>");
            return sb.ToString();
        }


        // Methods

        private void Clear()
        {
            Result = "";
            Path = "";
            SelectedIndex = 0;
            FeatureTour.GetNavigator()
                .IfCurrentStepEquals(ElementId.ButtonClear)
                .Close();
        }


        // Commands

        public ICommand CmdStartPositioning
        {
            get
            {
                if (m_cmdStartPositioning == null)
                {
                    m_cmdStartPositioning = new RelayCommand(TourStarter.StartPositioning);
                }
                return m_cmdStartPositioning;
            }
        }
        
        public ICommand CmdStartIntroduction
        {
            get
            {
                if (m_cmdStartIntroduction == null)
                {
                    m_cmdStartIntroduction = new RelayCommand(TourStarter.StartIntroduction);
                }
                return m_cmdStartIntroduction;
            }
        }

        public ICommand CmdStartActiveTour
        {
            get
            {
                if (m_cmdStartActiveTour == null)
                {
                    m_cmdStartActiveTour = new RelayCommand(TourStarter.StartActiveTour);
                }
                return m_cmdStartActiveTour;
            }
        }

        public ICommand CmdStartDialogTour
        {
            get
            {
                if (m_cmdStartDialogTour == null)
                {
                    m_cmdStartDialogTour = new RelayCommand(TourStarter.StartDialogTour);
                }
                return m_cmdStartDialogTour;
            }
        }

        public ICommand CmdStartCustomView
        {
            get
            {
                if (m_cmdStartCustomViewTour == null)
                {
                    m_cmdStartCustomViewTour = new RelayCommand(TourStarter.StartCustomViewTour);
                }
                return m_cmdStartCustomViewTour;
            }
        }

        public ICommand CmdStartOverView
        {
            get
            {
                if (m_cmdStartOverView == null)
                {
                    m_cmdStartOverView = new RelayCommand(TourStarter.StartOverView);
                }
                return m_cmdStartOverView;
            }
        }

        public ICommand CmdOpenDialog
        {
            get
            {
                if (m_cmdOpenDialog == null)
                {
                    m_cmdOpenDialog = new RelayCommand(() =>
                    {
                        var dlg = new Dialog();
                        dlg.ShowDialog();
                    });
                }
                return m_cmdOpenDialog;
            }
        }

        public ICommand CmdClear
        {
            get
            {
                if (m_cmdClear == null)
                {
                    m_cmdClear = new RelayCommand(Clear);
                }
                return m_cmdClear;
            }
        }


        // Properties

        public Placement Placement
        {
            get => m_placement;
            set => SetProperty(ref m_placement, value);
        }
        
        public int ColorSchemaIndex
        {
            get => m_colorSchemaIndex;
            set => SetProperty(ref m_colorSchemaIndex, value);
        }

        public int TabIndex
        {
            get => m_tabIndex;
            set => SetProperty(ref m_tabIndex, value);
        }

        public string Path
        {
            get => m_path;
            set => SetProperty(ref m_path, value);
        }

        public string Result
        {
            get => m_result;
            set => SetProperty(ref m_result, value);
        }

        public int SelectedIndex
        {
            get => m_selectedIndex;
            set => SetProperty(ref m_selectedIndex, value);
        }

        public string StyleText
        {
            get => m_styleText;
            set => SetProperty(ref m_styleText, value);
        }

        public PopupStyle PopupStyle => m_myPopupStyle;

        public static MainWindowViewModel Instance { get; } = new MainWindowViewModel();
    }
}
