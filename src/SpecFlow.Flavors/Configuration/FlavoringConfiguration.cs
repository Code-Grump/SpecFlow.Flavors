using System;
using System.Collections.Generic;
using System.Text;

namespace SpecFlow.Flavors.Configuration
{
    public class FlavoringConfiguration
    {
        public ICollection<string> FlavourTagPrefixes { get; } = new List<string> {"Flavor", "Flavour"};
    }
}
