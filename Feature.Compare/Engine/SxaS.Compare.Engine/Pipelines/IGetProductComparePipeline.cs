using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using SxaS.Compare.Engine.Entities;

namespace SxaS.Compare.Engine.Pipelines
{
    public interface IGetProductComparePipeline : IPipeline<string, ProductCompare, CommercePipelineExecutionContext>
    {
        
    }
}