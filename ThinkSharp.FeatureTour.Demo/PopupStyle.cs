// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ThinkSharp.FeatureTouring.Navigation;

namespace ThinkSharp.FeatureTouring
{
    public class PopupStyle : ObservableObject
    {
        private Thickness myBorderThickness = new Thickness(3);
        public Thickness BorderThickness
        {
            get { return myBorderThickness; }
            set { SetProperty(ref myBorderThickness, value, "BorderThickness"); }
        }

        public double BorderThicknessValue
        {
            get { return myBorderThickness.Top; }
            set { BorderThickness = new Thickness(value); }
        }

        private double myCornerRadius = 3;
        public double CornerRadius
        {
            get { return myCornerRadius; }
            set { SetProperty(ref myCornerRadius, value, "CornerRadius"); }
        }

        private double myFontSize = 12;
        public double FontSize
        {
            get { return myFontSize; }
            set
            {
                if (SetProperty(ref myFontSize, value, "FontSize"))
                    FeatureTour.GetNavigator().Close();
            }
        }

        private Brush myForeground = new SolidColorBrush(Color.FromRgb(0x04, 0x35, 0x6c));
        public Brush Foreground
        {
            get { return myForeground; }
            set { SetProperty(ref myForeground, value, "Foreground"); }
        }
        public Color? ForegroundColor
        {
            get { return (Foreground as SolidColorBrush)?.Color; }
            set { Foreground = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent; }
        }

        private Brush myBackground = new SolidColorBrush(Color.FromRgb(0x68, 0x9a, 0xd3));
        public Brush Background
        {
            get { return myBackground; }
            set { SetProperty(ref myBackground, value, "Background"); }
        }
        public Color? BackgroundColor
        {
            get { return (Background as SolidColorBrush)?.Color; }
            set { Background = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent; }
        }

        private Brush myBorderBrush = new SolidColorBrush(Color.FromRgb(0x27, 0x4f, 0x7d));
        public Brush BorderBrush
        {
            get { return myBorderBrush; }
            set { SetProperty(ref myBorderBrush, value, "BorderBrush"); }
        }
        public Color? BorderBrushColor
        {
            get { return (BorderBrush as SolidColorBrush)?.Color; }
            set { BorderBrush = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent; }
        }
    }
}
