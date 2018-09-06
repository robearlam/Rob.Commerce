using System.Threading.Tasks;
using Feature.Orders.ServiceBus.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.Orders.ServiceBus.Engine.EntityViews
{
    [PipelineDisplayName("EntityViewEnsureServiceBus")]
    public class EntityViewEnsureServiceBus : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EntityViewEnsureServiceBus(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            if (entityView.Name != "ServiceBusDashboard" || string.IsNullOrEmpty(entityView.Action))
            {
                return Task.FromResult(entityView);
            }

            _commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<ServiceBusPluginPolicy>();
            var scopesView = new EntityView
            {
                EntityId = entityView.EntityId,
                ItemId = "",
                DisplayName = "ServiceBus Scopes",
                Name = "ServiceBusScopes",
                UiHint = "Table",
                Icon = pluginPolicy.Icon
            };
            entityView.ChildViews.Add(scopesView);

            var searchScopePolicies = context.CommerceContext.Environment.GetPolicies<SearchScopePolicy>();

            foreach (var searchScopePolicy in searchScopePolicies)
            {
                var newEntityView = new EntityView
                {
                    Name = "",
                    DisplayName = "",
                    Icon = pluginPolicy.Icon,
                    ItemId = searchScopePolicy.Name
                };
                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Name",
                        IsHidden = false,
                        RawValue = "Name"
                    });
                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "DeletedListName",
                        IsHidden = false,
                        RawValue = "DeletedListName"
                    });

                var entityTypes = "<table>";

                foreach(var entityType in searchScopePolicy.EntityTypeNames)
                {
                    entityTypes = entityTypes + $"<tr><td style='color: red;width:100%'>{entityType}</td></tr>";
                }

                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "EntityTypes",
                        IsHidden = false,
                        IsReadOnly = true,
                        OriginalType = "Html",
                        UiType = "Html",
                        RawValue = entityTypes + "</table>"
                    });

                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "FullListName",
                        IsHidden = false,
                        RawValue = searchScopePolicy.FullListName
                    });
                newEntityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "IncrementalListName",
                        IsHidden = false,
                        RawValue = searchScopePolicy.IncrementalListName
                    });

                scopesView.ChildViews.Add(newEntityView);
            }

            return Task.FromResult(entityView);
        }
    }
}