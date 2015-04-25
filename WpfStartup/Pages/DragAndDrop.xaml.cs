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

namespace WpfStartup.Pages
{
    /// <summary>
    /// Interaction logic for DragDrop.xaml
    /// </summary>
    public partial class DragAndDrop : Page
    {
        //Just a collection of stuff to play with. It does not have to be a key value pair. In fact, it can be any object.
        //The DisplayMemberPath will determine the field that is used when displaying a value is needed.
        private List<KeyValuePair<Int32, String>> Months;

        public DragAndDrop()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {           
            Months = new List<KeyValuePair<Int32, String>>() 
            { 
                new KeyValuePair<Int32, String>(1,"January"),
                new KeyValuePair<Int32, String>(2,"February"),
                new KeyValuePair<Int32, String>(3,"March"),
                new KeyValuePair<Int32, String>(4,"April"),
                new KeyValuePair<Int32, String>(5,"May"),
                new KeyValuePair<Int32, String>(6,"June"),
                new KeyValuePair<Int32, String>(7,"July"),
                new KeyValuePair<Int32, String>(8,"August"),
                new KeyValuePair<Int32, String>(9,"September"),
                new KeyValuePair<Int32, String>(10,"October"),
                new KeyValuePair<Int32, String>(11,"November"),
                new KeyValuePair<Int32, String>(12,"December")
            };

            //For the first two, I set the collection to the moth, but change the display member.
            lb1.ItemsSource = Months;
            lb1.DisplayMemberPath = "Value";

            lb2.ItemsSource = Months;
            lb2.DisplayMemberPath = "Key";
            
            //For these two, they are intended to receive values, though you can also drag items one they are populated. 
            //They do not get a item source.
            lb3.DisplayMemberPath = "Value";
            
            lb4.DisplayMemberPath = "Key";
        }

        private void lb_MouseMove(object sender, MouseEventArgs e)
        {   
            //In many demonstrations, in addition to this check, you would also check how far the mouse had moved since the left button was clicked.
            //This prevents the object from being copied into memory just because a list item was clicked. For this example, the object is very small and the additional
            //complexity of adding the additional check we deemed unnecessary. However, if you were dragging a larger object, or running process that made false drags 
            // an issue, you may want to consider that tactice. There are plenty of examples on the net if you need an example.
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox lb = sender as ListBox;
                List<KeyValuePair<Int32, String>> obj =
                    lb.SelectedItems.OfType<KeyValuePair<Int32, String>>().ToList();
                DataObject theDO = new DataObject();
                theDO.SetData("theFormat", obj);
                if (obj != null)
                {
                    DragDrop.DoDragDrop(lb, theDO, DragDropEffects.Copy);
                }
            }
        }

        //This toggles the mouse pointer to show a drop of type copy is allowed
        private void lb_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        //This performs out drop logic
        private void lb_Drop(object sender, DragEventArgs e)
        {
            //Is this data we can take as a drop?
            if (e.Data.GetDataPresent("theFormat"))
            {
                //It was, so get the data by the format, parse it, and poof, we have data.
                List<KeyValuePair<Int32, String>> obj = e.Data.GetData("theFormat") as List<KeyValuePair<Int32, String>>;
                
                //Create a pointer to listbox that the drop event occured on
                ListBox lb = sender as ListBox;
                //Add each of the items in the drag collection to the list.
                foreach(KeyValuePair<Int32, String> kvp in obj)
                {
                    lb.Items.Add(kvp);
                }
                //Turn of the drop cursor.
                e.Effects = DragDropEffects.None;
            }
        }

    }
}
