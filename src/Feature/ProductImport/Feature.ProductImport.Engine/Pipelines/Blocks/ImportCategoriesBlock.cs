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

        private async Task CreateCategoryRelationships(CommercePipelineExecutionContext context, List<(string CategoryName, string CatalogName, string ParentId)> categories)
        {
            foreach (var category in categories)
            {
                await _associateCategoryToParentCommand.Process(context.CommerceContext, GenerateFullCatalogName(category.CatalogName), category.ParentId, GenerateFullCategoryId(category.CatalogName, category.CategoryName));
            }
        }

        private async Task CreateCategories(CommercePipelineExecutionContext context, List<(string CategoryName, string CatalogName, string ParentId)> categories)
        {
            foreach (var category in categories)
            {
                await _createCategoryCommand.Process(context.CommerceContext, category.CatalogName, category.CategoryName, category.CategoryName, category.CategoryName);
            }
        }

        private static List<(string CategoryName, string CatalogName, string ParentId)> GetCategoriesToImport(ImportCsvProductsArgument arg)
        {
            var categoriesToImport = new List<(string CategoryName, string CatalogName, string ParentId)>();
            foreach (var line in arg.FileLines)
            {
                for (var i = 0; i < line.Categories.Count; i++)
                {
                    var categoryName = line.Categories[i];
                    if (categoriesToImport.Exists(c => c.CategoryName == categoryName))
                        continue;

                    categoriesToImport.Add(GenerateCategoryTuple(i, line, categoryName));
                }
            }
            return categoriesToImport;
        }

        private static (string CategoryName, string CatalogName, string ParentId) GenerateCategoryTuple(int i, CsvImportLine line, string categoryName)
        {
            var isTopLevelCategory = i == 0;
            var parentCategory = isTopLevelCategory
                                ? line.FullEntityCatalogName
                                : GenerateFullCategoryId(line.CatalogName, line.Categories[i - 1]);

            var category = (CategoryName: categoryName,
                            CatalogName: line.CatalogName,
                            ParentId: parentCategory);

            return category;
        }

        private static string GenerateFullCatalogName(string catalogName)
        {
            return catalogName.ToEntityId<Catalog>();
        }

        private static string GenerateFullCategoryId(string catalogName, string categoryName)
        {
            return categoryName.ToCategoryId(catalogName);
        }
    }
}