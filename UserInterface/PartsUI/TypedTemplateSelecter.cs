using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UserInterface.PartsUI
{
    [ContentProperty(Name = "Templates")]
    public class TypedTemplateSelecter : DataTemplateSelector
    {


        public static DependencyProperty TargetTypeProperty =
            DependencyProperty.RegisterAttached(
                "TargetType", typeof(string), typeof(TypedTemplateSelecter),
                new PropertyMetadata(null));

        public static void SetTargetType(DependencyObject target, string? value) =>
            target.SetValue(TargetTypeProperty, value);

        public static string? GetTargetType(DependencyObject target) =>
            target.GetValue(TargetTypeProperty) as string;

        public static Type? GetTargetTypeT(DependencyObject target) =>
            Type.GetType(GetTargetType(target));


        public ObservableCollection<DataTemplate> Templates { get; set; } =
            new ObservableCollection<DataTemplate>();


        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item == null)
                return base.SelectTemplateCore(item);
            var target = item.GetType();
            foreach (var template in Templates)
            {
                var type = GetTargetTypeT(template);
                if (type == null)
                    continue;
                if (type.IsAssignableFrom(target))
                    return template;
            }
            return base.SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
