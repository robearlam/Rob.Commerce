using System.Collections.Generic;
using System.Linq;
using Feature.Compare.Website.Managers;
using Feature.Compare.Website.Models;
using Microsoft.OData.Client;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.XA.Feature.Catalog.MockData;
using Sitecore.Commerce.XA.Feature.Catalog.Models;
using Sitecore.Commerce.XA.Feature.Catalog.Repositories;
using Sitecore.Commerce.XA.Foundation.Catalog.Managers;
using Sitecore.Commerce.XA.Foundation.Common;
using Sitecore.Commerce.XA.Foundation.Common.Models;
using Sitecore.Commerce.XA.Foundation.Common.Models.JsonResults;
using Sitecore.Commerce.XA.Foundation.Common.Search;
using Sitecore.Commerce.XA.Foundation.Connect;
using Sitecore.Commerce.XA.Foundation.Connect.Managers;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Feature.Compare.Website.Repositories
{
    public class ProductCompareRepository : BaseCatalogRepository, IProductCompareRepository
    {
        private readonly IModelProvider _modelProvider;
        private readonly ICompareManager _compareManager;
        private readonly ISiteContext _siteContext;
        private const int DefaultIntValue = -1;

        public ProductCompareRepository(IModelProvider modelProvider, ISiteContext siteContext, ICompareManager compareManager, IStorefrontContext storefrontContext, ISearchInformation searchInformation, ISearchManager searchManager, ICatalogManager catalogManager, ICatalogUrlManager catalogUrlManager) 
            : base(modelProvider,storefrontContext,siteContext,searchInformation,searchManager,catalogManager,catalogUrlManager)
        {
            Assert.ArgumentNotNull(modelProvider, nameof(modelProvider));
            _modelProvider = modelProvider;
            _compareManager = compareManager;
            _siteContext = siteContext;
        }

        public virtual AddViewCompareButtonModel GetAddViewCompareButtonModel(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            var model = _modelProvider.GetModel<AddViewCompareButtonModel>();
            Init(model);
            if (Sitecore.Context.PageMode.IsExperienceEditor)
            {
                model.IsProductInCompareList = false;
                model.IsEdit = true;
            }
            else
            {
                var currentCatalogItem = _siteContext.CurrentCatalogItem;
                if (currentCatalogItem != null && _siteContext.IsProduct)
                    model.Initialize(currentCatalogItem);

                model.CatalogName = StorefrontContext.CurrentStorefront.Catalog;
                var productCompare = _compareManager.GetCurrentProductCompare(visitorContext, storefrontContext);
                var productIsInCompare = productCompare?.Result != null &&
                                         productCompare.Result.Products.Any(x => x.FriendlyId == currentCatalogItem?.Name);

                model.IsProductInCompareList = productIsInCompare;
            }

            model.ViewCompareButtonText = "View Product Comparison";
            model.AddToCompareButtonText = "Add to Compare";
            return model;
        }

        public virtual ProductCompareModel GetProductCompareModel(IStorefrontContext storefrontContext, IVisitorContext visitorContext)
        {
            var model = _modelProvider.GetModel<ProductCompareModel>();
            if (Sitecore.Context.PageMode.IsExperienceEditor)
            {
                model.Products = new List<ProductCompareListItemModel>();
                for (var i = 0; i <= 3; i++)
                {
                    model.Products.Add(GenerateMockProductCompareListModel());
                }
            }
            else
            {
                var productCompare = _compareManager.GetCurrentProductCompare(visitorContext, storefrontContext);
                if (productCompare.ServiceProviderResult.Success)
                {
                    model.Products = ConvertSellableItemsToModelList(productCompare.Result.Products, visitorContext);
                    model.RemoveFromCompareText = "Remove";
                    model.IsValid = true;
                }
            }
            return model;
        }

        private List<ProductCompareListItemModel> ConvertSellableItemsToModelList(DataServiceCollection<SellableItem> resultProducts, IVisitorContext visitorContext)
        {
            var items = new List<ProductCompareListItemModel>();

            foreach (var sellableItem in resultProducts)
            {
                var scItem = SearchManager.GetProduct(sellableItem.FriendlyId, StorefrontContext.CurrentStorefront.Catalog);
                if (scItem != null)
                {
                    var productCompareListItemModel = GenerateProductCompareListModel(scItem, sellableItem, visitorContext);
                    items.Add(productCompareListItemModel);
                }
                SiteContext.Items.Remove("CurrentCatalogItemRenderingModel");
            }
            return items;
        }

        private ProductCompareListItemModel GenerateProductCompareListModel(Item scItem, SellableItem sellableItem, IVisitorContext visitorContext)
        {
            return new ProductCompareListItemModel
            {
                CatalogItemRenderingModel = GetCatalogItemRenderingModel(visitorContext, scItem),
                Height = GetIntegerFromField(scItem, "Height"),
                Depth = GetIntegerFromField(scItem, "Depth"),
                Width = GetIntegerFromField(scItem, "Width"),
                EnergyConsumption = scItem["EnergyConsumption"],
                EnergyStarRating = scItem["EnergyStarRating"],
                IceDispenser = scItem["IceDispenser"] == "1",
                InternalLighting = scItem["InternalLighting"],
                Volume = GetIntegerFromField(scItem, "Volume"),
                SellableItemId = sellableItem.Id
            };
        }

        private ProductCompareListItemModel GenerateMockProductCompareListModel()
        {
            var model = _modelProvider.GetModel<CatalogItemRenderingModel>();
            return new ProductCompareListItemModel
            {
                CatalogItemRenderingModel = CatalogItemRenderingModelMockData.InitializeMockData(model),
                Height = 999,
                Depth = 999,
                Width = 999,
                EnergyConsumption = "999Khw",
                EnergyStarRating = "5 Star",
                IceDispenser = true,
                InternalLighting = "LED",
                Volume = 999,
                SellableItemId = "1234",
                IsEdit = true
            };
        }

        private static int GetIntegerFromField(Item scItem, string fieldName)
        {
            if (scItem?.Fields[fieldName] == null)
                return DefaultIntValue;

            int val;
            return int.TryParse(scItem.Fields[fieldName].Value, out val) 
                   ? val 
                   : DefaultIntValue;
        }

        public virtual BaseJsonResult AddProductToCompareCollection(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string catalogName, string productId, string varientId)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNullOrEmpty(catalogName, nameof(catalogName));
            Assert.ArgumentNotNullOrEmpty(productId, nameof(productId));

            var model = _modelProvider.GetModel<BaseJsonResult>();

            var productCompare = _compareManager.AddProductToCompareCollection(visitorContext, storefrontContext, catalogName, productId, varientId);
            if (!productCompare.ServiceProviderResult.Success)
            {
                model.SetErrors(productCompare.ServiceProviderResult);
                return model;
            }

            model.Success = true;
            return model;
        }

        public RemoveFromProductCompareModel RemoveProductFromCompareCollection(IStorefrontContext storefrontContext, IVisitorContext visitorContext, string sellableItemId)
        {
            Assert.ArgumentNotNull(storefrontContext, nameof(storefrontContext));
            Assert.ArgumentNotNull(visitorContext, nameof(visitorContext));
            Assert.ArgumentNotNullOrEmpty(sellableItemId, nameof(sellableItemId));

            var model = _modelProvider.GetModel<RemoveFromProductCompareModel>();
            model.RemovedSellableItemId = sellableItemId;

            var productCompare = _compareManager.RemoveProductFromCompareCollection(visitorContext, storefrontContext, sellableItemId);
            if (!productCompare.ServiceProviderResult.Success)
            {
                model.SetErrors(productCompare.ServiceProviderResult);
                return model;
            }

            model.Success = true;
            return model;
        }
    }
}