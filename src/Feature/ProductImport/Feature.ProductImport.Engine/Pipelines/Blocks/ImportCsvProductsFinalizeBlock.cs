using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class ImportCsvProductsFinalizeBlock : PipelineBlock<ImportCsvProductsArgument, ImportCsvProductsResult, CommercePipelineExecutionContext>
    {
        public override Task<ImportCsvProductsResult> Run(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(context, nameof(context)).IsNotNull();

            var errors = GetErrors(arg, context);
            var catalogImportResult = new ImportCsvProductsResult
            {
                Errors = errors,
                Succeeded = errors.Count < arg.ErrorThreshold,
                ResultCode = GetImportResultCode(arg, context, errors)
            };

            return Task.FromResult(catalogImportResult);
        }

        protected virtual string GetImportResultCode(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context, IList<CommandMessage> importErrors)
        {
            Condition.Requires(importErrors, nameof(importErrors)).IsNotNull();
            var count = importErrors.Count;

            if (count > arg.ErrorThreshold)
                return "failed";

            return count > 0 ? "completedwitherrors" : "succeeded";
        }

        private List<CommandMessage> GetErrors(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(context, nameof(context)).IsNotNull();
            return context.CommerceContext.GetMessages(m => string.Equals(context.GetPolicy<KnownResultCodes>().Error, m.Code, StringComparison.OrdinalIgnoreCase));
        }
    }
}
