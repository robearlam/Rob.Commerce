using Sitecore.Commerce.Services;

namespace SxaS.Compare.Website.Managers.Messages
{
    public class RemoveFromCompareResult : ServiceProviderResult
    {
        public string RemovedSellableItemId { get; set; }
    }
}