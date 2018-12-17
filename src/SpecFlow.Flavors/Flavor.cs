using System;
using System.Collections.Generic;
using System.Text;

namespace SpecFlow.Flavors
{
    public class Flavor
    {
        public Flavor(string category, string value)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Category { get; }

        public string Value { get; }
    }
}
