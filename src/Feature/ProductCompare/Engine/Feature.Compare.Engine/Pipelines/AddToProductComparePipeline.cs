using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines
{
    public class AddToProductComparePipeline : CommercePipeline<AddProductToCompareArgument, ProductCompare>, IAddToProductComparePipeline
    {
        public AddToProductComparePipeline(IPipelineConfiguration<IAddToProductComparePipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
