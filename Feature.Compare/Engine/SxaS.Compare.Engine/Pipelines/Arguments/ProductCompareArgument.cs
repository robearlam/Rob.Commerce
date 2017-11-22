using Sitecore.Commerce.Core;
using SxaS.Compare.Engine.Entities;

namespace SxaS.Compare.Engine.Pipelines.Arguments
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
