using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using SxaS.Compare.Engine.Entities;

namespace SxaS.Compare.Engine.Pipelines.Blocks
{
    public class GetProductCompareBlock : PipelineBlock<string, ProductCompare, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;

        public GetProductCompareBlock(IFindEntitiesInListPipeline findEntitiesInListPipeline)
        {
            _findEntitiesInListPipeline = findEntitiesInListPipeline;
        }

        public override async Task<ProductCompare> Run(string arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The compare collection id can not be null");

            var productCompare = new ProductCompare
            {
                Name = arg,
                Products = await GetListItems(arg, 10, context)
            };
            return productCompare;
        }

        protected virtual async Task<IEnumerable<SellableItem>> GetListItems(string listName, int take, CommercePipelineExecutionContext context)
        {
            return (await _findEntitiesInListPipeline.Run(new FindEntitiesInListArgument(typeof(SellableItem), listName, 0, take), context)).List.Items.OfType<SellableItem>();
        }
    }
}