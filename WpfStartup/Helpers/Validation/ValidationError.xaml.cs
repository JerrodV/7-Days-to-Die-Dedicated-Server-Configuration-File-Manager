using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfStartup.Helpers.Validation
{
	/// <summary>
	/// Interaction logic for ValidationError.xaml
	/// </summary>
	public partial class ValidationError : Page
	{
		private String m;

		public ValidationError(String m)
		{
			InitializeComponent();
			this.m = m;
		}

		private void Page_Loaded_1(object sender, RoutedEventArgs e)
		{
			Message.Text = m;
			System.Media.SystemSounds.Asterisk.Play();
		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{
			Window.GetWindow(this).Close();
		}
	}
}
