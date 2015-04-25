using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfStartup.Helpers.Validation.ValidationRules;

namespace WpfStartup.Helpers
{		
	public class Bindings
	{
	#region Validation By Rules List
		//TODO:Need Summary
		public struct ValidationByRulesListSettings
		{
			public Control Control;
			public String PropertyName;
			public Object DataObject;
			public List<ValidationRule> ValidationRules;
			public Boolean UseNotifiction;
		}

        public static void BindInputPropertyToObject(Control control, String propertyName, Object dataObject)
        {
            if (control is IInputElement && propertyName != "" && dataObject != null)
            {
                Binding b = new Binding(propertyName);
                b.NotifyOnValidationError = true;
                b.Source = dataObject;
                //Watch... In theory, any control we will bind to will have this, otherwise it posesses a datasource property
                control.SetBinding(TextBox.TextProperty, b);
            }
        }

		//TODO:Need Summary
		public static void BindInputPropertyToObject(ValidationByRulesListSettings settings)
		{
			BindInputPropertyToObject(settings.Control, settings.PropertyName, settings.DataObject, settings.ValidationRules, settings.UseNotifiction);
		}
		/// <summary>
		/// Takes a control and binds it to the property name of the the object passed in. 
		/// Optionally, you can include validation, and toggle the built in notification. 
		/// </summary>
		/// <param name="control">The System.Windows.Controls.Control to bind to.</param>
		/// <param name="propertyName">The name of the property on the object passed it that yuo wish to bind to.</param>
		/// <param name="dataObject">The object contining the property name to bind to.</param>
		/// <param name="validationRules">A List<Windows.System.Controls.ValidationRule>to apply to this bindings error event</param>
		/// <param name="useNotification">true/false Use Main Window notification</param>
		public static void BindInputPropertyToObject(Control control, String propertyName, Object dataObject, List<ValidationRule> validationRules = null, Boolean useNotification = false)
		{
			if (control is IInputElement && propertyName != "" && dataObject != null)
			{
				Binding b = new Binding(propertyName);
				b.NotifyOnValidationError = true;
				b.Source = dataObject;
				if (validationRules != null)
				{
					foreach (ValidationRule vr in validationRules)
					{
						b.ValidationRules.Add(vr);
					}
				}

				if (useNotification)
				{
					System.Windows.Controls.Validation.AddErrorHandler(control, HandleValidationError);
				}
				//Watch... In theory, any control we will bind to will have this, otherwise it posesses a datasource property
				//The exception to that would be checkboxes.
				control.SetBinding(TextBox.TextProperty, b);
			}
		}
	#endregion

	#region Validation By Types List
		/// <summary>
		/// Takes a control and binds it to the property name of the the object passed in. 
		/// Optionally, you can include validation, and toggle the built in notification. 
		/// </summary>
		/// <param name="control">The System.Windows.Controls.Control to bind to.</param>
		/// <param name="propertyName">The name of the property on the object passed it that yuo wish to bind to.</param>
		/// <param name="dataObject">The object contining the property name to bind to.</param>
		/// <param name="validationRules">A List<WpfStartup.Helpers.Validation.ValidationType>to apply to this bindings error event</param>
		/// <param name="useNotification">true/false Use Main Window notification</param>
		public struct ValidationByTypesListSettings
		{
			public Control Control;
			public String PropertyName;
			public Object DataObject;
			public List<ValidationType> ValidationTypes;
			public Boolean UseNotifiction;
		}
		//TODO:Need Summary
		public static void BindInputPropertyToObject(ValidationByTypesListSettings settings)
		{
			BindInputPropertyToObject(settings.Control, settings.PropertyName, settings.DataObject, settings.ValidationTypes, settings.UseNotifiction);
		}
		/// <summary>
		/// Takes a control and binds it to the property name of the the object passed in. 
		/// Optionally, you can include validation, and toggle the built in notification. 
		/// </summary>
		/// <param name="control">The System.Windows.Controls.Control to bind to.</param>
		/// <param name="propertyName">The name of the property on the object passed it that yuo wish to bind to.</param>
		/// <param name="dataObject">The object contining the property name to bind to.</param>
		/// <param name="validationRules">A Windows.System.Controls.ValidationRule to apply to this bindings error event</param>
		/// <param name="useNotification">true/false Use Main Window notification</param>
		public static void BindInputPropertyToObject(Control control, String propertyName, Object dataObject, List<ValidationType> types, Boolean useNotification = false)
		{
			if (control is IInputElement && propertyName != "" && dataObject != null)
			{
				Binding b = new Binding(propertyName);
				b.NotifyOnValidationError = true;
				b.Source = dataObject;
				foreach (ValidationType t in types)
				{
					b.ValidationRules.Add(CustomRules.GetRule(t));
				}
				if (useNotification)
				{
					System.Windows.Controls.Validation.AddErrorHandler(control, HandleValidationError);
				}

				control.SetBinding(TextBox.TextProperty, b);
			}
		}
	#endregion
	
