using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap.Win32API;
internal class RegistryRedirect
{

    //public static RegistryKey HKLM => _hklm ??=
    //    RegistryKey.OpenBaseKey(
    //        RegistryHive.LocalMachine, RegistryView.Registry64);
    //private static RegistryKey? _hklm = null;

    //public static RegistryKey CLASS => _classes ??=
    //    RegistryKey.OpenBaseKey(
    //        RegistryHive.ClassesRoot, RegistryView.Registry64);
    //private static RegistryKey? _classes = null;
    public static RegistryKey HKLM => Registry.LocalMachine;

    public static RegistryKey CLASS => Registry.ClassesRoot;
}
