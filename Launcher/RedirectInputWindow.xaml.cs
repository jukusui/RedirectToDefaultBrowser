using System.Collections.Generic;
using System.Windows;

namespace Launcher
{
    /// <summary>
    /// RedirectInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class RedirectInputWindow : Window
    {
        private void Init()
        {
            Setting.Input = TargetSetting.Input;
            Setting.Output = TargetSetting.Output;
            InitializeComponent();
            TitleInput.Text = TargetSetting.Title;
        }

        public RedirectInputWindow(RedirectSetting targetSetting)
        {
            TargetSetting = targetSetting;
            IsEdit = true;
            Init();
        }

        public RedirectInputWindow(IList<RedirectSetting> targetList)
        {
            TargetList = targetList;
            TargetSetting = new RedirectSetting();
            IsEdit = false;
            Init();
        }

        public static DependencyProperty IsEditProperty =
        DependencyProperty.Register(
            nameof(IsEdit), typeof(bool), typeof(RedirectInputWindow),
            new PropertyMetadata(false));

        public bool IsEdit
        {
            private get => (bool)GetValue(IsEditProperty);
            set => SetValue(IsEditProperty, value);
        }


        public RedirectTest Setting { get; set; } = new RedirectTest()
        { Test = "https://www.bing.com/search?q=Example" };



        public RedirectSetting TargetSetting { get; }
        public IList<RedirectSetting> TargetList { get; }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnEnter(object sender, RoutedEventArgs e)
        {
            TargetSetting.Input = Setting.Input;
            TargetSetting.Output = Setting.Output;
            TargetSetting.Title = TitleInput.Text;
            if (!IsEdit)
                TargetList.Add(TargetSetting);
            Close();
        }
    }
}
