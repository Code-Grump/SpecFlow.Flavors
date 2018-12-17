using System;
using BoDi;
using SpecFlow.Flavors;
using SpecFlow.Flavors.Generator;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly:RuntimePlugin(typeof(FlavorsPlugin))]
[assembly:GeneratorPlugin(typeof(FlavorsPlugin))]

namespace SpecFlow.Flavors
{
	public class FlavorsPlugin : IRuntimePlugin, IGeneratorPlugin
	{
		public void Initialize(
			RuntimePluginEvents runtimePluginEvents,
			RuntimePluginParameters runtimePluginParameters,
			UnitTestProviderConfiguration unitTestProviderConfiguration)
		{
			runtimePluginEvents.CustomizeScenarioDependencies += (sender, args) => RegisterRuntimeDependencies(args.ObjectContainer);
		}

		private void RegisterRuntimeDependencies(ObjectContainer container)
		{
			
		}

		public void Initialize(
			GeneratorPluginEvents generatorPluginEvents,
			GeneratorPluginParameters generatorPluginParameters,
			UnitTestProviderConfiguration unitTestProviderConfiguration)
		{
			generatorPluginEvents.CustomizeDependencies += (sender, args) => CustomizeGeneratorDependencies(args.ObjectContainer);
		}

		private void CustomizeGeneratorDependencies(ObjectContainer container)
		{
            container.RegisterTypeAs<FlavoredFeatureGeneratorProvider, IFeatureGeneratorProvider>();
		}
	}
}
