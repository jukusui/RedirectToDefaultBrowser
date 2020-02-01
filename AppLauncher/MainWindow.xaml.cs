using Shared;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace AppLauncher
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

        public string LastURL { get; } = Config.LastUrl;
        public bool HasLastData { get => !string.IsNullOrWhiteSpace(LastURL); }

        private async void Link_Clicked(object sender, RequestNavigateEventArgs e)
        {
            if (sender is Hyperlink hyperlink && hyperlink.NavigateUri != null)
            {
                try { await Opener.LaunchDefault(hyperlink.NavigateUri); }
                catch (Exception ex) { Launcher.ShowErrorMsg(ex); }
            }
        }

        private async void OpenDefault_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            try { await Opener.Open(LastURL); }
            catch (Exception ex) { Launcher.ShowErrorMsg(ex); }
            finally { btn.IsEnabled = true; }
        }

        private async void OpenEdge_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            try { await Opener.Open(LastURL, appMode: Opener.OpenerAppMode.PickWithUI); }
            catch (Exception ex) { Launcher.ShowErrorMsg(ex); }
            finally { btn.IsEnabled = true; }
        }

        private async void OpenRaw_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            btn.IsEnabled = false;
            try { await Opener.OpenRaw(LastURL); }
            catch (Exception ex) { Launcher.ShowErrorMsg(ex); }
            finally { btn.IsEnabled = true; }
        }

        private void Copy_Clicked(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LastURL);
        }

        #endregion

        #region Redirect
        public IList<RedirectSetting> Redirects { get; } = Config.Redirect.Redirects;

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
                target.WillRemove ^= true;
        }

        private void Redirect_Turn_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext != null && btn.DataContext is RedirectSetting target)
                target.Enable = !target.Enable;
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
