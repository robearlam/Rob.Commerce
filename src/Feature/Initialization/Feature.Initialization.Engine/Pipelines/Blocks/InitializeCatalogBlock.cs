using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Internal;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Composer;
using Sitecore.Commerce.Plugin.ManagedLists;
using Sitecore.Commerce.Plugin.Views;
using Sitecore.Framework.Pipelines;

namespace Feature.Initialization.Engine.Pipelines.Blocks
{
    [PipelineDisplayName(HabitatConstants.Pipelines.Blocks.InitializeCatalogBlock)]
    public class InitializeCatalogBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ImportCatalogsCommand _importCatalogsCommand;
        private readonly CommerceCommander _commerceCommander;

        public InitializeCatalogBlock(IHostingEnvironment hostingEnvironment, ImportCatalogsCommand importCatalogsCommand, CommerceCommander commerceCommander)
        {
            _hostingEnvironment = hostingEnvironment;
            _importCatalogsCommand = importCatalogsCommand;
            _commerceCommander = commerceCommander;
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            var artifactSet = "Environment.Habitat.Catalog-1.0";
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
                return arg;

            await CreateEntityComposerTemplates(context);
            await ImportCatalogData(context);

            return arg;
        }

        private async Task CreateEntityComposerTemplates(CommercePipelineExecutionContext context)
        {
            await CreateManufacturerDataTemplate(context);
            await CreateSizingTemplate(context);
        }

        private async Task CreateSizingTemplate(CommercePipelineExecutionContext context)
        {
            var composerTemplate = new ComposerTemplate("Sizing".ToEntityId<ComposerTemplate>());
            composerTemplate.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<ComposerTemplate>());
            composerTemplate.LinkedEntities = new List<string>() { "Sitecore.Commerce.Plugin.Catalog.SellableItem" };
            composerTemplate.Name = "Sizing";
            composerTemplate.DisplayName = "Sizing";

            var composerTemplateViewComponent = composerTemplate.GetComponent<EntityViewComponent>();
            var composerTemplateView = new EntityView
            {
                Name = "Sizing",
                DisplayName = "Sizing Data",
                DisplayRank = 0,
                ItemId = $"Composer-{System.Guid.NewGuid()}",
                EntityId = composerTemplate.Id
            };

            composerTemplateView.Properties.Add(new ViewProperty
            {
                Name = "Waist",
                DisplayName = "Waist",
                OriginalType = "System.Int64",
            });          

            composerTemplateView.Properties.Add(new ViewProperty
            {
                Name = "InsideLeg",
                DisplayName = "Inside Leg",
                OriginalType = "System.Int64",
            });

            composerTemplateView.Properties.Add(new ViewProperty
            {
                Name = "OutsideLeg",
                DisplayName = "Outside Leg",
                OriginalType = "System.Int64",
            });

            composerTemplateViewComponent.View.ChildViews.Add(composerTemplateView);
            await _commerceCommander.PersistEntity(context.CommerceContext, composerTemplate);
        }

        private async Task CreateManufacturerDataTemplate(CommercePipelineExecutionContext context)
        {
            var composerTemplate = new ComposerTemplate("Manufacturing".ToEntityId<ComposerTemplate>());
            composerTemplate.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<ComposerTemplate>());
            composerTemplate.LinkedEntities = new List<string>() {"Sitecore.Commerce.Plugin.Catalog.SellableItem"};
            composerTemplate.Name = "Manufacturing";
            composerTemplate.DisplayName = "Manufacturing";

            var composerTemplateViewComponent = composerTemplate.GetComponent<EntityViewComponent>();
            var composerTemplateView = new EntityView
            {
                Name = "Manufacturing",
                DisplayName = "Manufacturing",
                DisplayRank = 0,
                ItemId = $"Composer-{System.Guid.NewGuid()}",
                EntityId = composerTemplate.Id
            };

            composerTemplateView.Properties.Add(new ViewProperty
            {
                Name = "CountryOfOrigin",
                DisplayName = "Country of Origin",
                OriginalType = "System.String",
            });

            composerTemplateView.Properties.Add(new ViewProperty
            {
                Name = "ManufacturerType",
                DisplayName = "Manufacturer Type",
                OriginalType = "System.String",
                Policies = new List<Policy>
                {
                    new AvailableSelectionsPolicy
                    {
                        List = new List<Selection>
                        {
                            new Selection
                            {
                                DisplayName = "Hand",
                                Name = "Hand"
                            },
                            new Selection
                            {
                                DisplayName = "Automated",
                                Name = "Automated"
                            }
                        }
                    }
                }
            });

            composerTemplateViewComponent.View.ChildViews.Add(composerTemplateView);
            await _commerceCommander.PersistEntity(context.CommerceContext, composerTemplate);
        }

        private async Task ImportCatalogData(CommercePipelineExecutionContext context)
        {
            using (var stream = new FileStream(GetPath("Habitat.zip"), FileMode.Open, FileAccess.Read))
            {
                var file = new FormFile(stream, 0, stream.Length, stream.Name, stream.Name);
                await _importCatalogsCommand.Process(context.CommerceContext, file, CatalogConstants.Replace, 100, 100);
            }
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, "data", "Catalogs", fileName);
        }
    }
}
