// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Windows;
using ThinkSharp.FeatureTouring.Navigation;

namespace ThinkSharp.FeatureTouring
{
    public partial class Dialog
    {
        public Dialog()
        {
            InitializeComponent();
        }

        private void ButtonBaseOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMeToOnClick(object sender, RoutedEventArgs e)
        {
            var tour = FeatureTour.GetNavigator();

            tour.IfCurrentStepEquals(ElementID.ButtonPushMeToo).GoNext();
        }
    }
}
