using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models;

namespace Feature.Compare.Website.Models
{
    public class ProductCompareListItemModel : BaseCommerceRenderingModel
    {
        public string  SellableItemId { get; set; }
        public int Depth { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string EnergyConsumption { get; set; }
        public string EnergyStarRating { get; set; }
        public bool IceDispenser { get; set; }
        public string InternalLighting { get; set; }
        public int Volume { get; set; }
        public CatalogItemRenderingModel CatalogItemRenderingModel { get; set; }
    }
}