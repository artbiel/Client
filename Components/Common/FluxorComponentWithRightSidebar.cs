using Client.Store;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Client.Components.Common
{
    public class FluxorComponentWithRightSidebar : FluxorComponent
    {
        [Inject]
        public IDispatcher Dispatcher { get; set; }

        protected override void OnInitialized()
        {
            Dispatcher.Dispatch(new RightSidebarExistsAction(true));
            base.OnInitialized();
        }

        protected override void Dispose(bool disposing)
        {
            Dispatcher.Dispatch(new RightSidebarExistsAction(false));
            base.Dispose(disposing);
        }
    }
}
