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
    /// Interaction logic for ErrorViewer.xaml
    /// </summary>
    public partial class ErrorViewer : Page
    {
        public List<Exception> errors;
        public ErrorViewer(List<Exception> errors)
        {
            InitializeComponent();
            this.errors = errors;

            foreach(Exception x in errors)
            {
                AccordionItem ai = new AccordionItem();
                ai.Header = x.Message;
                ai.Content = (new ErrorItem(x)).Content;
                accErrorContainer.Items.Add(ai);
                ai.Margin = new Thickness(0.0d);
            }
        }
    }
}