	#region Validation By Rule
		public struct ValidationByRuleSettings
		{
			public Control Control;
			public String PropertyName;
			public Object DataObject;
			public ValidationRule ValidationRule;
			public Boolean UseNotifiction;
		}
		public static void BindInputPropertyToObject(ValidationByRuleSettings settings)
		{
			BindInputPropertyToObject(settings.Control, settings.PropertyName, settings.DataObject, settings.ValidationRule, settings.UseNotifiction);
		}
		public static void BindInputPropertyToObject(Control control, String propertyName, Object dataObject, ValidationRule validationRule = null, Boolean useNotification = false)
		{
			if (control is IInputElement && propertyName != "" && dataObject != null)
			{
				Binding b = new Binding(propertyName);
				b.NotifyOnValidationError = true;
				b.Source = dataObject;
				if (validationRule != null)
				{
					b.ValidationRules.Add(validationRule);
				}

				if (useNotification)
				{
					System.Windows.Controls.Validation.AddErrorHandler(control, HandleValidationError);
				}

				control.SetBinding(TextBox.TextProperty, b);
			}
		}
	#endregion

	#region Validation By Type
		//TODO:Need Summary
		public struct ValidationByTypeSettings
		{
			public Control Control;
			public String PropertyName;
			public Object DataObject;
			public ValidationRule ValidationType;
			public Boolean UseNotifiction;
		}
		//TODO:Need Summary
		public static void BindInputPropertyToObject(ValidationByTypeSettings settings)
		{
			BindInputPropertyToObject(settings.Control, settings.PropertyName, settings.DataObject, settings.ValidationType, settings.UseNotifiction);
		}
		/// <summary>
		/// Takes a control and binds it to the property name of the the object passed in. 
		/// Optionally, you can include validation, and toggle the built in notification. 
		/// </summary>
		/// <param name="control">The System.Windows.Controls.Control to bind to.</param>
		/// <param name="propertyName">The name of the property on the object passed it that yuo wish to bind to.</param>
		/// <param name="dataObject">The object contining the property name to bind to.</param>
		/// <param name="validationRules">A WpfStartup.Helpers.Validation.ValidationType to apply to this bindings error event</param>
		/// <param name="useNotification">true/false Use Main Window notification</param>
		public static void BindInputPropertyToObject(Control control, String propertyName, Object dataObject, ValidationType type, Boolean useNotification = false)
		{
			if (control is IInputElement && propertyName != "" && dataObject != null)
			{				
				Binding b = new Binding(propertyName);
				b.NotifyOnValidationError = true;
				b.NotifyOnSourceUpdated = true;				
				b.Source = dataObject;

				b.ValidationRules.Add(CustomRules.GetRule(type));


				if (useNotification)
				{
					System.Windows.Controls.Validation.AddErrorHandler(control, HandleValidationError);
				}

				control.SetBinding(TextBox.TextProperty, b);
			}
		}
	#endregion

		/// <summary>
		/// Binds a checkbox to a property of an object
		/// </summary>
		/// <param name="control">The System.Windows.Controls.Control to bind to.</param>
		/// <param name="propertyName">The name of the property on the object passed it that yuo wish to bind to.</param>
		/// <param name="dataObject">The object contining the property name to bind to.</param>        
		/// <param name="useNotification">true/false Use Main Window notification</param>
		public static void BindCheckbox(Control control, String propertyName, Object dataObject, Boolean useNotification = false)
		{            
			Binding b = new Binding(propertyName);
			b.NotifyOnValidationError = true;
			b.Source = dataObject;
			if (useNotification)
			{
				System.Windows.Controls.Validation.AddErrorHandler(control, HandleValidationError);
			}
			control.SetBinding(CheckBox.IsCheckedProperty, b);
		}

		//TODO:Need Summary			
		public static void HandleValidationError(object sender, ValidationErrorEventArgs e)
		{
			if (e.OriginalSource is IInputElement)
			{
				if (e.OriginalSource is TextBox)
				{
					BindingExpression b = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
					if (b.HasError)
					{
						Helpers.MainWindow.ShowModal(new Helpers.Validation.ValidationError(
							b.ValidationError.ErrorContent.ToString()));
					}
				}
			}
			e.Handled = true;
		}
	}
}
