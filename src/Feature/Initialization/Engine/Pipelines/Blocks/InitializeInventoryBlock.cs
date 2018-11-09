using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Internal;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Framework.Pipelines;

namespace Feature.Initialization.Engine.Pipelines.Blocks
{
    [PipelineDisplayName(HabitatConstants.Pipelines.Blocks.InitializeCatalogBlock)]
    public class InitializeInventoryBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        protected IHostingEnvironment HostingEnvironment { get; }
        protected ImportInventorySetsCommand ImportInventorySetsCommand { get; }

        public InitializeInventoryBlock(IHostingEnvironment hostingEnvironment, ImportInventorySetsCommand importInventorySetsCommand)
        {
            HostingEnvironment = hostingEnvironment;
            ImportInventorySetsCommand = importInventorySetsCommand;
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            const string artifactSet = "Environment.Habitat.Catalog-1.0";
            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
                return arg;

            using (var stream = new FileStream(GetPath("Habitat_Inventory.zip"), FileMode.Open, FileAccess.Read))
            {
                var file = new FormFile(stream, 0, stream.Length, stream.Name, stream.Name);
                await ImportInventorySetsCommand.Process(context.CommerceContext, file, CatalogConstants.ImportMode.Replace, 10);
            }

            return arg;
        }

        private string GetPath(string fileName)
        {
            return Path.Combine(HostingEnvironment.WebRootPath, "data", "Catalogs", fileName);
        }
    }
}