using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using UserInterface.PartsUI;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace UserInterface
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainFrame : Frame
    {
        public MainFrame()
        {
            this.InitializeComponent();
            Loaded += MainFrame_Loaded;
        }

        private void MainFrame_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateActiveState(CoreWindow.GetForCurrentThread().ActivationMode != CoreWindowActivationMode.Deactivated);
            Window.Current.Activated += Current_Activated;
            TitleBar = TitleBar.GetCurrent();
        }

        private void UpdateActiveState(bool isActive) =>
            VisualStateManager.GoToState(this,
                isActive ?
                "WindowFocused" :
                "WindowNotFocused", false);

        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
            => UpdateActiveState(e.WindowActivationState != Windows.UI.Core.CoreWindowActivationState.Deactivated);

        public static DependencyProperty TitleBarProperty =
        DependencyProperty.Register(
            nameof(TitleBar), typeof(TitleBar), typeof(MainFrame),
            new PropertyMetadata(null));

        public TitleBar TitleBar
        {
            get => (TitleBar)GetValue(TitleBarProperty);
            private set => SetValue(TitleBarProperty, value);
        }
    }
}
