using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SevenDaysConfigUI.Helpers
{
	public class MainWindow
	{
		public static void ShowModal(Page modalView, String title = "", Window owner = null)
		{
			SevenDaysConfigUI.MainWindow w = App.AppMainWindow;
			/*Capture the existing effect in the content window*/
			System.Windows.Media.Effects.Effect tempEffect = w.MainContent.Effect;
			/*Blur the current content*/
			w.MainContent.Effect = new System.Windows.Media.Effects.BlurEffect();

			/*Create the new window*/
			Window w2 = new Window();
			w2.Name = "window_" + Guid.NewGuid().ToString().Replace("-","_");
			LastWindow = w2.Name;
			/*Clear the title*/
			w2.Title = title;
			/*Set border style*/
			w2.WindowStyle = WindowStyle.SingleBorderWindow;
			/*This removes the min/max buttons*/
			w2.ResizeMode = ResizeMode.NoResize;
			/*The window has to be created before I can remove the icon. 
			 * This will attach a handy event for it.*/
			w2.Loaded += ModalWindow_Loaded;
			w2.Closing += ModalWindow_Closing;
			/*Set the content*/
			w2.Content = modalView;
			/*Size the window to the page passed in*/
			w2.SizeToContent = SizeToContent.WidthAndHeight;
			/*Set to open center of the current window*/
			w2.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			/*Declare our owner (This sometimes defaults to the button that sent it. Need to make sure.)*/
			if(owner == null)
			{			
				w2.Owner = w;
			}
			else
			{
				w2.Owner = owner;
			}
			/*Show the window as dialog.*/
			//w.ModalMask.Visibility = Visibility.Visible;
			w2.Effect = new System.Windows.Media.Effects.DropShadowEffect();
			w2.Background = Brushes.DimGray;
			try
			{
				w2.ShowDialog();
			}
			catch (Exception x)
			{
				String s = x.Message;
			}

			/*Return the effect on the content to what it was.*/
			w.MainContent.Effect = tempEffect;

		}

		///
		public static void ModalWindow_Loaded(object sender, RoutedEventArgs e)
		{
			SevenDaysConfigUI.MainWindow.RemoveIcon((Window)sender);
		}

		private static String LastWindow
		{
			get;
			set;
		}

		private static void ModalWindow_Closing(object sender, CancelEventArgs e)
		{
			if (sender is Window)
			{
				Window w = (Window)sender;
				if (w.Name != LastWindow)
				{
					e.Cancel = true;
					LastWindow = w.Name;
				}

			}

			App.AppMainWindow.ModalMask.Visibility = Visibility.Collapsed;
			//w2.DialogResult = false;
			Modal_Closing(sender, e);
		}

		public static void Modal_Closing(object sender, CancelEventArgs e)
		{   
		}

		public static void ShowContent(FrameworkElement element)
		{
			App.AppMainWindow.MainContent.Content = element;
			App.AppMainWindow.MainContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			App.AppMainWindow.MainContent.VerticalContentAlignment = VerticalAlignment.Stretch;
		}

		public static void ShowContent(FrameworkElement element, ContentControl target)
		{
			target.Content = element;
			target.HorizontalContentAlignment = HorizontalAlignment.Center;
			target.VerticalContentAlignment = VerticalAlignment.Center;
		}


		/// <summary>
		/// Places the message in an appropriate container and begins the notification animation
		/// </summary>        
		/// <param name="message">The message to be shown in the notification window</param>
		/// <param name="fadeDuration">The primary fade duration. Note* this does not effect the duration the message is availible semi-transparent</param>
		/// <param name="callback">If you wish to automatically begin the fade out animation, pass null.
		/// If you wish to not begin the animation until they mouse_out of the notification, pass a new EventHandler, otherwise, you can 
		/// create your own event to call when the window has opened. Remeber though, this will break the fade out timer chain and
		/// it will not resume until the mouse out event fires. You can start the fadeout timer yourself by calling App.AppMainWindow.NotificationShown(object sender, EventArgs e)</param>
		///
		public static List<TabItem> NotifictionMessageCollection = new List<TabItem>();
		public static void ShowNotification(String message, Boolean persists = false, EventHandler callback = null)
		{
			//Before we add the message, we need to check our collection to make sure we're not duplicating.
			if (MessageExists(message)) { return; }
			resetNotificationVisibility();
			TextBlock tb = new TextBlock();
			//tb.Height = App.AppMainWindow.NotificationTabs.Height;                        
			tb.TextWrapping = TextWrapping.Wrap;
			tb.Foreground = Brushes.Black;
			tb.Width = 234;
			tb.Text = message;
			TabItem ti = new TabItem();
			ti.Content = tb;
			NotifictionMessageCollection.Add(ti);
			ti.Header = NotifictionMessageCollection.Count.ToString();

			App.AppMainWindow.NotificationTabs.ItemsSource = null;
			App.AppMainWindow.NotificationTabs.ItemsSource = NotifictionMessageCollection;
			App.AppMainWindow.NotificationTabs.SelectedIndex = NotifictionMessageCollection.Count - 1;
			App.AppMainWindow.Notification.Effect = new System.Windows.Media.Effects.DropShadowEffect();
			App.AppMainWindow.Notification.Visibility = Visibility.Visible;
			App.AppMainWindow.Notification.IsExpanded = true;

			if (!persists)
			{
				if (callback == null)
				{
					Helpers.Animation.FadeAnimation(App.AppMainWindow.Notification, new Point(0.0, 1.0), App.FADEDURATION_LONG, App.AppMainWindow.NotificationShown);
				}
				else
				{
					Helpers.Animation.FadeAnimation(App.AppMainWindow.Notification, new Point(0.0, 1.0), App.FADEDURATION_LONG, callback);
				}
			}
		}

		//Reset to show the notifiction and stop all the timers.
		private static void resetNotificationVisibility()
		{
			App.AppMainWindow.Notification.BeginAnimation(Control.OpacityProperty, Helpers.Animation.DefaultOpacityAnimation);
			App.AppMainWindow.Notification.IsExpanded = true;
			App.AppMainWindow.notificationHoldTimer.Stop();
			App.AppMainWindow.notificationHoldTimer.Close();
			App.AppMainWindow.notificationFadeTimer.Stop();
			App.AppMainWindow.notificationFadeTimer.Close();
		}

		/// <summary>
		/// Check to see if the string already exists in the notifications collection
		/// </summary>
		/// <param name="message">String Message to check</param>
		/// <returns></returns>
		private static Boolean MessageExists(String message)
		{

			foreach (TabItem ti in NotifictionMessageCollection)
			{
				if (((TextBlock)ti.Content).Text == message)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Set notification area to not visible and clear the message collection
		/// </summary>
		public static void HideNotification()
		{
			App.AppMainWindow.BeginAnimation(Expander.OpacityProperty, null);
			NotifictionMessageCollection = new List<TabItem>();
		}

		

		//Status Message

		private static System.Timers.Timer StatusMessageTimer = new System.Timers.Timer();
		public static void SetStatus(String message, Double showDuration = 0)
		{
			App.AppMainWindow.StatusText = message;
			if (showDuration > 0)
			{
				StatusMessageTimer = new System.Timers.Timer(showDuration);
				StatusMessageTimer.Elapsed += HideStatusMessage;
				StatusMessageTimer.Start();
			}
		}

		private static void HideStatusMessage(object sender, System.Timers.ElapsedEventArgs e)
		{
			App.AppMainWindow.Dispatcher.Invoke((Action)delegate()
			{
				if (sender is System.Timers.Timer)
				{
					StatusMessageTimer.Stop();
					StatusMessageTimer.Dispose();
				}
				App.AppMainWindow.StatusText = "";
			});
		}

		

	}
}
