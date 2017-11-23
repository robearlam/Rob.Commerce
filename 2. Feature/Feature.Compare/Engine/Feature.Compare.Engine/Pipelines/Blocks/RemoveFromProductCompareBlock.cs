using System.Linq;
using System.Threading.Tasks;
using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines.Blocks
{
    public class RemoveFromProductCompareBlock : PipelineBlock<RemoveProductFromCompareArgument, ProductCompare, CommercePipelineExecutionContext>
    {
        private readonly IRemoveListEntitiesPipeline _removeListEntitiesPipeline;

        public RemoveFromProductCompareBlock(IRemoveListEntitiesPipeline removeListEntitiesPipeline)
        {
            _removeListEntitiesPipeline = removeListEntitiesPipeline;
        }

        public override async Task<ProductCompare> Run(RemoveProductFromCompareArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The arg can not be null");
            Condition.Requires(arg.SellableItemId).IsNotNull($"{Name}: The product id can not be null");
            Condition.Requires(arg.CompareCollection).IsNotNull($"{Name}: The Compare Collection can not be null");

            var list = arg.CompareCollection.Products.ToList();
            if (list.All(x => x.Id != arg.SellableItemId))
            {
                context.Logger.LogDebug($"{Name}: SellableItem doesn't exist in compare collection, no further action to take");
                return arg.CompareCollection;
            }

            await _removeListEntitiesPipeline.Run(new ListEntitiesArgument(new[]
            {
                arg.SellableItemId
            }, arg.CompareCollection.Name), context.CommerceContext.GetPipelineContextOptions());

            return arg.CompareCollection;
        }
    }
}
