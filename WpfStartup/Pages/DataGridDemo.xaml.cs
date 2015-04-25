using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfStartup.Models;

namespace WpfStartup.Pages
{
	/// <summary>
	/// Interaction logic for DataGridDemo.xaml
	/// </summary>
	public partial class DataGridDemo : Page
	{
		//Binding objects
		public People _people; 
		private People people
		{
			get
			{
				if (_people == null)
				{
					return new People();
				}
				else
				{
					return _people;
				}
			}
			set{
				this._people = value;
			}
		}
		
		private Person _currentPerson;
		public Person currentPerson 
		{
			get
			{
				if (_currentPerson == null)
				{
					return new Person();
				}
				else
				{
					return _currentPerson;
				}
			}
			set
			{	
				this._currentPerson = value;
			}
		}

		/*There is little co comment on here. This is why I like WPF. It is easy to set these things up freeing up time to make sweet grids instead.*/

		public DataGridDemo(People people = null)
		{
			InitializeComponent();
			
			if(people != null)
			{
				this.people = people;
			}
			//Pretty obvious...
		}

		private void DataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			LoadDataGrid();
			//I'll bet that load a data grid..
			BindVals();
			//I'll bet that binds values..
		}	

		public void LoadDataGrid()
		{
			//Loading datagrids it pretty easy in WPF..
			dgDemo.ItemsSource = null;//<-- Good practice. Some people complain about grids not updating right and doing this to remove old data is the fix. I just always do it now out of habbit.
			dgDemo.ItemsSource = this.people;
		}
		
		private void BindVals()
		{
			//Here are some examples of binding using the helper.
			//Notice the first example is not using the helper or validation, but the other two are. 
			//This was not corrected when I added validation to the binding process.
			//But stands as a pretty good example of how it helped shorten the syntax as 
			//it would have taken several more lines and a new class for a validation rule
			//to do it without it. It still works for the example.
			Binding bFN = new Binding("FirstName");
			bFN.Source = currentPerson;
			txtFName.SetBinding(TextBox.TextProperty, bFN);
			//OR
			Helpers.Bindings.BindInputPropertyToObject(txtLName, "LastName", currentPerson, Helpers.Validation.ValidationRules.ValidationType.EmptyString);
			Helpers.Bindings.BindInputPropertyToObject(txtPhone, "PhoneNumber", currentPerson, Helpers.Validation.ValidationRules.ValidationType.USPhone);
		}	

		//This grabs the current seletion from the grid and places it into our currentPerson field.
		//This is part of why I like WPF. Very easy.
		private void dgDemo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{            
			currentPerson = dgDemo.SelectedItem as Person;
			BindVals();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//And a one liner to save the current person to the database.
			Helpers.Database.GetCommand("Person_Set", currentPerson.GetParameters());
		}
	}
}
