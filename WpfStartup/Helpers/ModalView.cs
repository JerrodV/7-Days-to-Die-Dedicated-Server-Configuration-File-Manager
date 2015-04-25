using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfStartup.Helpers
{
	public class WindowView
	{
		protected Page _modalView;
		public Page View
		{
			get;
			set;
		}

		
		public String Title
		{
			get;
			set;
		}

		public Window Owner
		{
			get;
			set;
		}

        //This is the primary overload contsructor for creating a new modal.
		public WindowView(){}
		public WindowView(Page modalView, Window owner, String title = "")
		{
			this.View = modalView;
			this.Title = title;
			this.Owner = owner;			
		}

        //These allow the developer to override what happens when the window closes.
        //Note:These are called in addition to the actions the window normally performs.
		public CancelEventHandler WindowClosing;
		public RoutedEventHandler WindowLoaded;

        //Some people might go for the shorter syntax.
		public void Show(){ShowWindow();}

        //
		private void ShowWindow()
		{			
			if(this.View == null){this.View = new Page();}
			/*Create the new window*/
			Window w2 = new Window();			
			w2.Name = "window_" + Guid.NewGuid().ToString().Replace("-", "_");
			/*Clear the title*/
			w2.Title = this.Title;
			/*Set border style*/
			w2.WindowStyle = WindowStyle.SingleBorderWindow;
			/*This removes the min/max buttons*/
			w2.ResizeMode = ResizeMode.NoResize;						
			/*Set the content*/
			w2.Content = this.View;
			/*Size the window to the page passed in*/
			
			w2.SizeToContent = SizeToContent.WidthAndHeight;

			w2.Loaded += Window_Loaded;
			w2.Closing += Window_Closing;

			/*Set to open center of the current window*/
			w2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			/*Declare our owner (This sometimes defaults to the button that sent it. Need to make sure.)*/
			w2.Owner = this.Owner;
			w2.Effect = new System.Windows.Media.Effects.DropShadowEffect();
			w2.Background = Brushes.DimGray;
			w2.ShowInTaskbar = false;
			App.AppMainWindow.ModalMask.Visibility = Visibility.Visible;
			w2.Show();
			/*Return the effect on the content to what it was.*/
			//App.AppMainWindow.MainContent.Effect = tempEffect;
		}

		/// <summary>
		/// Handling the Load of modal view window to remove icon
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WpfStartup.MainWindow.RemoveIcon((Window)sender);
			if (WindowLoaded != null)
			{
				WindowLoaded(sender, e);
			}
			e.Handled = true;
		}

        /// <summary>
        /// Handles the close of the modal view window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			App.AppMainWindow.ModalMask.Visibility = Visibility.Collapsed;
			if (WindowClosing != null)
			{
				WindowClosing(sender, e);
			}
		}
	}
}
