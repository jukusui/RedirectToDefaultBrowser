using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage;

namespace Shared
{

    public static class Config
    {

        static Config()
        {
            try
            {
                ReadConfig();
            }
            catch (Exception) { }

            Redirect = Redirect ?? new Redirect(null);
        }

        public static string? LastUrl { get; set; }

        public static Redirect Redirect { get; private set; }


        public const string VerKey = "Version";

        public static void ReadConfig()
        {
            var load = false;
            var local = ApplicationData.Current.LocalSettings;
            if (local.Values.ContainsKey(VerKey) &&
                local.Values[VerKey] is int version)
            {
                switch (version)
                {
                    case 1:
                        if (
                            local.Values.ContainsKey(nameof(LastUrl)) &&
                            local.Values[nameof(LastUrl)] is string lastUrl)
                            LastUrl = lastUrl;
                        else
                            LastUrl = null;

                        RedirectSetting[]? redirectSettings = null;
                        if (local.Values.ContainsKey(nameof(Redirect)) &&
                            local.Values[nameof(Redirect)] is string redirect)
                            redirectSettings =
                                DeserializeRedirectSettings(redirect);
                        Redirect = new Redirect(redirectSettings);

                        load = true;
                        break;
                }
            }
            if (!load)
            {
                foreach (var file in FindOldConfig())
                    if (ReadOldConfig(file))
                    {
                        if (Redirect == null)
                            Redirect = new Redirect(null);
                        Save();
                        load = true;
                        break;
                    }
            }
            if (!load)
            {
                Redirect = new Redirect(null);
                Save();
            }

        }

        private static IEnumerable<FileInfo> FindOldConfig()
        {
            var path = ApplicationData.Current.LocalCacheFolder.Path +
                $"/Local/Launcher/";
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
                yield break;
            foreach (var child in dir.GetDirectories())
            {
                if (child.Name.StartsWith("Launcher.exe_"))
                {
                    var target = child.GetDirectories().FirstOrDefault(d => d.Name == "1.0.0.0");
                    var file = new FileInfo($@"{target.FullName}/user.config");
                    if (file.Exists)
                        yield return file;
                }
            }
        }

        private static bool ReadOldConfig(FileInfo file)
        {
            try
            {
                var result = false;
                const string settings = "/configuration/userSettings/Launcher.Properties.Settings/";
                var xml = new XmlDocument();
                using (var stream = file.OpenRead())
                    xml.Load(stream);
                var lastUrlNodes = xml.SelectNodes(settings + "setting[@name='LastURL']/value");
                if (lastUrlNodes.Count == 1)
                {
                    LastUrl = lastUrlNodes[0].InnerText;
                    result = true;
                }
                var redirectsNodes = xml.SelectNodes(settings + "setting[@name='Redirects']/value");
                if (redirectsNodes.Count == 1)
                {
                    Redirect = new Redirect(
                        DeserializeRedirectSettings(redirectsNodes[0].InnerXml));
                    result = true;
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void Save(bool skipLastUrl = false)
        {
            try
            {
                var local = ApplicationData.Current.LocalSettings;
                local.Values[VerKey] = 1;
                if (skipLastUrl)
                    local.Values[nameof(LastUrl)] = LastUrl;
                if (Redirect != null)
                {
                    Redirect.Refresh();
                    local.Values[nameof(Redirect)] = SerializeRedirectSettings(Redirect.Redirects.ToArray());
                }
            }
            catch (InvalidOperationException) { }
        }

        private static string SerializeRedirectSettings(RedirectSetting[] redirectSettings)
        {
            var serializer = new XmlSerializer(typeof(RedirectSetting[]));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, redirectSettings ?? Array.Empty<RedirectSetting>());
                return writer.ToString();
            }
        }

        private static RedirectSetting[]? DeserializeRedirectSettings(string xml)
        {
            var deserializer = new XmlSerializer(typeof(RedirectSetting[]));
            try
            {
                using (var reader = new StringReader(xml))
                    if (deserializer.Deserialize(reader) is RedirectSetting[] result)
                        return result;
            }
            catch (InvalidOperationException) { }
            return null;
        }

    }
}
