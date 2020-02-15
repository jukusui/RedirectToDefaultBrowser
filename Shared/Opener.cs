using Shared.Ex;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shared
{
    public class Opener
    {

        const string MSEdgeScheme = "microsoft-edge:";

        public enum OpenerAppMode
        {
            DefaultBrowser,
            EdgeLegacy,
            PickWithUI
        }


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
                throw new NotMSEdgeSchemeException(arg);
        }

        public static Uri VaridateUri(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                    return uri;
                default:
                    throw new SchemeException(uri);
            }
        }

        private static bool CheckUri(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "http":
                case "https":
                    return true;
                default:
                    return false;
            }
        }

        private static IEnumerable<Uri> VaridateUri(IEnumerable<Uri> uris)
        {
            foreach (var uri in uris)
                yield return VaridateUri(uri);
        }

        public static async Task LaunchDefault(Uri uri)
        {
            var status = await Windows.System.Launcher.QueryUriSupportAsync(
                uri, Windows.System.LaunchQuerySupportType.Uri);
            switch (status)
            {
                case Windows.System.LaunchQuerySupportStatus.Available:
                    if (await Windows.System.Launcher.LaunchUriAsync(uri))
                        return;
                    break;
            }
            throw BrowserException.FromQueryStatus(status, uri);
        }

        public static async Task LaunchWithUI(Uri uri)
        {
            var opt = new Windows.System.LauncherOptions
            {
                DisplayApplicationPicker = true
            };
            opt.UI.InvocationPoint = new Windows.Foundation.Point(0, 0);
            opt.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below;
            var status = await Windows.System.Launcher.QueryUriSupportAsync(
                uri, Windows.System.LaunchQuerySupportType.Uri);
            switch (status)
            {
                case Windows.System.LaunchQuerySupportStatus.Available:
                    if (await Windows.System.Launcher.LaunchUriAsync(uri, opt))
                        return;
                    break;
            }
            throw BrowserException.FromQueryStatus(status, uri);
        }



        private static async Task LaunchEdge(Uri uri)
        {
            var opt = new Windows.System.LauncherOptions()
            {
                TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
            };
            var status = await Windows.System.Launcher.QueryUriSupportAsync(
                uri, Windows.System.LaunchQuerySupportType.Uri,
                opt.TargetApplicationPackageFamilyName);
            switch (status)
            {
                case Windows.System.LaunchQuerySupportStatus.Available:
                    if (await Windows.System.Launcher.LaunchUriAsync(uri, opt))
                        return;
                    break;
            }
            throw BrowserException.FromQueryStatus(status, uri);
        }

        public static async Task OpenRaw(string arg,
            bool useEdgeHTML = true)
        {
            if (arg.StartsWith(MSEdgeScheme))
                if (Uri.TryCreate(arg, UriKind.Absolute, out Uri uri))
                    if (useEdgeHTML)
                        await LaunchEdge(uri);
                    else
                        await LaunchWithUI(uri);
                else
                    throw new NotUrlException(arg);
            else
                throw new NotMSEdgeSchemeException(arg);
        }

        public static async Task Open(
            string arg, bool useRedirect = true, OpenerAppMode appMode = OpenerAppMode.DefaultBrowser)
        {
            var uris = VaridateUri(RecognizeArg(arg));
            if (useRedirect)
                uris = uris.Select(uri =>
                {
                    foreach (var redirect in Config.Redirect.Redirects)
                        uri = redirect.Apply(uri);
                    return uri;
                });
            var uriArray = uris.ToArray();
            if (uriArray.Length == 0)
                throw new NoUrlException(arg);
            foreach (var uri in uriArray)
                switch (appMode)
                {
                    case OpenerAppMode.DefaultBrowser:
                        await LaunchDefault(uri);
                        break;
                    case OpenerAppMode.EdgeLegacy:
                        await LaunchEdge(uri);
                        break;
                    case OpenerAppMode.PickWithUI:
                        await LaunchWithUI(uri);
                        break;
                }
        }
    }
}