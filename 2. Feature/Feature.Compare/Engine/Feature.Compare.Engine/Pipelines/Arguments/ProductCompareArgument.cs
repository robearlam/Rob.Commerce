using Feature.Compare.Engine.Entities;
using Sitecore.Commerce.Core;

namespace Feature.Compare.Engine.Pipelines.Arguments
{
    public abstract class ProductCompareArgument : PipelineArgument
    {
        public ProductCompare CompareCollection { get; }

        protected ProductCompareArgument(ProductCompare compareCollection)
        {
            CompareCollection = compareCollection;
        }

    }
}
