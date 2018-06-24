using System.Collections.Generic;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;

namespace Feature.Compare.Engine.Entities
{
    public class ProductCompare : CommerceEntity
    {
        [Contained]
        public IEnumerable<SellableItem> Products { get; set; }
    }
}
