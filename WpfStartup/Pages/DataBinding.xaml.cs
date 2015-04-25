using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfStartup.Helpers;
using WpfStartup.Helpers.Validation.ValidationRules;
using WpfStartup.Models;


namespace WpfStartup.Pages
{
	/// <summary>
	/// Interaction logic for DataBinding.xaml
	/// </summary>
	public partial class DataBinding : Page
	{
		Person person = new Person();
		//For simple group validation, use this:

		List<Control> controlCollection = null;

		public DataBinding()
		{
			InitializeComponent();
			//Example load from Database Helper
			person = new Person(Database.GetCommand("GetPeople", null, true));//True for maintain connection because our Person class will use a reader on it and handle the close.
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//For this example, I will bind the ID. However, it will not work. There is no get/set on the ID field.
			//But, when you click the simulated save, we can display it, it is on the object. 
			//We can use this to protect key values and omit fields from datagrids.
            //*Note: Attempting to bind a read only property is a bad idea because it throws a non-fatal exception. To accomplish that
            //use the Binding helper as an example to create a one-way binding for your object.
            Helpers.Bindings.BindInputPropertyToObject(txtBOID, "ID", person, CustomRules.GetRule(ValidationType.Int32), true);			            
			//
            //We could bind but leave it not validated
            //Helpers.Bindings.BindInputPropertyToObject(txtBOID, "ID", person);
            //
            //If you need to use an algorythem to build your expression, you might use the ValidationByRuleSettings
            //Helpers.Bindings.ValidationByRuleSettings rulz = new Bindings.ValidationByRuleSettings();
            //rulz.DataObject = person;
            //rulz.PropertyName = "ID";
            //rulz.Control = txtBOID;
            //Helpers.Bindings.BindInputPropertyToObject(rulz);
            //
            //I favor the following. Note three different methods of providing validation ruls are being used
			Helpers.Bindings.BindInputPropertyToObject(txtBOFN, "FirstName", person, new List<ValidationRule>() { CustomRules.GetRule(0) }, true);

			Helpers.Bindings.BindInputPropertyToObject(txtBOLN, "LastName", person, new List<ValidationRule>() { CustomRules.GetRule(ValidationType.EmptyString) }, true);

			Helpers.Bindings.BindInputPropertyToObject(txtBOPN, "PhoneNumber", person, new List<ValidationType>() { ValidationType.USPhone }, true);

			//This was added to allow for easy access to the controls during validation. It just lets me iterate them as pointers later.
			controlCollection = new List<Control>() { txtBOID, txtBOFN, txtBOLN, txtBOPN };

			//Example of validation without using notification
			//Validation.AddErrorHandler(txtBOPN, HandleValidationError);
            
		}

		private void btnViewO_Click(object sender, RoutedEventArgs e)
		{
			btnViewO.IsEnabled = false;
			//This would normally not be needed. These assignments are just setting textboxes so we can see the binding works. 
			txtVOFN.Text = person.FirstName;
			txtVOLN.Text = person.LastName;
			txtVOPN.Text = person.PhoneNumber;
			txtVOID.Text = person.ID.ToString();
			//

			//The fields are all self validating, however, I have chosen to not force the user to enter a correct value to continue.
			//This means I have the additional reponcibility to control what I do with it when not correct. I believe the intended design would be that
			//validation occures, followed by a save, on close. For this example, I am doing the save on a button click, so the page validation event does not fire.
			//Otherwise, similare code might go it the form closing or page validating event, and the close prevented if not valid.
			if(Validate())
			{
				Helpers.Database.GetCommand("PersonSet", person.GetParameters(), false).ExecuteNonQuery();
				Helpers.MainWindow.ShowNotification("Person Saved: " + System.DateTime.Now.TimeOfDay.ToString());
			}
			else
			{
				Helpers.MainWindow.ShowModal(
				new Helpers.Validation.ValidationError("Not all of the fields are in the correct format to be saved. Check your data and try again."));
			}
			btnViewO.IsEnabled = true;
		}

		/// <summary>
		/// This will the binding control of all of the textboxes
		/// </summary>
		/// <returns>True if valid</returns>
		private Boolean Validate()
		{
			foreach (Control c in controlCollection)
			{
				BindingExpression b = c.GetBindingExpression(TextBox.TextProperty);
				if (b.HasError)
				{
					return false;
				}
			}
			return true;
		}


		//This is an example of how to create your own validation event. Using the ((TextBox)sender)
		//cast, you can get the ID of a specific textbox on the page.
		private void HandleValidationError(object sender, ValidationErrorEventArgs e)
		{
			if (e.OriginalSource is IInputElement)
			{
				if (e.OriginalSource is TextBox)
				{
					BindingExpression b = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
					if (b.HasError)
					{
						Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError(
							b.ValidationError.ErrorContent.ToString() + "\n This was called from the view"));						
					}
				}
			}
			e.Handled = true;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			//this.BindingGroup.NotifyOnValidationError = true;
		}
	}


	/*Keep for now*/
	//public class ValidateDataBindings : ValidationRule
	//{
	//	private DataBinding db;
	//	public ValidateDataBindings(DataBinding db) 
	//	{ this.db = db; }

	//	public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
	//	{
	//		if(value == null){ return new ValidationResult(false, true);}
	//		Boolean valid =
	//		    !db.txtVOFN.GetBindingExpression(TextBox.TextProperty).HasError
	//		    &&
	//		    !db.txtVOLN.GetBindingExpression(TextBox.TextProperty).HasError
	//		    &&
	//		    !db.txtVOPN.GetBindingExpression(TextBox.TextProperty).HasError
	//		    &&
	//		    !db.txtVOID.GetBindingExpression(TextBox.TextProperty).HasError;

	//		if (valid)
	//		{
	//			return new ValidationResult(true, null);
	//		}
	//		else
	//		{
	//			return new ValidationResult(false, null);
	//		}
	//	}
	//}
}
