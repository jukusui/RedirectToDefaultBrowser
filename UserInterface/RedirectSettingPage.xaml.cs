using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace UserInterface
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class RedirectSettingPage : NavigatablePage
    {
        public RedirectSettingPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            EditingSetting = new RedirectTest()
            {
                Test = "https://www.bing.com/search?q=Example"
            };
            var param = Target = e.Parameter;
            if (param is IList<RedirectSetting> list)
            {
                IsAddMode = true;
            }
            else if (param is RedirectSetting setting)
            {
                IsAddMode = false;
                EditingSetting.Input = setting.Input;
                EditingSetting.Output = setting.Output;
                TitleInput.Text = setting.Title;
            }
        }


        public static DependencyProperty IsAddModeProperty =
        DependencyProperty.Register(
            nameof(IsAddMode), typeof(bool), typeof(RedirectSettingPage),
            new PropertyMetadata(false));

        public bool IsAddMode
        {
            get => (bool)GetValue(IsAddModeProperty);
            set => SetValue(IsAddModeProperty, value);
        }

        public object Target { get; private set; }

        public static DependencyProperty EditingSettingProperty =
        DependencyProperty.Register(
            nameof(EditingSetting), typeof(RedirectTest), typeof(RedirectSettingPage),
            new PropertyMetadata(null, (d, e) => (d as RedirectSettingPage)?.OnEditingChanged(e)));

        public RedirectTest EditingSetting
        {
            get => (RedirectTest)GetValue(EditingSettingProperty);
            set => SetValue(EditingSettingProperty, value);
        }


        public static DependencyProperty ErrorInfosProperty =
        DependencyProperty.Register(
            nameof(ErrorInfos), typeof(string[]), typeof(RedirectSettingPage),
            new PropertyMetadata(Array.Empty<string>()));

        public string[] ErrorInfos
        {
            get => (string[])GetValue(ErrorInfosProperty);
            set => SetValue(ErrorInfosProperty, value);
        }


        public static DependencyProperty HasEditErrorProperty =
            DependencyProperty.Register(
                nameof(HasEditError), typeof(bool), typeof(RedirectSettingPage),
                new PropertyMetadata(null));

        public bool HasEditError
        {
            get => (bool)GetValue(HasEditErrorProperty);
            set => SetValue(HasEditErrorProperty, value);
        }


        private void OnEditingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is RedirectTest oldRedirect)
                oldRedirect.ErrorsChanged -= OnEditErrorChanged;
            if (e.NewValue != null && e.NewValue is RedirectTest newRedirect)
            {
                OnEditErrorChanged(newRedirect);
                newRedirect.ErrorsChanged += OnEditErrorChanged;
            }
        }

        private void OnEditErrorChanged(object sender, DataErrorsChangedEventArgs e)
        {
            if (sender is RedirectTest redirect)
                OnEditErrorChanged(redirect);
        }

        private void OnEditErrorChanged(RedirectTest sender)
        {
            HasEditError = sender.HasErrors;
            ErrorInfos = sender.GetErrors(null).Cast<string>().ToArray();
        }

        private void Save() {
            RedirectSetting targetSetting;
            if (Target is RedirectSetting target)
                targetSetting = target;
            else
                targetSetting = new RedirectSetting();
            targetSetting.Title = TitleInput.Text;
            targetSetting.Input = EditingSetting.Input;
            targetSetting.Output = EditingSetting.Output;
            if (IsAddMode && Target is IList<RedirectSetting> targetList)
                targetList.Add(targetSetting);
        }

        private void Back()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        protected override ContentDialog GetAskDialog() =>
            EditingSetting.HasErrors ?
            MoveRedirectDialog :
            SaveRedirectDialog;

        protected override void DialogPrimary() => Save();

        protected override void DialogSecondary() => Back();


        #region x:bind

        static public Visibility NullVisibility(object item) =>
            item == null ? Visibility.Collapsed : Visibility.Visible;

        static public string SelectIf(bool state, string itemT, string itemF) =>
            state ? itemT : itemF;

        static public string SelectTurnOnOff(bool state) =>
            state ? Shared.Properties.Resources.TurnOff : Shared.Properties.Resources.TurnOn;
        #endregion

        private void OnCancel_Click(object sender, RoutedEventArgs e) =>
            Back();

        private void OnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
            Back();
        }
    }
}
