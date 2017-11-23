using Feature.Compare.Engine.Entities;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines
{
    public interface IGetProductComparePipeline : IPipeline<string, ProductCompare, CommercePipelineExecutionContext>
    {
        
    }
}