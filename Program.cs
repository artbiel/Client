using Blazored.LocalStorage;
using Client.Infrastructure;
using Client.Services;
using Fluxor;
using Material.Blazor;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Local", options.ProviderOptions);
                options.AuthenticationPaths.LogInPath = "http:/localhost:6001/api/v1/account/login";
                options.AuthenticationPaths.LogOutPath = "http:/localhost:6001/api/v1/account/logout";
            });

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);
                options.UseReduxDevTools();
            });

            builder.Services.AddMBServices(
                toastServiceConfiguration: new MBToastServiceConfiguration()
                {
                    InfoDefaultHeading = "Info",
                    SuccessDefaultHeading = "Success",
                    WarningDefaultHeading = "Warning",
                    ErrorDefaultHeading = "Error",
                    Timeout = 3000,
                    MaxToastsShowing = 5,
                    Position = MBToastPosition.TopRight
                },

                animatedNavigationManagerServiceConfiguration: new MBAnimatedNavigationManagerServiceConfiguration()
                {
                    ApplyAnimation = true,
                    AnimationTime = 300
                },

                snackbarServiceConfiguration: new MBSnackbarServiceConfiguration()
                {
                    CloseMethod = MBNotifierCloseMethod.TimeoutAndDismissButton,
                    Leading = false,
                    Stacked = false,
                    Timeout = 5000
                }
            );

            builder.Services.AddSingleton<DragAndDropService>();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped<CourseService>();
            builder.Services.AddScoped<ArticleService>();
            builder.Services.AddScoped<WorkbanchService>();
            builder.Services.AddScoped<SupportService>();

            builder.Services.AddHttpClient("api")
               .AddHttpMessageHandler(sp =>
               {
                   var handler = sp.GetService<AuthorizationMessageHandler>()
                       .ConfigureHandler(
                           authorizedUrls: new[] { "https://localhost:7001" },
                           scopes: new[] { "CourseAPI" });

                   return handler;
               });

            builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("api"));

            builder.Services
                .AddOidcAuthentication(options =>
                {
                    builder.Configuration.Bind("oidc", options.ProviderOptions);
                    //options.UserOptions.RoleClaim = "role";
                });

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost/") });

            await builder.Build().RunAsync();
        }
    }
}
