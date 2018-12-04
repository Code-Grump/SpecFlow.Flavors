using System;
using System.Collections.Generic;
using System.Text;

namespace SpecFlow.Flavors
{
	public class FlavorDefinition
	{
		public ICollection<string> TagAssociations { get; } = new List<string>();
	}
}
