using System.Threading.Tasks;
using Feature.Compare.Engine.Entities;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines.Blocks
{
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull("The argument can not be null");

            modelBuilder.AddEntityType(typeof(ProductCompare));
            modelBuilder.EntitySet<ProductCompare>("Compare");

            var addProductToComparison = modelBuilder.Action("AddProductToComparison");
            addProductToComparison.Returns<string>();
            addProductToComparison.Parameter<string>("cartId");
            addProductToComparison.Parameter<string>("catalogName");
            addProductToComparison.Parameter<string>("productId");
            addProductToComparison.Parameter<string>("varientId");
            addProductToComparison.ReturnsFromEntitySet<CommerceCommand>("Commands");

            var removeProductFromComparison = modelBuilder.Action("RemoveProductFromComparison");
            removeProductFromComparison.Returns<string>();
            removeProductFromComparison.Parameter<string>("cartId");
            removeProductFromComparison.Parameter<string>("sellableItemId");
            removeProductFromComparison.ReturnsFromEntitySet<CommerceCommand>("Commands");

            return Task.FromResult(modelBuilder);
        }
    }
}
