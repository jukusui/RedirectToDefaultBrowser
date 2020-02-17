using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using UserInterface.PartsUI;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UserInterface
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : NavigatablePage
    {
        public MainPage()
        {
            LastUrl = Config.LastUrl;

            RedirectTest = new FullRedirectTest()
            {
                Test = "https://www.bing.com/search?q=Example",
                Redirects = Config.Redirect.Redirects
            };
            OnEditErrorChanged(RedirectTest);
            RedirectTest.ErrorsChanged += OnEditErrorChanged;

            this.InitializeComponent();
        }


        public static DependencyProperty LastUrlProperty =
        DependencyProperty.Register(
            nameof(LastUrl), typeof(string), typeof(MainPage),
            new PropertyMetadata(null));

        public string? LastUrl
        {
            get => (string?)GetValue(LastUrlProperty);
            set => SetValue(LastUrlProperty, value);
        }



        ////最近使ったファイル(JumpList)
        ////https://docs.microsoft.com/ja-jp/windows/uwp/files/how-to-track-recently-used-files-and-folders
        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    var str = Shared.Config.LastUrl;
        //    Debug.WriteLine(str);
        //    var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
        //    jumpList.SystemGroupKind = Windows.UI.StartScreen.JumpListSystemGroupKind.Recent;
        //    foreach (var item in jumpList.Items)
        //    {
        //        Debug.WriteLine(item.Description);
        //    }
        //}

        #region Open

        private static string UnboxUriText(object item)
            => item switch
            {
                null => "",
                UriWrapper wrapper => wrapper.Uri.AbsoluteUri,
                Uri uri => uri.AbsoluteUri,
                string str => str,
                _ => item?.ToString(),
            };

        private static Uri TryToUri(string str)
        {
            if (Uri.TryCreate(str, UriKind.Absolute, out Uri res))
                return res;
            return null;
        }

        private static Uri UnboxUri(object item)
            => item switch
            {
                null => null,
                UriWrapper wrapper => wrapper.Uri,
                Uri uri => uri,
                string str => TryToUri(str),
                _ => TryToUri(item?.ToString()),
            };

        private async void OnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            try
            {
                String strClp;
                Uri uriClp = null;
                if (btn.DataContext is UriWrapper wrapper)
                {
                    strClp = wrapper.Uri.AbsoluteUri;
                    uriClp = wrapper.Uri;
                }
                else
                {
                    strClp = btn.DataContext?.ToString() ?? "";
                    Uri.TryCreate(strClp, UriKind.Absolute, out uriClp);
                }
                var data = new DataPackage();
                data.SetText(strClp);
                if (uriClp != null)
                    if (uriClp.Scheme == "http" || uriClp.Scheme == "https")
                        data.SetWebLink(uriClp);
                    else
                        data.SetApplicationLink(uriClp);
                Clipboard.SetContent(data);
            }
            catch (Exception ex)
            {
                await new ExceptionDialog(ex).ShowAsync();
                return;
            }

            var flyout = FlyoutBase.GetAttachedFlyout(btn);
            var alreadyClosed = false;
            var canceller = new TaskCompletionSource<bool>();
            void closed(object f, object e)
            {
                alreadyClosed = true;
                canceller.SetResult(true);
            }
            flyout.Closed += closed;
            flyout.ShowAt(btn);

            await Task.WhenAny(
                canceller.Task,
                Task.Delay(1000));

            flyout.Closed -= closed;
            if (!alreadyClosed)
                flyout.Hide();
        }

        private ExecutableKey LaunchKey { get; } = new ExecutableKey();

        #region Open(edge scheme)

        private AsyncCommand? _OpenWithDefaultBrowser;
        private AsyncCommand OpenWithDefaultBrowser
        {
            get =>
                _OpenWithDefaultBrowser ??
                (_OpenWithDefaultBrowser =
                new AsyncCommand(OnOpenWithDefaultBrowser, LaunchKey));
        }

        private async Task OnOpenWithDefaultBrowser(object param)
        {
            try
            {
                await Opener.Open(UnboxUriText(param));
            }
            catch (Exception ex)
            {
                await new ExceptionDialog(ex).ShowAsync();
            }
        }

        private AsyncCommand? _OpenWithSelectedEdge;
        private AsyncCommand OpenWithSelectedEdge
        {
            get =>
                _OpenWithSelectedEdge ??
                (_OpenWithSelectedEdge =
                new AsyncCommand(OnOpenWithSelectedEdge, LaunchKey));
        }

        private async Task OnOpenWithSelectedEdge(object param)
        {
            try
            {
                await Opener.OpenRaw(UnboxUriText(param), false);
            }
            catch (Exception ex)
            {
                await new ExceptionDialog(ex).ShowAsync();
            }
        }

        #endregion
        #region Launch(http scheme)

        private AsyncCommand? _LaunchWithDefaultBrowser;
        public AsyncCommand LaunchWithDefaultBrowser
        {
            get =>
                _LaunchWithDefaultBrowser ??
                (_LaunchWithDefaultBrowser =
                new AsyncCommand(OnLaunchWithDefaultBrowser, LaunchKey));
        }

        private async Task OnLaunchWithDefaultBrowser(object param)
        {
            try
            {
                await Opener.LaunchDefault(UnboxUri(param));
            }
            catch (Exception ex)
            {
                await new ExceptionDialog(ex).ShowAsync();
            }
        }

        private AsyncCommand? _LaunchWithSelectedBrowser;
        public AsyncCommand LaunchWithSelectedBrowser
        {
            get =>
                _LaunchWithSelectedBrowser ??
                (_LaunchWithSelectedBrowser =
                new AsyncCommand(OnLaunchWithSelectedBrowser, LaunchKey));
        }

        private async Task OnLaunchWithSelectedBrowser(object param)
        {
            try
            {
                await Opener.LaunchWithUI(UnboxUri(param));
            }
            catch (Exception ex)
            {
                await new ExceptionDialog(ex).ShowAsync();
            }
        }

        #endregion


        private IEnumerable GetUrlList(string lastUrl)
        {
            var result = new List<object>();
            try
            {
                foreach (var item in Opener.RecognizeArg(lastUrl))
                    result.Add(new UriWrapper(item));
            }
            catch (Exception ex)
            {
                result.Add(ex.Message);
            }
            return result;
        }

        #endregion

        #region Redirect

        private ICommand SwitchEnable { get; } = new SwitchRedirectEnableCommand();

        #region Test

        public FullRedirectTest RedirectTest { get; }

        public static DependencyProperty ErrorInfosProperty =
        DependencyProperty.Register(
            nameof(ErrorInfos), typeof(string[]), typeof(MainPage),
            new PropertyMetadata(Array.Empty<string>()));

        public string[] ErrorInfos
        {
            get => (string[])GetValue(ErrorInfosProperty);
            set => SetValue(ErrorInfosProperty, value);
        }


        public static DependencyProperty HasEditErrorProperty =
            DependencyProperty.Register(
                nameof(HasEditError), typeof(bool), typeof(MainPage),
                new PropertyMetadata(null));

        public bool HasEditError
        {
            get => (bool)GetValue(HasEditErrorProperty);
            set => SetValue(HasEditErrorProperty, value);
        }

        private void OnEditErrorChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (sender is FullRedirectTest redirect)
                OnEditErrorChanged(redirect);
        }

        private void OnEditErrorChanged(FullRedirectTest sender)
        {
            HasEditError = sender.HasErrors;
            ErrorInfos = sender.GetErrors(null).Cast<string>().ToArray();
        }
        #endregion

        public static DependencyProperty SelectedRedirectSettingProperty =
            DependencyProperty.Register(
                nameof(SelectedRedirectSetting), typeof(RedirectSetting), typeof(MainPage),
                new PropertyMetadata(null));

        public RedirectSetting SelectedRedirectSetting
        {
            get => (RedirectSetting)GetValue(SelectedRedirectSettingProperty);
            set => SetValue(SelectedRedirectSettingProperty, value);
        }


        class SwitchRedirectEnableCommand : ICommand
        {
            event EventHandler ICommand.CanExecuteChanged { add { } remove { } }

            public bool CanExecute(object parameter)
                => parameter is RedirectSetting;

            public void Execute(object parameter)
            {
                if (parameter is RedirectSetting setting)
                    setting.Enable = !setting.Enable;
            }

        }

        private void AddRedirect_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RedirectSettingPage), Config.Redirect.Redirects);
        }

        private void OnRedirectEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is RedirectSetting setting)
                Frame.Navigate(typeof(RedirectSettingPage), setting);
        }
        #endregion


        #region AboutThisApp

        private string AppVersion
        {
            get
            {
                try
                {
                    var v = Package.Current.Id.Version;
                    return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
                }
                catch
                {
                    return "?.?.?.?";
                }
            }
        }
        #endregion

        #region x:bind

        static public Visibility NullVisibility(object item) =>
            item == null ? Visibility.Collapsed : Visibility.Visible;

        static public string SelectIf(bool state, string itemT, string itemF) =>
            state ? itemT : itemF;

        static public string SelectTurnOnOff(bool state) =>
            state ? Shared.Properties.Resources.TurnOff : Shared.Properties.Resources.TurnOn;
        #endregion
    }


    class UriWrapper
    {

        public UriWrapper(Uri uri) =>
            Uri = uri;

        public Uri Uri { get; }

    }

}
