using Microsoft.UI.Xaml.Data;
using System;

namespace AngioPlayer.Views.Converters;

public class ButtonImageConverter : IValueConverter
{
    // value = IsEnabled (bool)
    // parameter = base image name
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isEnabled = value is bool b && b;
        string baseName = parameter?.ToString() ?? "Default";

        string fileName = isEnabled ? $"{baseName}.svg" : $"{baseName}-disabled.svg";
        return new Uri($"ms-appx:///Assets/{fileName}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}