using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace WpfStartup.Helpers.Validation.ValidationRules
{
	/// <summary>
	/// This is used for easy access to the availible Validation Types 
	/// </summary>
	public enum ValidationType
	{
		EmptyString = 0,
		DateTime = 1,
		USPhone = 2,
		Int16 = 3,
		Int32 = 4,
		Int64 = 5	
		/*Add Validation Enum Here*/	
	}

	/// <summary>
	/// Returns an organized list of the availible Validation Rules
	/// </summary>
	public class CustomRulesList:List<ValidationRule>
	{
		public static List<ValidationRule> RulesList
		{
			get
			{
				List<ValidationRule> vrList = new List<ValidationRule>();
				vrList.Add(new EmptyString());
				vrList.Add(new DateTime());
				vrList.Add(new USPhone());
				vrList.Add(new Integer16());
				vrList.Add(new Integer32());
				vrList.Add(new Integer64());
				/*Add Custom Rules Here*/
				return vrList;
			}
		}
	}

	/// <summary>
	/// The Rules function is only to return a new instance of the desired type.
	/// It is a singular class, pluralized to be more logical in code.
	/// This can only be accomplished by inheriting ValidationRule(singular)
	/// </summary>
	public class CustomRules : ValidationRule 
	{
		public static ValidationRule GetRule(ValidationType type)
		{	
			switch(type)
			{
				case ValidationType.EmptyString:
				return new EmptyString();
				case ValidationType.DateTime:
				return new DateTime();
				case ValidationType.USPhone:
				return new USPhone();
				case ValidationType.Int16:
				return new Integer16();
				case ValidationType.Int32:
				return new Integer32();
				case ValidationType.Int64:
				return new Integer64();
				default: return null;
			}
		}

		public static ValidationRule GetRule(Int32 index)
		{
			switch (index)
			{
				case 0:
					return new EmptyString();
				case 1:
					return new DateTime();
				case 2:
					return new USPhone();
				case 3:
					return new Integer16();
				case 4:
					return new Integer32();
				case 5:
					return new Integer64();
				default: return null;
			}
		}

		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			return new ValidationResult(true, null);
		}
	}


	/*Add Custom Classes Here*/

	public class EmptyString : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			if (value.ToString() != "")
			{
				return new ValidationResult(true, null);
			}
			else
			{
				return new ValidationResult(false, "A value must be entered.");
			}
		}
	}

	public class DateTime : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			System.DateTime testVal = new System.DateTime();
			if (System.DateTime.TryParse(value.ToString(), out testVal))
			{
				return new ValidationResult(true, null);
			}
			else
			{
				return new ValidationResult(false, "A value must be entered.");
			}
		}
	}

	public class USPhone : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			String pattern = @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$";
			Regex regEx = new Regex(pattern);
			if (regEx.IsMatch(value.ToString()))
			{
				return new ValidationResult(true, null);
			}
			else
			{
				return new ValidationResult(false, "Value '" + value + "' is not a valid phone number. \n\rUse format: (123)456-7890");
			}
		}
	}

	public class Integer16 : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			Int16 testVal = new Int32();
			if (Int16.TryParse(value.ToString(), out testVal))
			{
				return new ValidationResult(true, null);
			}
			else
			{
				return new ValidationResult(false, "Value '" + value + "' is not a number.");
			}
		}
	}

	public class Integer32 : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			Int32 testVal = new Int32();
			if (Int32.TryParse(value.ToString(), out testVal))
			{
				return new ValidationResult(true, null);
			}
			else
			{
				return new ValidationResult(false, "Value '" + value + "' is not a number.");
			}
		}
	}

	public class Integer64 : ValidationRule
	{
		public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
		{
			Int64 testVal = new Int64();
			if (Int64.TryParse(value.ToString(), out testVal))
			{
				return new ValidationResult(true, null);
			}
			else
			{
				return new ValidationResult(false, "Value '" + value + "' is not a number.");
			}
		}
	}

}
