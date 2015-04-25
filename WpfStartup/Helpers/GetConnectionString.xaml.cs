using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace WpfStartup.Helpers
{
	/// <summary>
	/// Interaction logic for GetConnectionString.xaml
	/// </summary>
	public partial class GetConnectionString : Page
	{
		//Hopefully something like this can actually be usable for you, thought you might want to consider your security needs before doing so. 
		//The database class assumes a string will be in the user config file for this app.
		//If that string is missing, This modal can be used to obtain the connection string from the user. This could also be used at first launch. 
		
		//Mostly it demonstates how some of the helpers can be used to alter layout by removing unnecessary fields when an checkbox is selected.
		//This removes the the user name and password fields if the user selects integrated security.

		public String ConnectionString
		{
			get
			{
				SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
				
				if (txtServer.Text  != "")
				b.DataSource = txtServer.Text;
				
				if (txtCatalog.Text != "")
				b.InitialCatalog = txtCatalog.Text;
				
				if(cbUWA.IsChecked.HasValue)
				b.IntegratedSecurity = cbUWA.IsChecked.Value;
				
				if (txtUser.Text != "")
				b.UserID = txtUser.Text;
				
				if (txtPass.Password != "")
				b.Password = txtPass.Password;

				return b.ConnectionString;
			}
		}
		public GetConnectionString()
		{
			InitializeComponent();
		}

		private void cbUWA_Click(object sender, RoutedEventArgs e)
		{
			CheckBox cb = ((CheckBox)sender);

			if (cb.IsChecked.HasValue && cb.IsChecked.Value == true)
			{
				/*Disable Controls*/
				//Note that the Point takes doubles. You could only partially fade a control.
				//The last parameter is the length of time in whole seconds.
				txtUser.IsEnabled = false;
				txtUser.Text = "";
				Helpers.Animation.FadeAnimation(txtUser, new System.Windows.Point(1, 0), 1);

				txtPass.IsEnabled = false;
				txtPass.Password = "";
				Helpers.Animation.FadeAnimation(txtPass, new System.Windows.Point(1, 0), 1);
			}
			else
			{
				/*Enable Controls*/
				Helpers.Animation.FadeAnimation(txtUser, new System.Windows.Point(0, 1), 1);
				txtUser.IsEnabled = true;

				Helpers.Animation.FadeAnimation(txtPass, new System.Windows.Point(0, 1), 1);
				txtPass.IsEnabled = true;
			}
		}

		private void btnTest_Click(object sender, RoutedEventArgs e)
		{			
			try
			{
				SqlConnection conn = new SqlConnection(ConnectionString);
				conn.Open();
				//There is a chance that we open the connection, but then it is immediatly closed by the server.
				//If this happens, it should fail when close is called, but I was not sure so I coded in this ..thing.
				Boolean wasOpen = conn.State == System.Data.ConnectionState.Open;
				conn.Close();
				if(wasOpen)
				{
					Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError("Connection Successful."), "", Window.GetWindow(this));
				}
				else
				{
					Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError("The connection was not refused but could not be opened."), "", Window.GetWindow(this));					
				}
			}
			catch(Exception ex)
			{
				Helpers.MainWindow.ShowModal(new Helpers.DefaultErrorMessage("A Connection Error Has Occured:", ex), "", Window.GetWindow(this));				
			}			
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Window.GetWindow(this).Close();
			e.Handled = true;
		}
		
		private void txtServer_GotFocus(object sender, RoutedEventArgs e)
		{
			txtServer.SelectAll();
		}

		private void txtCatalog_GotFocus(object sender, RoutedEventArgs e)
		{
			txtCatalog.SelectAll();
		}

		private void txtUser_GotFocus(object sender, RoutedEventArgs e)
		{
			txtUser.SelectAll();
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			Properties.Settings.Default.Base_ConnectionString = ConnectionString;
			Properties.Settings.Default.Save();
			Window.GetWindow(this).Close();
		}

	}
}
