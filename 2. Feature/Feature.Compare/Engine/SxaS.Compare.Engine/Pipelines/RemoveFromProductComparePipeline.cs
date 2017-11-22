using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using SxaS.Compare.Engine.Entities;
using SxaS.Compare.Engine.Pipelines.Arguments;

namespace SxaS.Compare.Engine.Pipelines
{
    public class RemoveFromProductComparePipeline : CommercePipeline<RemoveProductFromCompareArgument, ProductCompare>, IRemoveFromProductComparePipeline
    {
        public RemoveFromProductComparePipeline(IPipelineConfiguration<IRemoveFromProductComparePipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
