// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

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
            get => myBorderThickness;
            set => SetProperty(ref myBorderThickness, value);
        }

        public double BorderThicknessValue
        {
            get => myBorderThickness.Top;
            set => BorderThickness = new Thickness(value);
        }

        private double myCornerRadius = 3;
        public double CornerRadius
        {
            get => myCornerRadius;
            set => SetProperty(ref myCornerRadius, value);
        }

        private double myFontSize = 12;
        public double FontSize
        {
            get => myFontSize;
            set
            {
                if (SetProperty(ref myFontSize, value))
                    FeatureTour.GetNavigator().Close();
            }
        }

        private Brush myForeground = new SolidColorBrush(Color.FromRgb(0x04, 0x35, 0x6c));
        public Brush Foreground
        {
            get => myForeground;
            set => SetProperty(ref myForeground, value);
        }
        public Color? ForegroundColor
        {
            get => (Foreground as SolidColorBrush)?.Color;
            set => Foreground = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent;
        }

        private Brush myBackground = new SolidColorBrush(Color.FromRgb(0x68, 0x9a, 0xd3));
        public Brush Background
        {
            get => myBackground;
            set => SetProperty(ref myBackground, value);
        }
        public Color? BackgroundColor
        {
            get => (Background as SolidColorBrush)?.Color;
            set => Background = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent;
        }

        private Brush myBorderBrush = new SolidColorBrush(Color.FromRgb(0x27, 0x4f, 0x7d));
        public Brush BorderBrush
        {
            get => myBorderBrush;
            set => SetProperty(ref myBorderBrush, value);
        }
        public Color? BorderBrushColor
        {
            get => (BorderBrush as SolidColorBrush)?.Color;
            set => BorderBrush = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent;
        }
    }
}
