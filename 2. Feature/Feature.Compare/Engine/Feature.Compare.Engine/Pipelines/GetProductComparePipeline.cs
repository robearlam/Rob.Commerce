using Feature.Compare.Engine.Entities;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines
{
    public class GetProductComparePipeline : CommercePipeline<string, ProductCompare>, IGetProductComparePipeline
    {
        public GetProductComparePipeline(IPipelineConfiguration<IGetProductComparePipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
