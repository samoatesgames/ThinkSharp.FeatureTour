// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ThinkSharp.FeatureTouring.Touring
{
    public class BinaryCalculatorViewModel : ObservableObject
    {
        private string _text = string.Empty;
        private string _operator = "";
        private int _result = 0;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value, "Text"); }
        }
        public ICommand CmdZero => new RelayCommand(() => Text = (EnteringNumber ? Text : "") + "0");
        public ICommand CmdOne => new RelayCommand(() => Text = (EnteringNumber ? Text : "") + "1");

        public ICommand CmdAdd => new RelayCommand(() =>
        {
            CalculateIntermadiateResult();
            _operator = "+";
            Text = "+";

        }, () => EnteringNumber);
        public ICommand CmdSubstract => new RelayCommand(() =>
        {
            CalculateIntermadiateResult();
            _operator = "-";
            Text = "-";
        }, () => EnteringNumber);
        public ICommand CmdResult => new RelayCommand(() => Text = CalculateIntermadiateResult(), () => EnteringNumber);

        public ICommand CmdClear => new RelayCommand(() =>
        {
            Text = "";
            _result = 0;
            _operator = "";
        });

        private string CalculateIntermadiateResult()
        {
            switch (_operator)
            {
                case "+":
                    _result += Convert.ToInt32(Text, 2);
                    break;
                case "-":
                    _result -= Convert.ToInt32(Text, 2);
                    break;
                default:
                    _result = Convert.ToInt32(Text, 2);
                    break;
            }
            _operator = "";
            return Convert.ToString(_result, 2);
        }

        private bool EnteringNumber
        {
            get { return Text != "+" && Text != "-" && Text != ""; }
        }
    }
}
