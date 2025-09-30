// Copyright (c) Jan-Niklas Schäfer. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace ThinkSharp.FeatureTouring.Test
{
    public static class TestHelper
    {
        public static TFieldType GetPrivateField<TFieldType>(this object instance, string fieldName) where TFieldType : class
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = instance.GetType().GetField(fieldName, bindFlags);
            return field.GetValue(instance) as TFieldType;
        }
    }
}
