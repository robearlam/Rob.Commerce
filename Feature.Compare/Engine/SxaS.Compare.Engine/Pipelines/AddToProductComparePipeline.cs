using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using SxaS.Compare.Engine.Entities;
using SxaS.Compare.Engine.Pipelines.Arguments;

namespace SxaS.Compare.Engine.Pipelines
{
    public class AddToProductComparePipeline : CommercePipeline<AddProductToCompareArgument, ProductCompare>, IAddToProductComparePipeline
    {
        public AddToProductComparePipeline(IPipelineConfiguration<IAddToProductComparePipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
