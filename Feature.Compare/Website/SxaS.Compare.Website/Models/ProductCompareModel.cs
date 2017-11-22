using System.Collections.Generic;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models;

namespace SxaS.Compare.Website.Models
{
    public class ProductCompareModel : BaseCommerceRenderingModel
    {
        public IList<ProductCompareListItemModel> Products { get; set; }
        public bool IsValid { get; set; }
        public string RemoveFromCompareText { get; set; }
    }
}