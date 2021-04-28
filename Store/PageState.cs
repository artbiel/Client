using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Store
{
    public class PageState
    {
        public string Title { get; }

        public PageState(string title)
        {
            Title = title ?? "";
        }
    }

    public class PageFeature : Feature<PageState>
    {
        public override string GetName() => "Page";

        protected override PageState GetInitialState() => new(null);
    }

    public static class PageStateReducers
    {
        [ReducerMethod]
        public static PageState ReduceSetPageTitleAction(PageState state, SetPageTitleAction action) => new(action.Title);

        [ReducerMethod]
        public static PageState ReduceClearPageTitleAction(PageState state, ClearPageTitleAction action) => new(null);
    }

    public class SetPageTitleAction
    {
        public string Title { get; }

        public SetPageTitleAction(string title)
        {
            Title = title;
        }
    }

    public class ClearPageTitleAction { }
}
