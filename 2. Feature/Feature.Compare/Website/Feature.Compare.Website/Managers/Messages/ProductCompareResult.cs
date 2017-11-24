using Feature.Compare.Engine.Entities;
using Sitecore.Commerce.Services;

namespace Feature.Compare.Website.Managers.Messages
{
    public class ProductCompareResult : ServiceProviderResult
    {
        public ProductCompare ProductCompare { get; set; }
    }
}