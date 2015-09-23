using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GCodeSender.NET
{
	class FloatValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			string s = value as string;
			float f;

			if(!float.TryParse(s, out f) || f <= 0.0)
			{
				return new ValidationResult(false, "Not a valid Increment");
			}

			return ValidationResult.ValidResult;
		}
	}
}
