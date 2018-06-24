using Sitecore.Commerce.Services;

namespace Feature.Compare.Website.Managers.Messages
{
    public class RemoveFromCompareResult : ServiceProviderResult
    {
        public string RemovedSellableItemId { get; set; }
    }
}