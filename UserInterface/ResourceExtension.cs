using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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


        public string? Name { get; set; }

        public string GroupName { get; set; } = "Resources";

        private static Dictionary<string, Func<ResourceManager>> _ResourceDict = new Dictionary<string, Func<ResourceManager>>();
        public static IReadOnlyDictionary<string, Func<ResourceManager>> ResourceDict { get => _ResourceDict; }

        private static ResourceManager GetResource(string group)
        {
            if (ResourceDict.TryGetValue(group, out var value))
                return value();
            var type = typeof(Shared.Properties.Resources).Assembly.GetType($"Shared.Properties.{group}");
            var prop = type.GetProperty("ResourceManager");
            var getter = (Func<ResourceManager>)prop.GetMethod.CreateDelegate(typeof(Func<ResourceManager>));
            _ResourceDict.Add(group, getter);
            return getter();
        }

        public static string? GetString(string group, string name)
        {
            string? result;
            try
            {
                if (name == null)
                    result = null;
                return
                    result = GetResource(group).GetString(name);
            }
            catch (Exception ex)
            {
                result = $"{ex.GetType().Name} @[{name}] ({ex.Message})";
            }
            if (result == null)
                result = $"null @[{name}]";
            return result;
        }

        protected override object ProvideValue() => GetString(GroupName, Name);
    }

    public class SharedResourceLoader : CustomXamlResourceLoader
    {
        protected override object GetResource(string resourceId, string objectType, string propertyName, string propertyType)
        {
            try
            {
                if (resourceId.Contains("."))
                {
                    var names = resourceId.Split('.');
                    var group = names.First();
                    var name = string.Join(".", names.Skip(1));
                    return ResourceExtension.GetString(group, name);
                }
                return
                    ResourceExtension.GetString("Resources", resourceId);
            }
            catch (Exception)
            {
                return resourceId;
            }
        }
    }
}
