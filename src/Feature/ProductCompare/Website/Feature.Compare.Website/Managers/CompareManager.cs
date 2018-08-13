using System;
using Feature.Compare.Engine.Entities;
using Feature.Compare.Website.Managers.Messages;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Commerce.XA.Foundation.Connect.Providers;
using Sitecore.Diagnostics;

namespace Feature.Compare.Website.Managers
{
    public class CompareManager : CartManager, ICompareManager
    {
        public CompareManager(IConnectServiceProvider connectServiceProvider) : base(connectServiceProvider)
        {
        }

        public virtual ManagerResponse<ProductCompareResult, ProductCompare> GetCurrentProductCompare(IVisitorContext visitorContext, IStorefrontContext storefrontContext)
        {
            var serviceProviderResult = LoadCompare(storefrontContext.CurrentStorefront.ShopName, visitorContext.CustomerId);
            var productCompare = serviceProviderResult.ProductCompare;
            return new ManagerResponse<ProductCompareResult, ProductCompare>(serviceProviderResult, productCompare);
        }

        public virtual ManagerResponse<ProductCompareResult, ProductCompare> AddProductToCompareCollection(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string catalogName, string productId, string varientId)
        {
            var productCompareResult = AddToCompare(storefrontContext.CurrentStorefront.ShopName, visitorContext.CustomerId, catalogName, productId, varientId);
            return new ManagerResponse<ProductCompareResult, ProductCompare>(productCompareResult, productCompareResult.ProductCompare);
        }

        public ManagerResponse<RemoveFromCompareResult, string> RemoveProductFromCompareCollection(IVisitorContext visitorContext, IStorefrontContext storefrontContext, string sellableItemId)
        {
            var productCompareResult = RemoveFromCompare(storefrontContext.CurrentStorefront.ShopName, visitorContext.CustomerId, sellableItemId);
            return new ManagerResponse<RemoveFromCompareResult, string>(productCompareResult, productCompareResult.RemovedSellableItemId);
        }

        protected RemoveFromCompareResult RemoveFromCompare(string shopName, string customerId, string sellableItemId)
        {
            var result = new RemoveFromCompareResult { RemovedSellableItemId = sellableItemId };
            try
            {
                var container = EngineConnectUtility.GetShopsContainer(shopName: shopName, customerId: customerId);
                Proxy.DoCommand(container.RemoveProductFromComparison(customerId, sellableItemId));
                result.Success = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to add product to Compare for shopName:'{shopName}', customerId'{customerId}'", ex, this);
            }
            return result;
        }

        protected ProductCompareResult AddToCompare(string shopName, string customerId, string catalogName, string productId, string varientId)
        {
            try
            {
                var container = EngineConnectUtility.GetShopsContainer(shopName: shopName, customerId: customerId);
                Proxy.DoCommand(container.AddProductToComparison(customerId, catalogName, productId, varientId));
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to add product to Compare for shopName:'{shopName}', customerId'{customerId}'", ex, this);
            }
            return LoadCompare(shopName, customerId);
        }

        protected ProductCompareResult LoadCompare(string shopName, string customerId)
        {
            var productCompareResult = new ProductCompareResult();
            try
            {
                var container = EngineConnectUtility.GetShopsContainer(shopName: shopName, customerId: customerId);
                var dataServiceActionQuerySingle = container.Compare.ByKey(customerId).Expand("Products");
                productCompareResult.ProductCompare = Proxy.GetValue(dataServiceActionQuerySingle);
                productCompareResult.Success = true;
            }
            catch (Exception ex)
            {
                Log.Error($"Unable to retrieve Product Compare for shopName:'{shopName}', customerId'{customerId}'", ex, this);
            }
            return productCompareResult;
        }
    }
}