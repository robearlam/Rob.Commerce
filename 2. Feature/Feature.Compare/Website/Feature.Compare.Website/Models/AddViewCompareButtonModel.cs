using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Data.Items;

namespace Feature.Compare.Website.Models
{
    public class AddViewCompareButtonModel : BaseCommerceRenderingModel
    {
        public string AddToCompareButtonText { get; set; }
        public string ViewCompareButtonText { get; set; }
        public string AddingToCompareWaitText { get; set; }
        public bool IsProductInCompareList { get; set; }
        public string CatalogName { get; set; }
        public string ProductId { get; set; }
        public string VarientId { get; set; }

        public void Initialize(Item productItem)
        {
            ProductId = productItem.Name;
        }
    }
}