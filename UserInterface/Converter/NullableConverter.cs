using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UserInterface.Converter
{
    public class NullableConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Activator.CreateInstance(targetType, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
