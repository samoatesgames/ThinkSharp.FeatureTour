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
        private Thickness m_myBorderThickness = new Thickness(3);
        public Thickness BorderThickness
        {
            get => m_myBorderThickness;
            set => SetProperty(ref m_myBorderThickness, value);
        }

        public double BorderThicknessValue
        {
            get => m_myBorderThickness.Top;
            set => BorderThickness = new Thickness(value);
        }

        private double m_myCornerRadius = 3;
        public double CornerRadius
        {
            get => m_myCornerRadius;
            set => SetProperty(ref m_myCornerRadius, value);
        }

        private double m_myFontSize = 12;
        public double FontSize
        {
            get => m_myFontSize;
            set
            {
                if (SetProperty(ref m_myFontSize, value))
                    FeatureTour.GetNavigator().Close();
            }
        }

        private Brush m_myForeground = new SolidColorBrush(Color.FromRgb(0x04, 0x35, 0x6c));
        public Brush Foreground
        {
            get => m_myForeground;
            set => SetProperty(ref m_myForeground, value);
        }
        public Color? ForegroundColor
        {
            get => (Foreground as SolidColorBrush)?.Color;
            set => Foreground = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent;
        }

        private Brush m_myBackground = new SolidColorBrush(Color.FromRgb(0x68, 0x9a, 0xd3));
        public Brush Background
        {
            get => m_myBackground;
            set => SetProperty(ref m_myBackground, value);
        }
        public Color? BackgroundColor
        {
            get => (Background as SolidColorBrush)?.Color;
            set => Background = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent;
        }

        private Brush m_myBorderBrush = new SolidColorBrush(Color.FromRgb(0x27, 0x4f, 0x7d));
        public Brush BorderBrush
        {
            get => m_myBorderBrush;
            set => SetProperty(ref m_myBorderBrush, value);
        }
        public Color? BorderBrushColor
        {
            get => (BorderBrush as SolidColorBrush)?.Color;
            set => BorderBrush = value.HasValue ? new SolidColorBrush(value.Value) : Brushes.Transparent;
        }
    }
}
