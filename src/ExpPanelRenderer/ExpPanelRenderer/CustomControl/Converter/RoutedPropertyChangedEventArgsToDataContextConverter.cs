using System;
using System.Globalization;
using ExpPanelRenderer.Common;
using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.Converter;

public class RoutedPropertyChangedEventArgsToDataContextConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CustomEventArgs args)
        {
            return args.CurrentDataContext;
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}