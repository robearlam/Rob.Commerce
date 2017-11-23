using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines
{
    public class RemoveFromProductComparePipeline : CommercePipeline<RemoveProductFromCompareArgument, ProductCompare>, IRemoveFromProductComparePipeline
    {
        public RemoveFromProductComparePipeline(IPipelineConfiguration<IRemoveFromProductComparePipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
