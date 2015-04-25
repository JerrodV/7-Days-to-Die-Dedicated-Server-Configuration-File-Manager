using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace WpfStartup.Helpers
{
	public static class Animation
	{
		/// <summary>
		/// This will fade in or out any object that can be cast to a FrameworkElement
		/// </summary>
		/// <param name="elem">The element to fade</param>
		/// <param name="startEnd">Point (start opacity, end opacity)</param>
		/// <param name="fadeSeconds">The duration of the fade</param>
		/// <param name="callback">Optional funciton to call when the fade has completed</param>
		public static void FadeAnimation(FrameworkElement elem, Point startEnd,
			Int16 fadeSeconds, EventHandler callback = null)
		{
			elem.BeginAnimation(Control.OpacityProperty, null);            
			// Create a storyboard to contain the animations.
			Storyboard storyboard = new Storyboard();
			TimeSpan duration = new TimeSpan(0, 0, fadeSeconds);
			// Create a DoubleAnimation to fade
			DoubleAnimation animation = new DoubleAnimation(startEnd.X, startEnd.Y, new Duration(duration));
			// Configure the animation to target the Opacity property 
			Storyboard.SetTargetName(animation, elem.Name);
			Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
			// Add the animation to the storyboard
			storyboard.Children.Add(animation);
			if (callback != null)
			{
				storyboard.Completed += callback;
			}
			// Begin the storyboard
			storyboard.Begin(elem, true);
		}

		/// <summary>
		/// Use this method to break the current animation, yet still have the element visible.
		/// Visibility changes do not effect the current state once an animation is applied.
		/// This is the work around.
		/// This is handy if, when on load, the element is not visible and an animation is applied.
	   /// Then later, you wish to show the object without applying another animation. 
		/// Alternatively,calling: [element].BeginAnimation([DPObject], null) 
		/// will return the element to it's visibility state when it loaded. 
		/// </summary>
		public static AnimationTimeline DefaultOpacityAnimation
		{
			get
			{
				return new DoubleAnimation(1.0, new Duration(new TimeSpan(0, 0, 0)));
			}
		}

	   
	}
}
