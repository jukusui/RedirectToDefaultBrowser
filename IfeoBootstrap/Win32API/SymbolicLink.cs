using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap.Win32API;
internal class SymbolicLink
{
    [Flags]
    private enum SymLinkFlags : int
    {
        FLAG_FILE = 0,
        FLAG_DIRECTORY = 1,
        FLAG_ALLOW_UNPRIVILEGED_CREATE = 2
    }

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern Boolean CreateSymbolicLinkW(
        [MarshalAs(UnmanagedType.LPWStr)] string lpSymlinkFileName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpTargetFileName,
        int dwFlags);

    private static void CreateSymbolicLink(string src, string dest, SymLinkFlags flag)
    {
        if (CreateSymbolicLinkW(dest, src, (int)flag))
            return;
        throw new Win32Exception();
    }

    public static void CreateFileSymbolicLink(string src, string dest)
        => CreateSymbolicLink(src, dest, SymLinkFlags.FLAG_FILE);
    public static void CreateFolderSymbolicLink(string src, string dest)
        => CreateSymbolicLink(src, dest, SymLinkFlags.FLAG_DIRECTORY);

}
