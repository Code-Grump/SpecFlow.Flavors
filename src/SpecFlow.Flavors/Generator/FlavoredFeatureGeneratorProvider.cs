using System;
using BoDi;
using SpecFlow.Flavors.Configuration;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.Flavors.Generator
{
    public class FlavoredFeatureGeneratorProvider : IFeatureGeneratorProvider
    {
        private readonly IObjectContainer _container;

        private readonly UnitTestFeatureGeneratorProvider _baseProvider;

        public FlavoredFeatureGeneratorProvider(IObjectContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));

            _baseProvider = new UnitTestFeatureGeneratorProvider(
                new UnitTestFeatureGenerator(
                    _container.Resolve<IUnitTestGeneratorProvider>(),
                    _container.Resolve<CodeDomHelper>(),
                    _container.Resolve<SpecFlowConfiguration>(),
                    _container.Resolve<IDecoratorRegistry>()));
        }

        public bool CanGenerate(SpecFlowDocument document) => _baseProvider.CanGenerate(document);

        public IFeatureGenerator CreateGenerator(SpecFlowDocument document)
        {
            var baseGenerator = _baseProvider.CreateGenerator(document);

            return new FlavoredFeatureGenerator(baseGenerator, _container.Resolve<FlavoringConfiguration>());
        }

        public int Priority => PriorityValues.Normal;
    }
}
