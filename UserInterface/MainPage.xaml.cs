using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UserInterface.PartsUI;
using Windows.ApplicationModel.Core;
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
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UserInterface
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.Activated += Current_Activated;
        }

        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            VisualStateManager.GoToState(this,
                e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated ?
                WindowNotFocused.Name :
                WindowFocused.Name, false);
        }

        public static DependencyProperty TitleBarProperty =
        DependencyProperty.Register(
            nameof(TitleBar), typeof(TitleBar), typeof(MainPage),
            new PropertyMetadata(null));

        public TitleBar TitleBar
        {
            get => (TitleBar)GetValue(TitleBarProperty);
            private set => SetValue(TitleBarProperty, value);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TitleBar = TitleBar.GetCurrent();
            base.OnNavigatedTo(e);
        }


        //最近使ったファイル(JumpList)
        //https://docs.microsoft.com/ja-jp/windows/uwp/files/how-to-track-recently-used-files-and-folders
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var str = Shared.Config.LastUrl;
            Debug.WriteLine(str);
            var jumpList = await Windows.UI.StartScreen.JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = Windows.UI.StartScreen.JumpListSystemGroupKind.Recent;
            foreach (var item in jumpList.Items)
            {
                Debug.WriteLine(item.Description);
            }
        }
    }
}
