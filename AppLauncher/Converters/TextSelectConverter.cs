using System;
using System.Globalization;
using System.Windows.Data;

namespace AppLauncher.Converters
{
    class TextSelectConverter : IValueConverter
    {
        public string? Splitter { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sTrue, sFalse;
            if(parameter is string[] arr)
            {
                switch (arr.Length)
                {
                    case 0:
                        return null;
                    case 1:
                        return arr[0];
                    default:
                        sTrue = arr[0];
                        sFalse = arr[1];
                        break;
                }
            }
            else
            {
                var p = parameter?.ToString() ?? "";
                var i = Splitter == null ? p.Length : p.IndexOf(Splitter);
                sTrue = i < 0 ? p : p.Substring(0, i);
                sFalse = i < 0 ? "" : p.Substring(i + (Splitter?.Length ?? 0));
            }
            if ((value is bool b) && b)
                return sTrue;
            else
                return sFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
