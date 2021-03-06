﻿using System;
using System.Threading.Tasks;
using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Catalog;

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

            //add a prefix to the cart id so it won't parse into a guid
            cartId += "xxx";
            var entityId = cartId.StartsWith(CommerceEntity.IdPrefix<ProductCompare>(), StringComparison.OrdinalIgnoreCase) ? cartId : cartId.ToEntityId<ProductCompare>();
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
