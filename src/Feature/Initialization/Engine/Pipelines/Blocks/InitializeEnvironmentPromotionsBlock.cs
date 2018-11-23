using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Coupons;
using Sitecore.Commerce.Plugin.Promotions;
using Sitecore.Commerce.Plugin.Rules;
using Sitecore.Framework.Pipelines;

namespace Feature.Initialization.Engine.Pipelines.Blocks
{
    [PipelineDisplayName("Habitat.InitializeEnvironmentPromotionsBlock")]
    public class InitializeEnvironmentPromotionsBlock : PipelineBlock<string, string, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;
        private readonly IAddPromotionBookPipeline _addBookPipeline;
        private readonly IAddPromotionPipeline _addPromotionPipeline;
        private readonly IAssociateCatalogToBookPipeline _associateCatalogToBookPipeline;
        private readonly IAddQualificationPipeline _addQualificationPipeline;
        private readonly IAddBenefitPipeline _addBenefitPipeline;
        private readonly IAddPublicCouponPipeline _addPublicCouponPipeline;

        public InitializeEnvironmentPromotionsBlock(IAddPromotionBookPipeline addBookPipeline, IAssociateCatalogToBookPipeline associateCatalogToBookPipeline, IAddPromotionPipeline addPromotionPipeline, IAddQualificationPipeline addQualificationPipeline, IAddBenefitPipeline addBenefitPipeline, IPersistEntityPipeline persistEntityPipeline, IAddPublicCouponPipeline addPublicCouponPipeline)
        {
            _addBookPipeline = addBookPipeline;
            _associateCatalogToBookPipeline = associateCatalogToBookPipeline;
            _addPromotionPipeline = addPromotionPipeline;
            _addQualificationPipeline = addQualificationPipeline;
            _addBenefitPipeline = addBenefitPipeline;
            _persistEntityPipeline = persistEntityPipeline;
            _addPublicCouponPipeline = addPublicCouponPipeline;
        }

        public override async Task<string> Run(string arg, CommercePipelineExecutionContext context)
        {
            const string artifactSet = "Environment.Habitat.Promotions-1.0";

            if (!context.GetPolicy<EnvironmentInitializationPolicy>().InitialArtifactSets.Contains(artifactSet))
                return arg;

            context.Logger.LogInformation($"{Name}.InitializingArtifactSet: ArtifactSet={artifactSet}");
            var addPromotionBookArgument = new AddPromotionBookArgument("Habitat_PromotionBook")
            {
                DisplayName = "Habitat Promotion Book",
                Description = "This is the Habitat promotion book"
            };

            var book = await _addBookPipeline.Run(addPromotionBookArgument, context);
            await Create10OffPreviousPurchasesPromotion(book, context);
            await _associateCatalogToBookPipeline.Run(new CatalogAndBookArgument(book.Name, "Habitat_Master"), context);
            return arg;
        }

        private async Task Create10OffPreviousPurchasesPromotion(PromotionBook book, CommercePipelineExecutionContext context)
        {
            var addPromotionArgument = new AddPromotionArgument(book, "10OffPreviousPurchases", DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(1), "10% off previous purchases", "10% off previous purchases")
            {
                DisplayName = "10% off previous purchases",
                Description = "10% discount on all items previously purchased in the past year",
                IsExclusive = true
            };
            var promotion = await _addPromotionPipeline.Run(addPromotionArgument,context);

            var conditionModel = new ConditionModel
            {
                Id = Guid.NewGuid().ToString(),
                LibraryId = Foundation.Rules.Engine.RulesConstants.Conditions.CurrentCustomerHasPurchasedCurrentProductPreviouslyCondition,
                Name = Foundation.Rules.Engine.RulesConstants.Conditions.CurrentCustomerHasPurchasedCurrentProductPreviouslyCondition,
                Properties = new List<PropertyModel>
                {
                    new PropertyModel { IsOperator = false, Name = "Days", Value = "365", DisplayType = "System.Int32" },
                }
            };
            promotion = await _addQualificationPipeline.Run(new PromotionConditionModelArgument(promotion,conditionModel), context);

            var actionModel = new ActionModel
            {
                Id = Guid.NewGuid().ToString(),
                LibraryId = Foundation.Rules.Engine.RulesConstants.Benefits.CartItemPreviouslyPurchasedPercentOffAction,
                Name = Foundation.Rules.Engine.RulesConstants.Benefits.CartItemPreviouslyPurchasedPercentOffAction
            };
            await _addBenefitPipeline.Run(new PromotionActionModelArgument(promotion,actionModel), context);

            promotion = await _addPublicCouponPipeline.Run(new AddPublicCouponArgument(promotion, "REPEAT10"), context);
            promotion.SetComponent(new ApprovalComponent(context.GetPolicy<ApprovalStatusPolicy>().Approved));
            await _persistEntityPipeline.Run(new PersistEntityArgument(promotion), context);
        }
    }
}