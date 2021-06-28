using Fluxor;

namespace Client.Store
{
    public record RightSidebarState(bool Exists, bool IsDisplay);

    public class RightSidebarFeature : Feature<RightSidebarState>
    {
        public override string GetName() => "RightSidebar";

        protected override RightSidebarState GetInitialState() => new(false, false);
    }

    public static class RightSidebarReducers
    {
        [ReducerMethod]
        public static RightSidebarState ReduceToggleRightSidebarAction(RightSidebarState state, ToggleRightSidebarAction action) =>
            new RightSidebarState(state.Exists, !state.IsDisplay);

        [ReducerMethod]
        public static RightSidebarState ReduceRightSidebarExistsAction(RightSidebarState state, RightSidebarExistsAction action) =>
            new RightSidebarState(action.Exists, state.IsDisplay);
    }

    public record ToggleRightSidebarAction();

    public record RightSidebarExistsAction(bool Exists);
}

