using Sitecore.Commerce.Services;
using SxaS.Compare.Engine.Entities;

namespace SxaS.Compare.Website.Managers.Messages
{
    public class ProductCompareResult : ServiceProviderResult
    {
        public ProductCompare ProductCompare { get; set; }
    }
}