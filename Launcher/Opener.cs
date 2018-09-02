using Launcher.Ex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Launcher
{
    class Opener
    {

        const string MSEdgeScheme = "microsoft-edge:";



        public static IEnumerable<Uri> RecognizeArg(string arg)
        {
            if (arg.StartsWith(MSEdgeScheme))
            {
                if (Uri.TryCreate(arg.Substring("microsoft-edge:".Length), UriKind.Absolute, out Uri oneUri))
                {
                    //スキーム:"URLそのまま"
                    //microsoft-edge:https://translate.google.co.jp/#ja/en/%E3%83%86%E3%82%B9%E3%83%88
                    yield return oneUri;
                }
                else
                {
                    //スキーム:url="URLエンコード済み"
                    //microsoft-edge:?url=https%3A%2F%2Ftranslate.google.co.jp%2F%23ja%2Fen%2F%25E3%2583%2586%25E3%2582%25B9%25E3%2583%2588
                    if (Uri.TryCreate(arg, UriKind.Absolute, out Uri source))
                    {
                        //"?"をスキップ、"&"で分割
                        if (string.IsNullOrWhiteSpace(source.Query))
                            yield break;
                        foreach (var query in
                            source.Query.Substring(1).Split('&'))
                        {
                            if (!query.StartsWith("url=", StringComparison.OrdinalIgnoreCase))
                                continue;
                            //"url="をスキップ
                            var urlStr = Uri.UnescapeDataString(query.Substring(4));
                            if (Uri.TryCreate(urlStr, UriKind.Absolute, out Uri url))
                            {
                                yield return url;
                            }
                        }
                    }
                }
            }
            else
                throw new MSEdgeSchemeException();
        }

        public static Uri VaridateUri(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                    return uri;
                default:
                    throw new SchemeException();
            }
        }

        public static IEnumerable<Uri> VaridateUri(IEnumerable<Uri> uris)
        {
            foreach (var uri in uris)
                yield return VaridateUri(uri);
        }

        public static async Task LaunchDefault(Uri uri)
        {
            if (!await Windows.System.Launcher.LaunchUriAsync(uri))
                throw new BrowserException();
        }

        public static async Task LaunchEdge(Uri uri)
        {
            var opt = new Windows.System.LauncherOptions()
            {
                TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
            };
            if (!await Windows.System.Launcher.LaunchUriAsync(uri, opt))
                throw new BrowserException();
        }

        public static async Task OpenRaw(string arg)
        {
            if (arg.StartsWith(MSEdgeScheme))
                if (Uri.TryCreate(arg, UriKind.Absolute, out Uri uri))
                    await LaunchEdge(uri);
                else
                    throw new NotUrlException();
            else
                throw new MSEdgeSchemeException();
        }

        public static async Task Open(
            string arg, bool useRedirect = true, bool useEdge = false)
        {
            var uris = VaridateUri(RecognizeArg(arg));
            if (useRedirect)
                uris = uris.Select(uri =>
                {
                    foreach (var redirect in Redirect.Instance.Redirects)
                        uri = redirect.Apply(uri);
                    return uri;
                });
            var uriArray = uris.ToArray();
            if (uriArray.Length == 0)
                throw new NoUrlException();
            foreach (var uri in uriArray)
                if (useEdge)
                    await LaunchEdge(uri);
                else
                    await LaunchDefault(uri);
        }
    }
}