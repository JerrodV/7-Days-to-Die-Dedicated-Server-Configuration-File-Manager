using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/*EDIT WITH CAUTION - THIS POWERS ALL OF THE NOTIFICATION FUNCTIONS*/

namespace SevenDaysConfigUI
{

	/// <summary>
	/// This class contains the events that control the fade ins/outs of the notificaton window. 
	/// 
	/// It is finicky and easy to break. Edit with caution.
	/// 
	/// It also includes the get and set for the status text. This will always pull from 
	/// this.Status.Items[0]. If you wish to add something else to t
	/// </summary>
    public partial class MainWindow 
    {
        const int NOTIFICATION_HOLD_TIMERINTERVAL = 4000;
        public System.Timers.Timer notificationHoldTimer = new System.Timers.Timer(NOTIFICATION_HOLD_TIMERINTERVAL);

        const int NOTIFICATION_FADE_TIMERINTERVAL = 20000;
        public System.Timers.Timer notificationFadeTimer = new System.Timers.Timer(NOTIFICATION_FADE_TIMERINTERVAL);

        /// <summary>
        ///Gets or sets the current Content of 
        ///MainWindow.Status.Items[0], which is defaulted to a Label.
        ///This makes the Content a string, but it could be any renderable object.
        /// </summary>
        public String StatusText
        {
            get 
            {
                return ((Label)this.Status.Items[0]).Content.ToString();
            }

            set 
            {
                Label l = this.Status.Items[0] as Label;
                l.Content = value; 
            }
        }
	   
        public void NotificationShown(object sender, EventArgs e)
        {
            if (Notification.IsExpanded)
            {
                Notification.BeginAnimation(Control.OpacityProperty, Helpers.Animation.DefaultOpacityAnimation);
                Notification.IsExpanded = true;            
                notificationHoldTimer.Stop();
                notificationHoldTimer.Close();
                notificationFadeTimer.Stop();
                notificationFadeTimer.Close();

                notificationHoldTimer = new System.Timers.Timer(NOTIFICATION_HOLD_TIMERINTERVAL);
                notificationHoldTimer.Enabled = true;
                notificationHoldTimer.Elapsed += FadeOut;
                notificationHoldTimer.Start();
            }           
        }

        public void FadeOut(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                Notification.BeginAnimation(Control.OpacityProperty, null);
                Notification.IsExpanded = false;
                notificationHoldTimer.Stop();
                notificationHoldTimer.Dispose();
                Helpers.Animation.FadeAnimation(Notification, new Point(1.0, 0.2), 3, FadeComplete);
            });
        }

        public void FinishFadeOut(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                Helpers.Animation.FadeAnimation(Notification, new Point(0.2, 0.0), 2, FadeComplete);
            });
        }

        private void FadeComplete(object sender, EventArgs e)
        {   
            this.Dispatcher.Invoke((Action)delegate()
            {
                if (notificationFadeTimer.Enabled)
                {                    
                    notificationFadeTimer.Stop();
                    notificationFadeTimer.Dispose();
                    Notification.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    notificationFadeTimer = new System.Timers.Timer(NOTIFICATION_FADE_TIMERINTERVAL);
                    notificationFadeTimer.Elapsed += FinishFadeOut;
                    notificationFadeTimer.Start();
                }
                Helpers.MainWindow.NotifictionMessageCollection = new System.Collections.Generic.List<TabItem>();

            });
        }

        private void Notification_MouseEnter(object sender, MouseEventArgs e)
        {
            Notification.BeginAnimation(Control.OpacityProperty, Helpers.Animation.DefaultOpacityAnimation);
            Notification.IsExpanded = true;            
            if (notificationHoldTimer.Enabled)
            {
                notificationHoldTimer.Stop();
                notificationHoldTimer.Dispose();
            }
            if (notificationFadeTimer.Enabled)
            {
                notificationFadeTimer.Stop();
                notificationFadeTimer.Dispose();
            }
            e.Handled = true;
        }

        private void Notification_MouseLeave(object sender, MouseEventArgs e)
        {
            NotificationShown(sender, new EventArgs());
            e.Handled = true;
        }
    }
}
