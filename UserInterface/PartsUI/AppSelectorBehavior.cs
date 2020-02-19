using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace UserInterface.PartsUI
{
    public static class AppSelectorBehavior
    {


        public static DependencyProperty DestUriProperty =
        DependencyProperty.Register(
            "DestUri", typeof(Uri), typeof(AppSelectorBehavior),
            new PropertyMetadata(null, OnDestUriChanged));

        public static void SetDestUri(DependencyObject d, Uri? value) =>
            d.SetValue(DestUriProperty, value);

        public static Uri? GetDestUri(DependencyObject d) =>
            d.GetValue(DestUriProperty) as Uri;

        private static void OnDestUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Hyperlink hyperlink)
            {
                if (e.OldValue != null)
                    hyperlink.Click -= Hyperlink_Click;
                if (e.NewValue != null)
                    hyperlink.Click += Hyperlink_Click;
            }
        }



        public static DependencyProperty SelectModeProperty =
            DependencyProperty.RegisterAttached(
                "SelectMode", typeof(bool), typeof(AppSelectorBehavior),
                new PropertyMetadata(false));

        public static void SetSelectMode(DependencyObject d, bool value) =>
            d.SetValue(SelectModeProperty, value);

        public static bool GetSelectMode(DependencyObject d) =>
            d.GetValue(SelectModeProperty) as bool? ?? false;


        private static async void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            try
            {
                var uri = GetDestUri(sender);
                if (uri == null)
                    return;
                var option = new LauncherOptions();
                if (GetSelectMode(sender))
                    option.DisplayApplicationPicker = true;

                await Launcher.LaunchUriAsync(uri, option);
            }
            catch (Exception ex)
            {
                await new ExceptionDialog(ex).ShowAsync();
            }
        }
    }
}
