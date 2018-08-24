using System;
using System.Globalization;
using System.Windows.Data;

namespace Launcher.Converters
{
    class TextSelectConverter : IValueConverter
    {
        public string Splitter { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = parameter?.ToString() ?? "";
            var i = p.IndexOf(Splitter);
            var sTrue = i < 0 ? p : p.Substring(0, i);
            var sFalse = i < 0 ? "" : p.Substring(i + Splitter.Length);
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
