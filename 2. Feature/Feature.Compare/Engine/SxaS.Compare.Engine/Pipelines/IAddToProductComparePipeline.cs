using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using SxaS.Compare.Engine.Entities;
using SxaS.Compare.Engine.Pipelines.Arguments;

namespace SxaS.Compare.Engine.Pipelines
{
    public interface IAddToProductComparePipeline : IPipeline<AddProductToCompareArgument, ProductCompare, CommercePipelineExecutionContext>
    {

    }
}