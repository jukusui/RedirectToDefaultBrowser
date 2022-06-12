using System;
using System.Diagnostics;

namespace IfeoBootstrap
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var info = RegistryInfo.GetInfo();
            var win = new Install.EdgeSelectWindow();
            foreach (var edge in info.MsEdgeApps)
            {
                win.Apps.Add(edge);
                var arg = edge.LaunchCommand;
                if (arg != null)
                    Console.WriteLine(arg[0]);
                else
                    Console.WriteLine("null");
            }
            win.SelectAll();
            Debug.WriteLine("RES:" + win.ShowDialog());
            //Console.ReadKey(false);
        }

    }
}