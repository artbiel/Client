using Fluxor;

namespace Client.Store
{
    public record ArticleSidebarState(ArticleSidebarTab CurrentTab);

    public class ArticleSidebarFeature : Feature<ArticleSidebarState>
    {
        public override string GetName() => "ArticleSidebar";

        protected override ArticleSidebarState GetInitialState() => new(ArticleSidebarTab.Structure);
    }

    public static class ArticleSidebarReducers
    {
        [ReducerMethod]
        public static ArticleSidebarState ReduceToggleArticleSidebarTabAction(ArticleSidebarState state,
            ToggleArticleSidebarTabAction action) => new ArticleSidebarState(action.Tab);
    }

    public record ToggleArticleSidebarTabAction(ArticleSidebarTab Tab);

    public enum ArticleSidebarTab
    {
        Structure,
        History
    }

    public enum ArticleSidebarHistoryTab
    {
        Course,
        Article
    }
}