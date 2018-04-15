using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            vText.Text = typeof(Launcher).Assembly.GetName().Version.ToString();

        }

        private void Link_Clicked(object sender, RequestNavigateEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.NavigateUri != null)
            {
                System.Diagnostics.Process.Start(hyperlink.NavigateUri.AbsoluteUri);
            }
        }
    }
}
