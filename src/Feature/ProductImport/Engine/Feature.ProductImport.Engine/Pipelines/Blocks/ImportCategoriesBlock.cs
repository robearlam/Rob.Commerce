using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Feature.ProductImport.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Feature.ProductImport.Engine.Pipelines.Blocks
{
    public class ImportCategoriesBlock : PipelineBlock<ImportCsvProductsArgument, ImportCsvProductsArgument, CommercePipelineExecutionContext>
    {
        private readonly CreateCategoryCommand _createCategoryCommand;
        private readonly AssociateCategoryToParentCommand _associateCategoryToParentCommand;
        private const int CategoryIndex = 12;
        private const int CatalogNameIndex = 10;

        public ImportCategoriesBlock(CreateCategoryCommand createCategoryCommand, AssociateCategoryToParentCommand associateCategoryToParentCommand)
        {
            _createCategoryCommand = createCategoryCommand;
            _associateCategoryToParentCommand = associateCategoryToParentCommand;
        }

        public override async Task<ImportCsvProductsArgument> Run(ImportCsvProductsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg, nameof(arg)).IsNotNull();
            Condition.Requires(arg.FileLines, nameof(arg.FileLines)).IsNotNull();

            var categories = GetCategoriesToImport(arg);
            await CreateCategories(context, categories);
            await CreateCategoryRelationships(context, categories);

            return arg;
        }

        private async Task CreateCategoryRelationships(CommercePipelineExecutionContext context, Dictionary<string, Tuple<string, string, string>> categories)
        {
            foreach (var category in categories.Values)
            {
                await _associateCategoryToParentCommand.Process(context.CommerceContext, $"{CommerceEntity.IdPrefix<Catalog>()}{category.Item3}", category.Item2, $"{CommerceEntity.IdPrefix<Category>()}{category.Item3}-{category.Item1}");
            }
        }

        private async Task CreateCategories(CommercePipelineExecutionContext context, Dictionary<string, Tuple<string, string, string>> categories)
        {
            foreach (var category in categories.Values)
            {
                await _createCategoryCommand.Process(context.CommerceContext, category.Item3, category.Item1, category.Item1, category.Item1);
            }
        }

        private static Dictionary<string, Tuple<string, string, string>> GetCategoriesToImport(ImportCsvProductsArgument arg)
        {
            var categoriesToImport = new Dictionary<string, Tuple<string, string, string>>();
            foreach (var line in arg.FileLines)
            {
                var catalogName = line.Split(',')[CatalogNameIndex];
                var categoryData = line.Split(',')[CategoryIndex];
                var categories = categoryData.Split('|');
                for (var i = 0; i < categories.Length; i++)
                {
                    var categoryName = categories[i];
                    if (categoriesToImport.ContainsKey(categoryName))
                        continue;

                    var parentId = i == 0 ? $"{CommerceEntity.IdPrefix<Catalog>()}{catalogName}" : $"{CommerceEntity.IdPrefix<Category>()}{catalogName}-{categories[i - 1]}";
                    var category = new Tuple<string, string, string>(categoryName, parentId, catalogName);
                    categoriesToImport.Add(categoryName, category);
                }
            }
            return categoriesToImport;
        }
    }
}