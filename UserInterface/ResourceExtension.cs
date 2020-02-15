using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Resources;

namespace UserInterface
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class ResourceExtension : MarkupExtension
    {

        public ResourceExtension() { }
        public ResourceExtension(string name) { Name = name; }


        public string Name { get; set; }

        protected override object ProvideValue()
        {
            try
            {
                if (Name == null)
                    return null;
                return
                    Shared.Properties.Resources.ResourceManager.GetString(Name, Shared.Properties.Resources.Culture);
            }catch(Exception ex)
            {
                return $"{ex.GetType().Name} @[{Name}]({ex.Message})";
            }
        }
    }

    public class SharedResourceLoader : CustomXamlResourceLoader
    {
        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            return
                Shared.Properties.Resources.ResourceManager.GetString(resourceId, Shared.Properties.Resources.Culture);
        }
    }
}
