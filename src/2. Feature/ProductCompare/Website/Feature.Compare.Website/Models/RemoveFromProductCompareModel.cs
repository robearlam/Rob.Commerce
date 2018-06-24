using System;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;

namespace Feature.Compare.Website.Models
{
    public class RemoveFromProductCompareModel : BaseJsonResult
    {
        public string RemovedSellableItemId { get; set; }

        public RemoveFromProductCompareModel(IStorefrontContext storefrontContext) : base(storefrontContext)
        {
        }

        public RemoveFromProductCompareModel(ServiceProviderResult result, IStorefrontContext storefrontContext) : base(result, storefrontContext)
        {
        }

        public RemoveFromProductCompareModel(string area, Exception exception, IStorefrontContext storefrontContext) : base(area, exception, storefrontContext)
        {
        }

        public RemoveFromProductCompareModel(string url, IStorefrontContext storefrontContext) : base(url, storefrontContext)
        {
        }
    }
}