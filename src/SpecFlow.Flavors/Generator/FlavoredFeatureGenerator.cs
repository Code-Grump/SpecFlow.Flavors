using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using SpecFlow.Flavors.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.Flavors.Generator
{
    public class FlavoredFeatureGenerator : IFeatureGenerator
    {
        private readonly IFeatureGenerator _featureGenerator;

        private readonly FlavoringConfiguration _configuration;

        public FlavoredFeatureGenerator(IFeatureGenerator featureGenerator, FlavoringConfiguration configuration)
        {
            _featureGenerator = featureGenerator ?? throw new ArgumentNullException(nameof(featureGenerator));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public CodeNamespace GenerateUnitTestFixture(
            SpecFlowDocument specFlowDocument,
            string testClassName,
            string targetNamespace)
        {
            var specFlowFeature = specFlowDocument.SpecFlowFeature;

            var flavors = GetFlavors(specFlowFeature.Tags).ToList();

            // If the feature doesn't have any flavors, just invoke the feature generator.
            if (!flavors.Any())
            {
                return _featureGenerator.GenerateUnitTestFixture(specFlowDocument, testClassName, targetNamespace);
            }
            
            // Create the range of flavor combinations.
            var combinations = CreateFlavorCombinations(flavors);

            var generatorResults = new List<CodeNamespace>();

            foreach (var combination in combinations)
            {
                var clonedDocument = CreateFlavoredFeature(specFlowDocument, combination);

                string previousFormat = null;
                if (_featureGenerator is UnitTestFeatureGenerator unitTestFeatureGenerator)
                {
                    previousFormat = unitTestFeatureGenerator.TestclassNameFormat;
                    var extension = "_With" + string.Join(
                        "_AndWith",
                        combination.Select(combo => combo.Category + combo.Value));

                    unitTestFeatureGenerator.TestclassNameFormat += extension;
                }

                // Generate the flavored test fixture.
                var generatorResult = _featureGenerator.GenerateUnitTestFixture(clonedDocument, testClassName, targetNamespace);

                generatorResults.Add(generatorResult);

                if (previousFormat != null)
                {
                    ((UnitTestFeatureGenerator) _featureGenerator).TestclassNameFormat = previousFormat;
                }
            }

            var result = generatorResults.First();

            foreach (var generatorResult in generatorResults)
            {
                foreach (CodeTypeDeclaration type in generatorResult.Types)
                {
                    result.Types.Add(type);
                }
            }

            return result;
        }

        private static IEnumerable<List<Flavor>> CreateFlavorCombinations(IEnumerable<Flavor> flavors)
        {
            // Sort the flavors into buckets.
            var buckets = flavors
                .GroupBy(flav => flav.Category)
                .ToList();

            if (buckets.Count == 1)
            {
                return new [] { buckets[0].ToList() };
            }

            // Perform a cross-join over all the buckets to get all permutations.
            var result = buckets.Aggregate(
                Enumerable.Empty<IEnumerable<Flavor>>(),
                (current, bucket) => current.SelectMany(flavs => bucket.Select(flav => flavs.Concat(Singleton(flav)))));

            return result.Select(combo => combo.ToList());
        }

        private static IEnumerable<T> Singleton<T>(T item)
        {
            yield return item;
        }

        private SpecFlowDocument CreateFlavoredFeature(SpecFlowDocument specFlowDocument, IEnumerable<Flavor> flavors)
        {
            var sourceFeature = specFlowDocument.SpecFlowFeature;

            // Strip off all the flavor tags we no longer want.
            var tags = sourceFeature.Tags
                .Where(tag => !IsFlavorTag(tag))
                .ToList();

            // Add the flavors specified.
            foreach (var flavor in flavors)
            {
                tags.Add(new Tag(null, $"{flavor.Category}: {flavor.Value}"));
            }

            // Generate a copy of the feature with the tags specified.
            // TODO: Try filtering out all scenarios which don't match.
            var flavoredFeature = new SpecFlowFeature(
                tags.ToArray(),
                sourceFeature.Location,
                sourceFeature.Language,
                sourceFeature.Keyword,
                sourceFeature.Name,
                sourceFeature.Description,
                sourceFeature.Children.ToArray());

            return new SpecFlowDocument(
                flavoredFeature,
                specFlowDocument.Comments.ToArray(),
                specFlowDocument.SourceFilePath);
        }

        private bool IsFlavorTag(Tag tag)
        {
            var parts = tag.Name.Split(new[] {':'}, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return false;
            }

            var prefix = parts[0].Trim();

            return _configuration.FlavourTagPrefixes
                .Any(pre => string.Equals(prefix, pre, StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<Flavor> GetFlavors(IEnumerable<Tag> tags)
        {
            return tags
                .Select(tag => tag.Name.Split(new[] {':'}, 2, StringSplitOptions.RemoveEmptyEntries))
                .Where(parts => parts.Length == 2)
                .Select(parts => (Prefix: parts[0].Trim(), Value: parts[1].Trim()))
                .Where(parts => parts.Value != "" && _configuration.FlavourTagPrefixes
                    .Any(prefix => string.Equals(prefix, parts.Prefix, StringComparison.OrdinalIgnoreCase)))
                .Select(parts => new Flavor(parts.Prefix, parts.Value));
        }
    }
}
