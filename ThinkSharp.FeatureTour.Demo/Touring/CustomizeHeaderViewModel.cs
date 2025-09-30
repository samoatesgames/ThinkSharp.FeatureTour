// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace ThinkSharp.FeatureTouring.Touring
{
    public class CustomizeHeaderViewModel : ObservableObject
    {
        private string m_header = "My Header";

        public string Header
        {
            get => m_header;
            set => SetProperty(ref m_header, value);
        }
    }
}
