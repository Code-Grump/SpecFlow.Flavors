using System;
using System.Collections.Generic;
using System.Text;

namespace SpecFlow.Flavors
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class FlavorAttribute : Attribute
	{
		public FlavorAttribute(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(name));
			}

			Name = name;
		}

		public string Name { get; }
	}
}
