using System;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;

namespace Feature.Compare.Website.Models
{
    public class RemoveFromProductCompareModel : BaseJsonResult
    {
        public string RemovedSellableItemId { get; set; }

        public RemoveFromProductCompareModel(IContext context, IStorefrontContext storefrontContext) : base(context, storefrontContext)
        {
        }

        public RemoveFromProductCompareModel(ServiceProviderResult result, IStorefrontContext storefrontContext, IContext context) : base(result, storefrontContext, context)
        {
        }

        public RemoveFromProductCompareModel(string area, Exception exception, IStorefrontContext storefrontContext, IContext context) : base(area, exception, storefrontContext, context)
        {
        }

        public RemoveFromProductCompareModel(string url, IStorefrontContext storefrontContext) : base(url, storefrontContext)
        {
        }
    }
}