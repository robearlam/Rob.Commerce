using System;
using System.Threading.Tasks;
using Foundation.PluginEnhancements.Engine.Entities;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;

namespace Foundation.PluginEnhancements.Engine.Commands
{
    public class PluginCommander : CommerceCommand
    {
        public PluginCommander(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<UserPluginOptions> CurrentUserSettings(CommerceContext commerceContext, CommerceCommander commerceCommander)
        {
            var userPluginOptionsId = $"Entity-UserPluginOptions-{commerceContext.CurrentCsrId().Replace("\\", "|")}";

            var userPluginOptions = await commerceCommander.GetEntity<UserPluginOptions>(commerceContext, userPluginOptionsId);

            if (userPluginOptions == null)
            {
                userPluginOptions = new UserPluginOptions {
                    Id = userPluginOptionsId
                };
            }

            return userPluginOptions;
        }
    }
}