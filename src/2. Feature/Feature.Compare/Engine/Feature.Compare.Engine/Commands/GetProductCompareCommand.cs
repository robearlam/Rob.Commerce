using System;
using System.Threading.Tasks;
using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;

namespace Feature.Compare.Engine.Commands
{
    public class GetProductCompareCommand : CommerceCommand
    {
        private readonly IGetProductComparePipeline _getProductComparePipeline;

        public GetProductCompareCommand(IGetProductComparePipeline getProductComparePipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _getProductComparePipeline = getProductComparePipeline;
        }

        public virtual async Task<ProductCompare> Process(CommerceContext context, string cartId)
        {
            return await GetProductCompareComponent(context, cartId);
        }

        protected virtual async Task<ProductCompare> GetProductCompareComponent(CommerceContext context, string cartId)
        {
            if (string.IsNullOrEmpty(cartId))
            {
                return null;
            }

            var entityPrefix = CommerceEntity.IdPrefix<ProductCompare>();
            var entityId = cartId.StartsWith(entityPrefix, StringComparison.OrdinalIgnoreCase) ? cartId : $"{entityPrefix}{cartId}";
            var options = new CommercePipelineExecutionContextOptions(context);
            var productCompareComponent = await _getProductComparePipeline.Run(entityId, options);
            if (productCompareComponent == null)
            {
                context.Logger.LogDebug($"Entity {entityId} was not found.");
            }

            return productCompareComponent;
        }
    }
}
