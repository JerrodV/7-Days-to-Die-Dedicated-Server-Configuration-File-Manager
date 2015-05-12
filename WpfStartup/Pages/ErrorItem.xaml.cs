using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SevenDaysConfigUI.Pages
{
    /// <summary>
    /// Interaction logic for ErrorItem.xaml
    /// </summary>
    public partial class ErrorItem : Page
    {
        public Exception data;
        public FrameworkElement Content;
        public ErrorItem(Exception data)
        {
            InitializeComponent();
            this.data = data;
            this.txtMessage.Text = data.Message !=null ? data.Message : "";
            this.txtInnerException.Text = data.InnerException != null ? data.InnerException.ToString() : "";
            this.txtStackTrace.Text = data.StackTrace != null ? data.StackTrace : "";
            this.Content = pnlLayout;
        }
    }
}
