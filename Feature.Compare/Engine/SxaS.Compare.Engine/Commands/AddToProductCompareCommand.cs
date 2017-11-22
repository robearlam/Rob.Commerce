using System;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using SxaS.Compare.Engine.Entities;
using SxaS.Compare.Engine.Pipelines;
using SxaS.Compare.Engine.Pipelines.Arguments;

namespace SxaS.Compare.Engine.Commands
{
    public class AddToProductCompareCommand : GetProductCompareCommand
    {
        private readonly IAddToProductComparePipeline _addToProductComparePipeline;

        public AddToProductCompareCommand(IGetProductComparePipeline getProductComparePipeline, IAddToProductComparePipeline addToProductComparePipeline, IServiceProvider serviceProvider) : base(getProductComparePipeline, serviceProvider)
        {
            _addToProductComparePipeline = addToProductComparePipeline;
        }

        public virtual async Task<ProductCompare> Process(CommerceContext commerceContext, string cartId, string catalogName, string productId, string variantId)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var productCompareComponent = await GetProductCompareComponent(commerceContext, cartId);
                var arg = new AddProductToCompareArgument(productCompareComponent, catalogName, productId, variantId);
                return await _addToProductComparePipeline.Run(arg, new CommercePipelineExecutionContextOptions(commerceContext));
            }
        }
    }
}
