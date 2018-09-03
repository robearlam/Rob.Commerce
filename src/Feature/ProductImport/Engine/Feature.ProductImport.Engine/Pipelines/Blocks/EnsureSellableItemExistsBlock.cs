using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class EnsureSellableItemExistsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        public override Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}
