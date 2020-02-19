using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UserInterface.PartsUI
{
    public class TitleBar : DependencyObject
    {

        public TitleBar()
        {
            try
            {
                var titleBar = CoreApplication.GetCurrentView().TitleBar;
                CoreTitleBar = titleBar;
                AppTitleBar = ApplicationView.GetForCurrentView()?.TitleBar;
                TitleBarLayoutUpdate();
                titleBar.LayoutMetricsChanged += OnUpdate;
                titleBar.IsVisibleChanged += OnUpdate;
            }
            catch
            {
                Height = 32;
            }
        }

        private void OnUpdate(CoreApplicationViewTitleBar sender, object args) =>
            TitleBarLayoutUpdate();

        private void TitleBarLayoutUpdate()
        {
            if (CoreTitleBar == null)
                return;
            if (CoreTitleBar.IsVisible)
            {
                Height = CoreTitleBar.Height;
                InsetMargin = new Thickness(
                    CoreTitleBar.SystemOverlayLeftInset, 0,
                    CoreTitleBar.SystemOverlayRightInset, 0);
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
                Height = 0;
                InsetMargin = new Thickness(0, 0, 0, 0);
            }
        }

        #region Layouts

        public static DependencyProperty HeightProperty =
        DependencyProperty.Register(
            nameof(Height), typeof(double), typeof(TitleBar),
            new PropertyMetadata(0.0));

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            private set => SetValue(HeightProperty, value);
        }


        public static DependencyProperty InsetMarginProperty =
        DependencyProperty.Register(
            nameof(InsetMargin), typeof(Thickness), typeof(TitleBar),
            new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        public Thickness InsetMargin
        {
            get => (Thickness)GetValue(InsetMarginProperty);
            set => SetValue(InsetMarginProperty, value);
        }


        public static DependencyProperty VisibilityProperty =
        DependencyProperty.Register(
            nameof(Visibility), typeof(Visibility), typeof(TitleBar),
            new PropertyMetadata(Visibility.Visible));

        public Visibility Visibility
        {
            get => (Visibility)GetValue(VisibilityProperty);
            set => SetValue(VisibilityProperty, value);
        }

        #endregion

        #region Colors

        private static PropertyChangedCallback
            GenCC(Action<ApplicationViewTitleBar, Color> setter)
            => new PropertyChangedCallback(
                (s, e) => setter((s as TitleBar).AppTitleBar, (Color)e.NewValue));

        public static DependencyProperty ForegroundProperty =
        DependencyProperty.Register(
            nameof(Foreground), typeof(Color), typeof(TitleBar),
            new PropertyMetadata(null, GenCC((t, v) => t.ForegroundColor = v)));

        public Color Foreground
        {
            get => (Color)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }


        public static DependencyProperty BackgroundProperty =
        DependencyProperty.Register(
            nameof(Background), typeof(Color), typeof(TitleBar),
            new PropertyMetadata(null, GenCC((t, v) => t.BackgroundColor = v)));

        public Color Background
        {
            get => (Color)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static DependencyProperty ButtonForegroundProperty =
            DependencyProperty.Register(
                nameof(ButtonForeground), typeof(Color), typeof(TitleBar),
                new PropertyMetadata(null, GenCC((t, v) => t.ButtonForegroundColor = v)));

        public Color ButtonForeground
        {
            get => (Color)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value);
        }

        public static DependencyProperty ButtonBackgroundProperty =
        DependencyProperty.Register(
            nameof(ButtonBackground), typeof(Color), typeof(TitleBar),
            new PropertyMetadata(null, GenCC((t, v) => t.ButtonBackgroundColor = v)));

        public Color ButtonBackground
        {
            get => (Color)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }


        public static DependencyProperty ButtonHoverForegroundProperty =
            DependencyProperty.Register(
                nameof(ButtonHoverForeground), typeof(Color), typeof(TitleBar),
                new PropertyMetadata(null, GenCC((t, v) => t.ButtonHoverForegroundColor = v)));

        public Color ButtonHoverForeground
        {
            get => (Color)GetValue(ButtonHoverForegroundProperty);
            set => SetValue(ButtonHoverForegroundProperty, value);
        }

        public static DependencyProperty ButtonHoverBackgroundProperty =
        DependencyProperty.Register(
            nameof(ButtonHoverBackground), typeof(Color), typeof(TitleBar),
            new PropertyMetadata(null, GenCC((t, v) => t.ButtonHoverBackgroundColor = v)));

        public Color ButtonHoverBackground
        {
            get => (Color)GetValue(ButtonHoverBackgroundProperty);
            set => SetValue(ButtonHoverBackgroundProperty, value);
        }


        public static DependencyProperty ButtonPressedForegroundProperty =
            DependencyProperty.Register(
                nameof(ButtonPressedForeground), typeof(Color), typeof(TitleBar),
                new PropertyMetadata(null, GenCC((t, v) => t.ButtonPressedForegroundColor = v)));

        public Color ButtonPressedForeground
        {
            get => (Color)GetValue(ButtonPressedForegroundProperty);
            set => SetValue(ButtonPressedForegroundProperty, value);
        }

        public static DependencyProperty ButtonPressedBackgroundProperty =
        DependencyProperty.Register(
            nameof(ButtonPressedBackground), typeof(Color), typeof(TitleBar),
            new PropertyMetadata(null, GenCC((t, v) => t.ButtonPressedBackgroundColor = v)));

        public Color ButtonPressedBackground
        {
            get => (Color)GetValue(ButtonPressedBackgroundProperty);
            set => SetValue(ButtonPressedBackgroundProperty, value);
        }


        public static DependencyProperty InactiveForegroundProperty =
    DependencyProperty.Register(
        nameof(InactiveForeground), typeof(Color), typeof(TitleBar),
        new PropertyMetadata(null, GenCC((t, v) => t.InactiveForegroundColor = v)));

        public Color InactiveForeground
        {
            get => (Color)GetValue(InactiveForegroundProperty);
            set => SetValue(InactiveForegroundProperty, value);
        }

        public static DependencyProperty InactiveBackgroundProperty =
        DependencyProperty.Register(
            nameof(InactiveBackground), typeof(Color), typeof(TitleBar),
            new PropertyMetadata(null, GenCC((t, v) => t.InactiveBackgroundColor = v)));

        public Color InactiveBackground
        {
            get => (Color)GetValue(InactiveBackgroundProperty);
            set => SetValue(InactiveBackgroundProperty, value);
        }


        public static DependencyProperty ButtonInactiveForegroundProperty =
            DependencyProperty.Register(
                nameof(ButtonInactiveForeground), typeof(Color), typeof(TitleBar),
                new PropertyMetadata(null, GenCC((t, v) => t.ButtonInactiveForegroundColor = v)));

        public Color ButtonInactiveForeground
        {
            get => (Color)GetValue(ButtonInactiveForegroundProperty);
            set => SetValue(ButtonInactiveForegroundProperty, value);
        }

        public static DependencyProperty ButtonInactiveBackgroundProperty =
            DependencyProperty.Register(
                nameof(ButtonInactiveBackground), typeof(Color), typeof(TitleBar),
                new PropertyMetadata(null, GenCC((t, v) => t.ButtonInactiveBackgroundColor = v)));

        public Color ButtonInactiveBackground
        {
            get => (Color)GetValue(ButtonInactiveBackgroundProperty);
            set => SetValue(ButtonInactiveBackgroundProperty, value);
        }

        #endregion


        public CoreApplicationViewTitleBar? CoreTitleBar { get; }
        public ApplicationViewTitleBar? AppTitleBar { get; }
    }
}
