using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines.Blocks
{
    public class AddToProductCompareBlock : PipelineBlock<AddProductToCompareArgument, ProductCompare, CommercePipelineExecutionContext>
    {
        private readonly IGetSellableItemPipeline _getSellableItemPipeline;
        private readonly IAddListEntitiesPipeline _addListEntitiesPipeline;

        public AddToProductCompareBlock(IGetSellableItemPipeline getSellableItemPipeline, IAddListEntitiesPipeline addListEntitiesPipeline)
        {
            _getSellableItemPipeline = getSellableItemPipeline;
            _addListEntitiesPipeline = addListEntitiesPipeline;
        }

        public override async Task<ProductCompare> Run(AddProductToCompareArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The arg can not be null");
            Condition.Requires(arg.ProductId).IsNotNull($"{Name}: The product id can not be null");
            Condition.Requires(arg.CompareCollection).IsNotNull($"{Name}: The Compare Collection can not be null");

            var sellableItem = await _getSellableItemPipeline.Run(BuildProductArgument(arg), context);
            if (sellableItem == null)
            {
                context.Logger.LogWarning($"ProductCompare: Unable to find sellable item to add to collection:{arg.CatalogName}-{arg.ProductId}-{arg.VariantId}");
                return arg.CompareCollection;
            }

            var list = arg.CompareCollection.Products.ToList();
            if (list.Any(x => x.Id == sellableItem.Id))
            {
                context.Logger.LogDebug($"{Name}: SellableItem already exists in compare collection, no further action to take");
                return arg.CompareCollection;
            }

            var addArg = new ListEntitiesArgument(new List<string> {sellableItem.Id}, arg.CompareCollection.Name);
            var result = await _addListEntitiesPipeline.Run(addArg, context);
            return arg.CompareCollection;
        }

        private static ProductArgument BuildProductArgument(AddProductToCompareArgument arg)
        {
            return new ProductArgument
            {
                CatalogName = arg.CatalogName,
                ProductId = arg.ProductId,
                VariantId = arg.VariantId
            };
        }
    }
}
