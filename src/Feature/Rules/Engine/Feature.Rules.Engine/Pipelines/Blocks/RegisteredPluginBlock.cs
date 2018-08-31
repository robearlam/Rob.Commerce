using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Feature.Rules.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Feature.Rules")]
    public class RegisteredPluginBlock : PipelineBlock<IEnumerable<RegisteredPluginModel>, IEnumerable<RegisteredPluginModel>, CommercePipelineExecutionContext>
    {
        public override Task<IEnumerable<RegisteredPluginModel>> Run(IEnumerable<RegisteredPluginModel> arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return Task.FromResult((IEnumerable<RegisteredPluginModel>)null);
            }

            var plugins = arg.ToList();
            PluginHelper.RegisterPlugin(this, plugins);

            return Task.FromResult(plugins.AsEnumerable());
        }
    }
}
