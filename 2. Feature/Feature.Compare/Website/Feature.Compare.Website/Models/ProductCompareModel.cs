using System.Collections.Generic;
using Sitecore.Commerce.XA.Foundation.Common.Models;

namespace Feature.Compare.Website.Models
{
    public class ProductCompareModel : BaseCommerceRenderingModel
    {
        public IList<ProductCompareListItemModel> Products { get; set; }
        public bool IsValid { get; set; }
        public string RemoveFromCompareText { get; set; }
    }
}