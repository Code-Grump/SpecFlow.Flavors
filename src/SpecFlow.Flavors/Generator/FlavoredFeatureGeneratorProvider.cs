using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using SpecFlow.Flavors.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.Flavors.Generator
{
    public class FlavoredFeatureGeneratorProvider : IFeatureGeneratorProvider
    {
        private readonly IObjectContainer _container;

        public FlavoredFeatureGeneratorProvider(IObjectContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public bool CanGenerate(SpecFlowDocument document) => true;

        public IFeatureGenerator CreateGenerator(SpecFlowDocument document)
        {
            return new FlavoredFeatureGenerator(null, _container.Resolve<FlavoringConfiguration>());
        }

        public int Priority => PriorityValues.Normal;
    }
}
