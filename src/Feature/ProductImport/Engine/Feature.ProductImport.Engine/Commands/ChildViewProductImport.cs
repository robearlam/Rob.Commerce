using System;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.EntityViews;

namespace Feature.ProductImport.Engine.Commands
{
    public class ChildViewProductImport : CommerceCommand
    {
        public EntityView Process(CommerceContext commerceContext, EntityView entityView)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    var tableView = new EntityView
                    {
                        EntityId = string.Empty,
                        Name = "Import New Catalog",
                        UiHint = "Table"
                    };
                    entityView.ChildViews.Add(tableView);

                    var containerView = new EntityView
                    {
                        EntityId = "bob123",
                        ItemId = "bob123",
                        EntityVersion = 1,
                        Name = "Summary"
                    };

                    var properties = containerView.Properties;
                    var viewPropertyArray = new ViewProperty[1];
                    var viewProperty1 = new ViewProperty
                    {
                        Name = "Name",
                        RawValue = "bob678",
                        IsReadOnly = false,
                        UiType = "EntityLink"
                    };
                    viewPropertyArray[0] = viewProperty1;
                    
                    properties.AddRange(viewPropertyArray);
                    tableView.ChildViews.Add(containerView);
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"ChildViewProductImport.Exception: Message={ex.Message}");
                }
                return entityView;
            }
        }
    }
}
