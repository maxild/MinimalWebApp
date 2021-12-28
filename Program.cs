using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);

// NOTE: ConfigureServices and Configure (middleware pipeline) are not
// equally composable. They are in fact very different

// Add services to the container (side effects!!!!).
// 2-Phase setup (ConfigureServices phase):
//   1. Add dependencies to the DI container (add phase)
//   2. Call build to freeze the container (resolve phase, where dependency graph can be built efficiently)
builder.Services.AddControllers();
// The extension methods below are for a more bloated setup
// builder.Services.AddControllersWithViews();
// builder.Services.AddRazorPages();
// builder.Services.AddMvc(); // AddControllersWithViews + AddRazorPages
var app = builder.Build(); // here we (sort of) know what "types"(!!!) we need to resolve to run!!!!

// Configure the HTTP request pipeline.
// 1-Phase setup (Configure phase) that is more pure (no side effects, you can change previous/outer middleware 'before' you)

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseStaticFiles();

app.UseRouting();
// app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/hello/1", context =>
    {
        return context.Response.WriteAsync("Hello Delegate");
    });
    endpoints.MapControllerRoute("name", "/hello/2", new { controller = "Home", action = "Index" });
});

// app.MapRazorPages();

// Start the Host/WebApp
app.Run();

namespace MinimalWebApi
{
    // MVC has a bloated/complex state machine that run filters
    [AuthorizationFilter]
    [ResourceFilter]
    [ActionFilter] // You only need action filters...eventually in MVC+ (Houdini)
    [ResultFilter]
    [ExceptionFilter]
    public class HomeController : ControllerBase
    {
        public string Index() => "Hello Manual Action";
    }

    public class SomethingController : ControllerBase
    {
        [HttpGet("/hello/3")]
        public string Hello() => "Hello Attribute Route";
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizationFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context) => Task.CompletedTask;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ResourceFilterAttribute : Attribute, IAsyncResourceFilter
    {
        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next) => Task.CompletedTask;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) => Task.CompletedTask;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ResultFilterAttribute : Attribute, IAsyncResourceFilter
    {
        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next) => Task.CompletedTask;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ExceptionFilterAttribute : Attribute, IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context) => Task.CompletedTask;
    }
}
