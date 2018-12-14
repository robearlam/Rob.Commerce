using Sitecore.Commerce.Core;

namespace Foundation.PluginEnhancements.Engine.Policies
{
    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            Icon = "cubes";
        }

        public string Icon { get; set; }
    }
}