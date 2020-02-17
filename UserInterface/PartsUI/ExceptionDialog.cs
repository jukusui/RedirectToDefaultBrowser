using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace UserInterface.PartsUI
{
    public class ExceptionDialog : ContentDialog
    {
        public ExceptionDialog(Exception exception)
        {
            Title = Shared.Properties.Resources.ExceptionOccurred;
            Content = exception.Message;
            CloseButtonText = Shared.Properties.Resources.Close;
        }
    }
}
