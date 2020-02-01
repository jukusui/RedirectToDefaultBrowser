using System;
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
            titleBar.LayoutMetricsChanged += (s, e) => TitleBarLayoutUpdate();
        }

        private TitleBar(double height)
        {
            Height = height;
        }

        private void TitleBarLayoutUpdate()
        {
            Height = TitleBarView.Height;
        }

        private static Dictionary<CoreApplicationViewTitleBar, TitleBar> Dict = new Dictionary<CoreApplicationViewTitleBar, TitleBar>();


        public static DependencyProperty HeightProperty =
        DependencyProperty.Register(
            nameof(Height), typeof(double), typeof(TitleBar),
            new PropertyMetadata(0.0));

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            private set => SetValue(HeightProperty, value);
        }


        public CoreApplicationViewTitleBar TitleBarView { get; }
    }
}
