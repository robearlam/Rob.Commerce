using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Pipelines;

namespace Feature.Initialization.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Habitat.InitializeEnvironmentPricingBlock")]
    public class InitializeEnvironmentPricingBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IAddPriceBookPipeline _addPriceBookPipeline;
        private readonly IAssociateCatalogToBookPipeline _associateCatalogToBookPipeline;

        public InitializeEnvironmentPricingBlock(IAddPriceBookPipeline addPriceBookPipeline, IAssociateCatalogToBookPipeline associateCatalogToBookPipeline)
        {
            _addPriceBookPipeline = addPriceBookPipeline;
            _associateCatalogToBookPipeline = associateCatalogToBookPipeline;
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            const string artifactSet = "Environment.Habitat.Pricing-1.0";

            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
                return arg;

            context.Logger.LogInformation($"{Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");

            try
            {
                var currencySetId = context.GetPolicy<GlobalCurrencyPolicy>().DefaultCurrencySet;
                var addPriceBookArgument = new AddPriceBookArgument("Habitat_PriceBook")
                {
                    ParentBook = string.Empty,
                    Description = "Habitat price book",
                    DisplayName = "Habitat",
                    CurrencySetId = currencySetId
                };

                var book = await _addPriceBookPipeline.Run(addPriceBookArgument, context);
                await _associateCatalogToBookPipeline.Run(new CatalogAndBookArgument(book.Name, "Habitat_Master"), context);
            }
            catch (Exception ex)
            {
                context.CommerceContext.LogException(Name, ex);
            }

            return arg;
        }
    }
}
