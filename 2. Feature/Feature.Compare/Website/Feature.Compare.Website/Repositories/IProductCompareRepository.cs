using Feature.Compare.Website.Models;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Connect;

namespace Feature.Compare.Website.Repositories
{
    public interface IProductCompareRepository
    {
        AddViewCompareButtonModel GetAddViewCompareButtonModel(IStorefrontContext storefrontContext, IVisitorContext visitorContext);
        ProductCompareModel GetProductCompareModel(IStorefrontContext storefrontContext, IVisitorContext visitorContext);
        BaseJsonResult AddProductToCompareCollection(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string catalogName, string productId, string varientId);
        RemoveFromProductCompareModel RemoveProductFromCompareCollection(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string sellableItemId);
    }
}