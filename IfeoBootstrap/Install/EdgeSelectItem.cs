using Jukusui.Notify;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap.Install;
public class EdgeSelectItem : NotifyBase
{

    public EdgeSelectItem() { }
    public EdgeSelectItem(string name, string path)
    {
        Name = name;
        Path = path;
    }

    private bool isChecked = false;
    private string path = "";
    private string name = "";

    public string Name { get => name; set => OnPropertyChanged(ref name, value); }
    public string Path { get => path; set => OnPropertyChanged(ref path, value); }
    public bool IsChecked { get => isChecked; set => OnPropertyChanged(ref isChecked, value); }
}
