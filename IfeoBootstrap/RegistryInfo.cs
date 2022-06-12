using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace IfeoBootstrap;
public class RegistryInfo
{

    private RegistryInfo()
    {
        Update();
    }

    public void Update()
    {
        var dict = ListAll(SCHEME_HTTP, SCHEME_MS_EDGE);

        if (dict.TryGetValue(SCHEME_HTTP, out var https))
            HttpApps = https.Distinct().Select(key => AppRegistry.TryCreate(key)!).Where(ar => ar != null).ToArray();
        else
            HttpApps = Array.Empty<AppRegistry>();

        if (dict.TryGetValue(SCHEME_MS_EDGE, out var edges))
            MsEdgeApps = edges.Distinct().Select(key => AppRegistry.TryCreate(key)!).Where(ar => ar != null && ar.IsMatchRoot).ToArray();
        else
            MsEdgeApps = Array.Empty<AppRegistry>();
    }


    private static readonly string SCHEME_HTTP = "http";
    private static readonly string SCHEME_MS_EDGE = "microsoft-edge";
    //全ての何らかの拡張子かURIに紐づけられたプログラムの一覧
    private static readonly string REGISTERED_APP = @"SOFTWARE\RegisteredApplications";
    //アプリの情報内のURIスキームに関する情報
    private static readonly string URL_ASSOC = @"URLAssociations";

    private static Dictionary<string, List<string>> ListAll(params string[] schemes)
    {
        var roots = new[] { Registry.LocalMachine, Registry.CurrentUser };
        var dict = new Dictionary<string, List<string>>();
        foreach (var root in roots)
        {
            if (root == null)
                continue;
            using var regApp = root.OpenSubKey(REGISTERED_APP);
            if (regApp != null)
            {
                foreach (var appName in regApp.GetValueNames())
                {
                    try
                    {
                        var appValues = regApp.GetValue(appName);
                        if (appValues is not string appStorage)
                            continue;
                        using var urlKey = root.OpenSubKey($@"{appStorage}\{URL_ASSOC}");
                        if (urlKey == null)
                            continue;
                        foreach (var urlName in urlKey.GetValueNames())
                        {
                            var urlNameL = urlName.ToLower();
                            if (schemes.Contains(urlNameL) &&
                                urlKey.GetValue(urlNameL) is string openerName)
                            {
                                if (!dict.TryGetValue(urlNameL, out var list))
                                {
                                    list = new List<string>();
                                    dict.Add(urlNameL, list);
                                }
                                list.Add(openerName);
                            }
                        }

                    }
                    catch (Exception) { }
                }
            }
        }
        return dict;
    }

    private static RegistryInfo? instance = null;

    public static RegistryInfo GetInfo()
    {
        if (instance == null)
            instance = new RegistryInfo();
        return instance;
    }

    public IReadOnlyList<AppRegistry> HttpApps { get; private set; } = null!;
    public IReadOnlyList<AppRegistry> MsEdgeApps { get; private set; } = null!;
}
