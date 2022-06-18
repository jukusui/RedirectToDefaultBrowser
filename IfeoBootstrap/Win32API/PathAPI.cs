using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap.Win32API;
internal unsafe class PathAPI
{

    private enum ItemType : int
    {
        File = 0x80,
        Directory = 0x10,
    }

    [DllImport("Shlwapi.dll")]
    private static extern bool PathRelativePathToW(char* pszPath,
        [MarshalAs(UnmanagedType.LPWStr)] string pszFrom, [MarshalAs(UnmanagedType.I4)] ItemType dwAttrFrom,
        [MarshalAs(UnmanagedType.LPWStr)] string pszTo, [MarshalAs(UnmanagedType.I4)] ItemType dwAttrTo);

    public static string GetRelativePath(string abstractPath, string basePath, bool abstractIsFile = false, bool baseIsFile = false)
    {
        char* output = stackalloc char[260];
        if (PathRelativePathToW(output,
            basePath, baseIsFile ? ItemType.File : ItemType.Directory,
            abstractPath, abstractIsFile ? ItemType.File : ItemType.Directory))
            return new string(output);
        else
            throw new IOException();
    }


}
