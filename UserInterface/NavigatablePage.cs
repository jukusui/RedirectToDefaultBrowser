using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Navigation;

namespace UserInterface
{
    public class NavigatablePage : Page
    {

        protected virtual ContentDialog GetAskDialog() => null;

        protected virtual void DialogPrimary() { }
        protected virtual void DialogCancel() { }
        protected virtual void DialogSecondary() { }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= BackRequested;

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility =
                Frame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
            currentView.BackRequested += BackRequested; base.OnNavigatedTo(e);
        }

        protected virtual async void BackRequested(object sender, BackRequestedEventArgs e) =>
            await OnBackRequested(() => e.Handled = true);

        protected async override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse && !e.Handled)
            {
                var point = e.GetCurrentPoint(this);
                if (point.Properties.IsXButton1Pressed)
                {
                    await OnBackRequested(() => e.Handled = true);
                }
            }
        }

        private async Task OnBackRequested(Action onHandled)
        {
            var dialog = GetAskDialog();
            if (dialog != null)
            {
                onHandled();
                var result = await dialog.ShowAsync();
                switch (result)
                {
                    case ContentDialogResult.None:
                        return;
                    case ContentDialogResult.Primary:
                        DialogPrimary();
                        break;
                    case ContentDialogResult.Secondary:
                        DialogSecondary();
                        break;
                }

            }
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

    }

}
