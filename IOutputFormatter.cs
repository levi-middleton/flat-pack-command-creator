using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatPackCommandCreator
{
	public interface IOutputFormatter
	{
		string Format(string input);
	}
}
