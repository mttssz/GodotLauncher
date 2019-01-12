using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace GodotLauncher.Classes
{
    public static class CommonUtils
    {
        public static void PopupExceptionMessage(string title, Exception ex)
        {
            MessageBox.Show(ex.Message, title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
