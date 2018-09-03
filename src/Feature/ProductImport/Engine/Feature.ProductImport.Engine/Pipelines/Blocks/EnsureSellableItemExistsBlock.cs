using System;
using System.Linq;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Mappers;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class EnsureSellableItemExistsBlock : PipelineBlock<ImportSingleCsvLineArgument, ImportSingleCsvLineResult, CommercePipelineExecutionContext>
    {
        private readonly ISellableItemMapper _sellableItemMapper;
        private readonly ICreateSellableItemPipeline _createSellableItemPipeline;
        private readonly IEditSellableItemPipeline _editSellableItemPipeline;

        public EnsureSellableItemExistsBlock(ISellableItemMapper sellableItemMapper, ICreateSellableItemPipeline createSellableItemPipeline, IEditSellableItemPipeline editSellableItemPipeline)
        {
            _sellableItemMapper = sellableItemMapper;
            _createSellableItemPipeline = createSellableItemPipeline;
            _editSellableItemPipeline = editSellableItemPipeline;
        }

        public override async Task<ImportSingleCsvLineResult> Run(ImportSingleCsvLineArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(arg.Line, nameof(arg.Line)).IsNotNull();

            var createSellableItemArg = _sellableItemMapper.MapToArg(arg.Line);
            var createResult = await _createSellableItemPipeline.Run(createSellableItemArg, context);
            var sellableItem = createResult.SellableItems.FirstOrDefault();
            if (sellableItem == null)
            {
                return new ImportSingleCsvLineResult
                {
                    Success = false
                };
            }

            sellableItem = _sellableItemMapper.MapToEntity(sellableItem, arg.Line);
            await _editSellableItemPipeline.Run(sellableItem, context);
            return new ImportSingleCsvLineResult
            {
                Success = true
            };
        }
    }
}
