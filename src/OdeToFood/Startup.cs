using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OdeToFood.Services;
using Microsoft.AspNet.Routing;
using OdeToFood.Entities;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.PlatformAbstractions;
using OdeToFood.Middleware;

namespace OdeToFood
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddEntityFramework()
                    .AddSqlServer()
                    .AddDbContext<OdeToFoodDbContext>(options => options.UseSqlServer(Configuration["database:connection"]));

            services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<OdeToFoodDbContext>();

            services.AddSingleton(provider => Configuration);
            services.AddSingleton<IGreeter, Greeter>();
            services.AddScoped<IRestaurantData, SqlRestaurantData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment environment,
            IApplicationEnvironment appEnvironment,
            IGreeter greeter)
        {
            app.UseIISPlatformHandler();

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDeveloperExceptionPage();
            app.UseRuntimeInfoPage("/info");

            //Points default to index.html in folder http://localhost/ serves index.html
            app.UseDefaultFiles();
            //Points default to file names (not folder specific)
            app.UseStaticFiles();

            app.UseFileServer();

            app.UseNodeModules(appEnvironment);

            app.UseIdentity();

            app.UseMvc(ConfigureRoute);

            app.Run(async (context) =>
            {
                var greeting = greeter.GetGreeting();
                await context.Response.WriteAsync(greeting);
            });
                    }

        private void ConfigureRoute(IRouteBuilder routeBuilder)
        {
            // pulls apart URL /Home/Index (home controller, index method)
            //{controller=home} default is home, ? returns is available
            //convention based routing            
            routeBuilder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");

        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
