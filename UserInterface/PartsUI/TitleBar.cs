﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UserInterface.PartsUI
{
    public class TitleBar : DependencyObject
    {

        public static TitleBar GetCurrent()
        {
            try
            {
                var titleBar = CoreApplication.GetCurrentView().TitleBar;
                if (Dict.TryGetValue(titleBar, out TitleBar res))
                    return res;

                return Dict[titleBar] = new TitleBar(titleBar);
            }
            catch
            {
                return new TitleBar(32);
            }
        }

        private TitleBar(CoreApplicationViewTitleBar titleBar)
        {
            TitleBarView = titleBar;
            TitleBarLayoutUpdate();
            titleBar.LayoutMetricsChanged += OnUpdate;
            titleBar.IsVisibleChanged += OnUpdate;
        }

        private void OnUpdate(CoreApplicationViewTitleBar sender, object args) =>
            TitleBarLayoutUpdate();

        private TitleBar(double height)
        {
            Height = height;
        }

        private void TitleBarLayoutUpdate()
        {
            if (TitleBarView == null)
                return;
            if (TitleBarView.IsVisible)
            {
                Height = TitleBarView.Height;
                InsetMargin = new Thickness(
                    TitleBarView.SystemOverlayLeftInset, 0,
                    TitleBarView.SystemOverlayRightInset, 0);
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
                Height = 0;
                InsetMargin = new Thickness(0, 0, 0, 0);
            }
        }

        private static readonly Dictionary<CoreApplicationViewTitleBar, TitleBar> Dict =
            new Dictionary<CoreApplicationViewTitleBar, TitleBar>();


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



        public CoreApplicationViewTitleBar? TitleBarView { get; }
    }
}
