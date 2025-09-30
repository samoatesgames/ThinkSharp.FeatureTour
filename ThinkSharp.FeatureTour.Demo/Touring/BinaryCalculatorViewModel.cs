// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ThinkSharp.FeatureTouring.Touring
{
    public class BinaryCalculatorViewModel : ObservableObject
    {
        private string m_text = string.Empty;
        private string m_operator = "";
        private int m_result;
        public string Text
        {
            get => m_text;
            set => SetProperty(ref m_text, value);
        }
        public ICommand CmdZero => new RelayCommand(() => Text = $"{(EnteringNumber ? Text : "")}0");
        public ICommand CmdOne => new RelayCommand(() => Text = $"{(EnteringNumber ? Text : "")}1");

        public ICommand CmdAdd => new RelayCommand(() =>
        {
            CalculateIntermadiateResult();
            m_operator = "+";
            Text = "+";

        }, () => EnteringNumber);
        public ICommand CmdSubstract => new RelayCommand(() =>
        {
            CalculateIntermadiateResult();
            m_operator = "-";
            Text = "-";
        }, () => EnteringNumber);
        public ICommand CmdResult => new RelayCommand(() => Text = CalculateIntermadiateResult(), () => EnteringNumber);

        public ICommand CmdClear => new RelayCommand(() =>
        {
            Text = "";
            m_result = 0;
            m_operator = "";
        });

        private string CalculateIntermadiateResult()
        {
            switch (m_operator)
            {
                case "+":
                    m_result += Convert.ToInt32(Text, 2);
                    break;
                case "-":
                    m_result -= Convert.ToInt32(Text, 2);
                    break;
                default:
                    m_result = Convert.ToInt32(Text, 2);
                    break;
            }
            m_operator = "";
            return Convert.ToString(m_result, 2);
        }

        private bool EnteringNumber => Text != "+" && Text != "-" && Text != "";
    }
}
