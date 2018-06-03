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
            UrlBox.Text = LastURL;
            vText.Text = typeof(Launcher).Assembly.GetName().Version.ToString();

        }

        public string LastURL { get; } = Properties.Settings.Default.LastURL;
        public bool HasLastData { get => !string.IsNullOrWhiteSpace(LastURL); }

        private void Link_Clicked(object sender, RequestNavigateEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.NavigateUri != null)
            {
                System.Diagnostics.Process.Start(hyperlink.NavigateUri.AbsoluteUri);
            }
        }

        private async void OpenDefault_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            string message = null;
            btn.IsEnabled = false;
            try
            {
                message = await Launcher.Launch(LastURL);
            }
            catch (Exception ex)
            {
                message = $"予期せぬエラー\n{ex}";
            }
            finally
            {
                btn.IsEnabled = true;
            }
            if (message != null)
                MessageBox.Show("ブラウザを開けませんでした\n\n" + message, Launcher.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void OpenEdge_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            try
            {
                var uri = new Uri(LastURL);
                var opt = new Windows.System.LauncherOptions()
                {
                    TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
                };
                await Windows.System.Launcher.LaunchUriAsync(uri, opt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Edgeを開けませんでした\n\n予期せぬエラー\n{ex}", Launcher.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }

        private void Copy_Clicked(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LastURL);
        }
    }
}
