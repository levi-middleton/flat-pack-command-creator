using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FlatPackCommandCreator
{
	public class RadioButtonConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;
			
			return parameterString.Equals(value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string parameterString = parameter as string;
			if (parameterString == null)
				return DependencyProperty.UnsetValue;

			return parameterString;
		}
	}
}
