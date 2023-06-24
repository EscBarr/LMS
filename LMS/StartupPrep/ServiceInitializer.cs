using LMS.EntityСontext;
using LMS.Services.AuthService;
using LMS.Services;
using LMS.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LMS.Repos;
using LMS.EntityContext;
using LMS.Git;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authentication;
using LMS.Services.BasicAuth;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace LMS.StartupPrep
{
    public static partial class ServiceInitializer
    {
        public static IServiceCollection RegisterApplicationServices(
                                            this IServiceCollection services, IConfiguration configuration)
        {
            RegisterCustomDependencies(services, configuration);

            RegisterDBContextDependencies(services, configuration);

            RegisterAuthDependencies(services, configuration);

            //RegisterSwagger(services);
            return services;
        }

        private static void RegisterCustomDependencies(IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            //services.AddControllersWithViews();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddRazorPages();
            //services.AddRazorPages().AddRazorPagesOptions(options =>
            //{
            //    options.Conventions.AddPageRoute("/Courses/Manage",
            //                                     "courses/Manage/{id?}/{handler?}/{UserId?}");
            //});
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.PageViewLocationFormats.Add("/Pages/Courses/Partials/{0}" + RazorViewEngine.ViewExtension);
            });

            services.AddSingleton<IAuthService>(new AuthService(configuration));
            services.AddSingleton<IConfiguration>(configuration);
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddHttpClient();
            services.Configure<GitSettings>(configuration.GetSection(nameof(GitSettings)));
            services.AddScoped<CourseRepo>();
            services.AddScoped<GitService>();
            services.AddScoped<LabWorksRepo>();
            services.AddScoped<AssignedVariantsRepo>();
            services.AddScoped<UsersRepo>();
        }

        private static void RegisterDBContextDependencies(IServiceCollection services, IConfiguration configuration)
        {                                                                                                            //DockerConnection DefaultConnection
            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        private static void RegisterAuthDependencies(IServiceCollection services, IConfiguration configuration)
        {
            //ПАРАМЕТРЫ ТОКЕНА
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JwtIssuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JwtAudience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddBasic();

            //ПАРАМЕТРЫ ТОКЕНА
            //Параметры авторизации
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });
            //Параметры авторизации
        }

        private static void RegisterSwagger(IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }
}