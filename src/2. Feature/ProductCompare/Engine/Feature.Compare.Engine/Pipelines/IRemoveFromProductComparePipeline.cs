using Feature.Compare.Engine.Entities;
using Feature.Compare.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Compare.Engine.Pipelines
{
    public interface IRemoveFromProductComparePipeline : IPipeline<RemoveProductFromCompareArgument, ProductCompare, CommercePipelineExecutionContext>
    {
        
    }
}