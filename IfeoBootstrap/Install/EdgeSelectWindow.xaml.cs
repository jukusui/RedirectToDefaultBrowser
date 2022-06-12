using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Globalization;

namespace IfeoBootstrap.Install;
/// <summary>
/// EdgeSelectWindow.xaml の相互作用ロジック
/// </summary>
public partial class EdgeSelectWindow : Window
{
    public EdgeSelectWindow()
    {
        InitializeComponent();
        Apps.CollectionChanged +=
            (s, e) => CheckError();
        CheckError();
    }



    public ObservableCollection<AppRegistry> Apps { get; } = new();

    public IEnumerable<AppRegistry> SelectedItems { get => listBox.SelectedItems.Cast<AppRegistry>(); }

    private static readonly DependencyPropertyKey MessagePropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(Message), typeof(string), typeof(EdgeSelectWindow), null);
    public static readonly DependencyProperty MessageProperty = MessagePropertyKey.DependencyProperty;
    public string? Message { get => (string)GetValue(MessageProperty); private set => SetValue(MessagePropertyKey, value); }

    private static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly(
        nameof(HasError), typeof(bool), typeof(EdgeSelectWindow), null);
    public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;
    public bool HasError { get => (bool)GetValue(HasErrorProperty); private set => SetValue(HasErrorPropertyKey, value); }

    public void SelectAll()
    {
        foreach (var item in Apps)
            listBox.SelectedItems.Add(item);
    }

    private void CheckError()
    {
        HasError = listBox.SelectedItems.Count <= 0;
        if (Apps.Count == 0)
            Message = "Edgeが検出されませんでした";
        else
            Message = HasError ? "1個以上の項目を選択してください" : null;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        => CheckError();

    private void OnOkClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }


    private void OnLoad(object sender, RoutedEventArgs e)
    {
        var helper = new System.Windows.Interop.WindowInteropHelper(this);
        Win32API.WindowApi.RemoveMenuItem(helper.Handle, Win32API.WindowApi.SystemCommand.CS_CLOSE, false);
    }

}

public class OpenExplorer : ICommand
{
#pragma warning disable 0067
    public event EventHandler? CanExecuteChanged;
#pragma warning restore 0067

    public bool CanExecute(object parameter)
        => File.Exists(parameter?.ToString() ?? "");

    public void Execute(object parameter)
    {
        Process.Start("explorer.exe", $"/select,\"{parameter}\"").Dispose();
    }
}

public class InvertConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isOn = false;
        if (value is bool boolean)
        {
            isOn = boolean;
        }
        else if (value is Visibility visibility)
        {
            isOn = visibility == Visibility.Visible;
        }
        if (targetType == typeof(Visibility))
            return isOn ? Visibility.Collapsed : Visibility.Visible;
        else
            return !isOn;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(value, targetType, parameter, culture);
    }
}
