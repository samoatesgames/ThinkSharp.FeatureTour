// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Windows.Controls;
using ThinkSharp.FeatureTouring.Navigation;

namespace ThinkSharp.FeatureTouring
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = MainWindowViewModel.Instance;

            MyTbResult.TextChanged += TbResultTextChanged;
            MyTbPath.TextChanged += TbPathTextChanged;
            MyCbOptions.SelectionChanged += CbOptionsSelectionChanged;

            var navigator = FeatureTour.GetNavigator();

            navigator.OnStepEntering(ElementId.Rectangle).Execute(s =>
            {
                if (DataContext is MainWindowViewModel mainWindowViewModel)
                {
                    mainWindowViewModel.Placement = (Placement)s.Tag;
                }
                TabControl.SelectedIndex = 0;

            });
            navigator.OnStepEntering(ElementId.TextBoxResult).Execute(_ => TabControl.SelectedIndex = 1);
            navigator.OnStepEntering(ElementId.ButtonPushMe).Execute(_ => TabControl.SelectedIndex = 2);
            navigator.OnStepEntering(ElementId.CustomView).Execute(_ => TabControl.SelectedIndex = 3);

            navigator.OnStepEntered(ElementId.TextBoxResult).Execute(_ => MyTbResult.Focus());
            navigator.OnStepEntered(ElementId.TextBoxPath).Execute(_ => MyTbPath.Focus());
            navigator.OnStepEntered(ElementId.ComboBoxOption).Execute(_ => MyCbOptions.Focus());
            navigator.OnStepEntered(ElementId.ButtonClear).Execute(_ => MyBtnClear.Focus());
        }

        private void CbOptionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyCbOptions.SelectedIndex == 1)
            {
                var navigator = FeatureTour.GetNavigator();
                navigator.IfCurrentStepEquals(ElementId.ComboBoxOption).GoNext();
            }
        }

        private void TbPathTextChanged(object sender, TextChangedEventArgs e)
        {
            var path = MyTbPath.Text.Trim(' ', '\\');
            var expectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Trim(' ', '\\');
            if (path.Equals(expectedPath, StringComparison.InvariantCultureIgnoreCase))
            {
                var navigator = FeatureTour.GetNavigator();
                navigator.IfCurrentStepEquals(ElementId.TextBoxPath).GoNext();
            }
        }

        private void TbResultTextChanged(object sender, TextChangedEventArgs e)
        {
            var result = MyTbResult.Text.Trim();
            if (result == "21")
            {
                var navigator = FeatureTour.GetNavigator();
                navigator.IfCurrentStepEquals(ElementId.TextBoxResult).GoNext();
            }
        }
    }
}
