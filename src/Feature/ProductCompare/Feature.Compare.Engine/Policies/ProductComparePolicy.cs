using Sitecore.Commerce.Core;

namespace Feature.Compare.Engine.Policies
{
    public class ProductComparePolicy : Policy
    {
        public int MaxCompareCount { get; set; }
        public Settings.CompareFullAction CompareFullAction { get; set; }

        public ProductComparePolicy()
        {
            MaxCompareCount = 3;
            CompareFullAction = Settings.CompareFullAction.RemoveOldest;
        }
    }
}