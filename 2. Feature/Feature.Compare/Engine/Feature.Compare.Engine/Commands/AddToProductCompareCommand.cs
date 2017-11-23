using System;
using System.Threading.Tasks;
using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines;
using Feature.Compare.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;

namespace Feature.Compare.Engine.Commands
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
