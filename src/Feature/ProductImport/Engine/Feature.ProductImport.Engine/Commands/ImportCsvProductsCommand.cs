using System;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Microsoft.AspNetCore.Http;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Conditions;

namespace Feature.ProductImport.Engine.Commands
{
    public class ImportCsvProductsCommand : CommerceCommand
    {
        private readonly IImportCsvProductsPipeline _importCsvProductsPipeline;

        public ImportCsvProductsCommand(IImportCsvProductsPipeline importCsvProductsPipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Condition.Requires(importCsvProductsPipeline, nameof(importCsvProductsPipeline)).IsNotNull();
            _importCsvProductsPipeline = importCsvProductsPipeline;
        }

        public async Task<CommerceCommand> Process(CommerceContext commerceContext, IFormFile importFile, string mode, int errorThreshold, bool publishEntities = true)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var pipelineContextOptions = commerceContext.GetPipelineContextOptions();
                var catalogsArgument = new ImportCsvProductsArgument(importFile, mode)
                {
                    ErrorThreshold = errorThreshold
                };

                if (publishEntities)
                    commerceContext.Environment.SetPolicy(new AutoApprovePolicy());

                var importResult = await _importCsvProductsPipeline.Run(catalogsArgument, pipelineContextOptions);
                if (importResult != null)
                {
                    Messages.AddRange(importResult.Errors);
                    ResponseCode = importResult.ResultCode;
                }
                else
                    ResponseCode = commerceContext.GetPolicy<KnownResultCodes>().Error;

                if (publishEntities)
                    commerceContext.Environment.RemovePolicy(typeof(AutoApprovePolicy));
            }
            return this;
        }
    }
}
