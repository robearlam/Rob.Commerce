using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Promotions;
using Sitecore.Framework.Pipelines;

namespace Feature.Initialization.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Habitat.InitializeEnvironmentPromotionsBlock")]
    public class InitializeEnvironmentPromotionsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IAddPromotionBookPipeline _addBookPipeline;
        private readonly IAssociateCatalogToBookPipeline _associateCatalogToBookPipeline;

        public InitializeEnvironmentPromotionsBlock(IAddPromotionBookPipeline addBookPipeline, IAssociateCatalogToBookPipeline associateCatalogToBookPipeline)
        {
            _addBookPipeline = addBookPipeline;
            _associateCatalogToBookPipeline = associateCatalogToBookPipeline;
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            const string artifactSet = "Environment.Habitat.Promotions-1.0";

            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
                return arg;

            context.Logger.LogInformation($"{Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");
            var addPromotionBookArgument = new AddPromotionBookArgument("Habitat_PromotionBook")
            {
                DisplayName = "Habitat Promotion Book",
                Description = "This is the Habitat promotion book"
            };

            var book = await _addBookPipeline.Run(addPromotionBookArgument, context);
            await _associateCatalogToBookPipeline.Run(new CatalogAndBookArgument(book.Name, "Habitat_Master"), context);
            return arg;
        }
    }
}
