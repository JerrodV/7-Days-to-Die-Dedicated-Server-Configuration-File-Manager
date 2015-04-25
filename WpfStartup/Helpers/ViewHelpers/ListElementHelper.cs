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

namespace WpfStartup.Helpers.ViewHelpers
{
    class ListElementHelper
    {
	   /// <summary>
	    /// Locates the selected item and returns its contents as a string.  Used for simple listboxes with text items. 
	   /// </summary>
	   /// <param name="lb">The listbox to check</param>
	   /// <returns>The currenetly selected text</returns>
        public static String GetSelectText(ListBox lb)
        {
            foreach (ListBoxItem l in lb.Items)
            {
                if (l.IsSelected)
                {
                    return l.Content.ToString();
                }
            }
            return "";
        }

	   /// <summary>
	   /// Searches a listbox for the indicated string and gives the item focus. Used for simple listboxes with text items. 
	   /// </summary>
	   /// <param name="lb">The listbox in which to locate the control to select</param>
	   /// <param name="value">The text to locate within the ListBox.Items collection</param>
	   /// <returns>Int32 position of selected item in the array</returns>
	   public static Int32 SetSelectedText(ListBox lb, String value)
        {
            for(int i = 0; i < lb.Items.Count;i++)
            {
                if (((ListBoxItem)lb.Items[i]).Content.ToString() == value)
                {
				 ((ListBoxItem)lb.Items[i]).Focus();
				 return i;
                }
            }
		  return -1;
        }

	   /// <summary>
	   /// Attempts to move the selected item in a ListBox to the next item in the list. If the end of the list is reached
	   /// the current selection will be kept.
	   /// </summary>
	   /// <param name="lb">The ListBox in which to increment the selected ID</param>
	   /// <param name="wrap">If wrap is true, and the end of the list is reached, it will return to index 0. 
	   /// Otherwise, it will stay at the end of the list.</param>
        public static void SelectNextItem(ListBox lb, Boolean wrap = false)
        {
            if (lb.SelectedIndex < lb.Items.Count)
            {
                lb.SelectedIndex++;
            }
		  else if(lb.SelectedIndex == lb.Items.Count && wrap)
		  {
			lb.SelectedIndex = 0;
		  }

        }
	   
	   /// <summary>
	   /// Attempts to move the selected item in a ListBox to the previous item in the list. If the end of the list is reached
	   /// the current selection will be kept.
	   /// </summary>
	   /// <param name="lb"></param>
	   /// <param name="wrap"></param>
        public static void SelectPreviousItem(ListBox lb, Boolean wrap)
        {
            if (lb.SelectedIndex > 0)
            {
                lb.SelectedIndex--;
            }
		  else if(lb.SelectedIndex == 0)
		  {
			lb.SelectedIndex = lb.Items.Count;
		  }
        }

	   
	   #region Visual Tree Helpers

	   #region Sources
	   /*
	    *I have not produced a demo for this and also did not write the code. They are pretty specific in that
	    *the object in question has to be hidden in the logical code files, but exist in the markup. 
	    *
	    * I found a need for FindVisualChild when looking for a way to manipulate the vertical offset
	    * of a scrollbar. The scrollbar 'wraps' the control it functions and for some odd reason, does not appear
	    * as a sub object of the control. This allowed me to 'Walk the visual tree' down to the scrollbar control
	    * and manipulate it on a pointer.
	    * 
		  ScrollViewer sv = Helpers.ViewHelpers.ListElementHelper.FindVisualChild<ScrollViewer>(lbRank);
		  sv.ScrollChanged += HandleRankScrollChange;
		  currentVerticalOffset = sv.VerticalOffset;
	    *  
	    * While originally I took FindVisualChild from StackOverflow. I did not comment the source at the time and had to re-reference it.
	    * It appears to have originated on MSDN.	    * 
	    * FindVisualChild : 
	    * http://msdn.microsoft.com/en-us/library/bb613579.aspx
	    * http://stackoverflow.com/questions/980120/finding-control-within-wpf-itemscontrol	    * 
	    * 
	    * 
	    * This is the converse of the method above. It 'Walks the visual tree up' looking for the defined type and returns that object.
	    * In this example, I use it to return the entire row from a datadrid, when I allow the user to select by cell. e would also cast to DataGridCell.
	    * 
	    * DataGridRow listViewItem = Helpers.ViewHelpers.ListElementHelper.FindAnchestor<DataGridRow>((DependencyObject)e.OriginalSource);
	    * 
	    * And I got FindAnchestor on Christian Mosers WPF Tutorial.net
	    * http://wpftutorial.net/DragAndDrop.html
	    */
	   #endregion

	   /// <summary>
	   /// Used to locate a child within within a DependancyObject. Useful when it's needed to address a property on a sub-object when the 
	   /// sub-object is not exposed in logical files. i.e. ScrollBars
	   /// </summary>
	   /// <typeparam name="childItem">The type of the object to locate</typeparam>
	   /// <param name="obj">current object</param>
	   /// <returns>DependencyObject child object as T</returns>
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        /// <summary>
	   /// Helper to search up the VisualTree
	   /// Uses T to search up the visual tree until it locates its parent.
        /// </summary>
        /// <typeparam name="T">The type(class name) of the object to locate</typeparam>
	   /// <param name="current">The current DependencyObject to begin searching from</param>
        /// <returns>The parent object as type T</returns>
        public static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
	   #endregion

    }
}
