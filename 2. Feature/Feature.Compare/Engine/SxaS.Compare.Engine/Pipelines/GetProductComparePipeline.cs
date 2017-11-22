using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using SxaS.Compare.Engine.Entities;

namespace SxaS.Compare.Engine.Pipelines
{
    public class GetProductComparePipeline : CommercePipeline<string, ProductCompare>, IGetProductComparePipeline
    {
        public GetProductComparePipeline(IPipelineConfiguration<IGetProductComparePipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
