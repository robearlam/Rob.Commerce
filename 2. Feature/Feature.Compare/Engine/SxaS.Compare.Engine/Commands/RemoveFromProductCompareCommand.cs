using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using SxaS.Compare.Engine.Entities;
using SxaS.Compare.Engine.Pipelines;
using SxaS.Compare.Engine.Pipelines.Arguments;

namespace SxaS.Compare.Engine.Commands
{
    public class RemoveFromProductCompareCommand : GetProductCompareCommand
    {
        private readonly IRemoveFromProductComparePipeline _removeFromProductComparePipeline;

        public RemoveFromProductCompareCommand(IGetProductComparePipeline getProductComparePipeline, IRemoveFromProductComparePipeline removeFromProductComparePipeline, IServiceProvider serviceProvider) : base(getProductComparePipeline, serviceProvider)
        {
            _removeFromProductComparePipeline = removeFromProductComparePipeline;
        }

        public virtual async Task<ProductCompare> Process(CommerceContext commerceContext, string cartId, string sellableItemId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var productCompareComponent = await GetProductCompareComponent(commerceContext, cartId);
                var arg = new RemoveProductFromCompareArgument(productCompareComponent, sellableItemId);
                return await _removeFromProductComparePipeline.Run(arg, new CommercePipelineExecutionContextOptions(commerceContext));
            }
        }
    }
}
