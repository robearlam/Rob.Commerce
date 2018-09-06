using System.Collections.Generic;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Foundation.PluginEnhancements.Engine.Entities
{
    public class UserPluginOptions : CommerceEntity
    {
        public UserPluginOptions()
        {
            EnabledPlugins = new List<string>();
        }

        public List<string> EnabledPlugins { get; set; }

        public Task<bool> Intialize(CommerceContext commerceContext)
        {
            Id = $"Entity-UserPluginOptions-{commerceContext.CurrentCsrId().Replace("\\", "|")}";

            return Task.FromResult(true);
        }       
    }
}