using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

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
        #region History
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

        #endregion

        #region Redirect
        public IList<RedirectSetting> Redirects { get; } = Redirect.Instance.Redirects;

        private void Redirect_Add_Clicked(object sender, RoutedEventArgs e)
        {
            var win = new RedirectInputWindow(Redirects);
            win.ShowDialog();
        }
        #endregion

        private void Redirect_Edit_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext != null && btn.DataContext is RedirectSetting target)
            {
                var win = new RedirectInputWindow(target);
                win.ShowDialog();
            }
        }

        private void Redirect_Remove_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext != null && btn.DataContext is RedirectSetting target)
                Redirects.Remove(target);
        }

        private void Redirect_Turn_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext != null && btn.DataContext is RedirectSetting target)
                target.Enable ^= true;
        }

        private void Redirect_Up_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext != null && btn.DataContext is RedirectSetting target)
            {
                var index = Redirects.IndexOf(target);
                Redirects.Remove(target);
                if (index == 0)
                    Redirects.Add(target);
                else
                    Redirects.Insert(index - 1, target);
            }
        }

        private void Redirect_Down_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext != null && btn.DataContext is RedirectSetting target)
            {
                var index = Redirects.IndexOf(target);
                Redirects.Remove(target);
                if (index == Redirects.Count)
                    Redirects.Insert(0, target);
                else
                    Redirects.Insert(index + 1, target);
            }
        }
    }
}
