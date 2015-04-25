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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfStartup.Helpers
{
	/// <summary>
	/// Interaction logic for DefaultErrorMessage.xaml
	/// </summary>
	public partial class DefaultErrorMessage : Page
	{
		private String message;
		private Exception ex;
		public DefaultErrorMessage(String message, Exception ex)
		{			
			InitializeComponent();
			this.message = message;
			this.ex = ex;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			txtDefaultMessage.Text = message;
			txtExemptionMessage.Text = "Error: \n";
			txtExemptionMessage.Text += ex.Message + "\n\n";
			txtExemptionMessage.Text += "Stack Trace: \n";
			txtExemptionMessage.Text += ex.StackTrace.ToString() + "\n\n";
			txtExemptionMessage.Text += "Inner Exception: \n";
			txtExemptionMessage.Text += ex.InnerException.ToString() + "\n\n";
		}
	}
}
