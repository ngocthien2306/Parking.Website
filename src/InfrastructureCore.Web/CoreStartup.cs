using AutoMapper;
using InfrastructureCore.DAL;
using InfrastructureCore.Modules;
using InfrastructureCore.Web.Data;
using InfrastructureCore.Web.Extensions;
using InfrastructureCore.Web.LocalizationResources;
using InfrastructureCore.Web.Mappers;
using InfrastructureCore.Web.Provider;
using InfrastructureCore.Web.Services.IService;
using InfrastructureCore.Web.Services.ServiceImpl;
using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using Microsoft.IdentityModel.Tokens;
using Modules.FileUpload.Mappers;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace InfrastructureCore.Web
{
    public class CoreStartup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public CoreStartup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            _configuration.GetSection("ConnectionStrings").Bind(GlobalConfiguration.DbConnections);

            GlobalConfiguration.WebRootPath = _hostingEnvironment.WebRootPath;
            GlobalConfiguration.ContentRootPath = _hostingEnvironment.ContentRootPath;
            services.AddModules(_hostingEnvironment.ContentRootPath);

            services.AddCors();

            //compression
            services.AddResponseCompression();
            //add options feature
            services.AddOptions();
            //add HTTP sesion state feature
            //services.AddHttpSession();
            //services.AddSession(options =>
            //{
            //    options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.SessionCookie}";
            //    options.Cookie.HttpOnly = true;

            //    //whether to allow the use of session values from SSL protected page on the other store pages which are not
            //    options.Cookie.SecurePolicy = DataSettingsManager.DatabaseIsInstalled && EngineContext.Current.Resolve<IStoreContext>().CurrentStore.SslEnabled
            //        ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.None;
            //});

            //add localization
            services.AddLocalization();


            // Add framework services.
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });



            //services.AddSession(opts =>
            //{
            //    opts.Cookie.IsEssential = true; // make the session cookie Essential
            //    opts.IdleTimeout = TimeSpan.FromHours(1);
            //});
            //var sitecode = _configuration.GetSection("MESSiteStartup").GetSection("SiteCode").Value;
            //services.ConfigureSession(new SessionService(sitecode));
            services.AddDistributedMemoryCache();
            short sessionTimeOut = short.Parse(_configuration.GetSection("MESSiteStartup").GetSection("SessionTimeOut").Value.ToString());
            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true; // make the session cookie Essential
                opts.IdleTimeout = TimeSpan.FromMinutes(500);
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(GlobalConfiguration.DbConnections.FrameworkConnection.ConnectionString));
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                  //.AddCookie(options =>
                  //{
                  //    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                  //}
                  .AddJwtBearer(options =>
                  {
                      options.SaveToken = true;
                      options.RequireHttpsMetadata = false;
                      options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          //ValidAudience = "https://www.yogihosting.com",
                          //ValidIssuer = "https://www.yogihosting.com",
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MynameisJamesBond007"))
                      };
                  })
                  .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                  {
                      //options.AccessDeniedPath = new PathString("/en/Account/Access");
                      options.Cookie = new CookieBuilder
                      {
                          //Domain = "",
                          HttpOnly = true,
                          Name = ".aspNetCoreDemo.Security.Cookie",
                          Path = "/",
                          SameSite = SameSiteMode.Lax,
                          SecurePolicy = CookieSecurePolicy.SameAsRequest
                      };
                      options.Events = new CookieAuthenticationEvents
                      {
                          OnSignedIn = context =>
                          {
                              Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                  "OnSignedIn", context.Principal.Identity.Name);
                              return Task.CompletedTask;
                          },
                          OnSigningOut = context =>
                          {
                              Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                  "OnSigningOut", context.HttpContext.User.Identity.Name);
                              return Task.CompletedTask;
                          },
                          OnValidatePrincipal = context =>
                          {
                              Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                  "OnValidatePrincipal", context.Principal.Identity.Name);
                              return Task.CompletedTask;
                          }
                      };
                      options.ExpireTimeSpan = TimeSpan.FromDays(7);
                      //options.LoginPath = new PathString("/en/MESAccount/MESLogin");
                      options.LoginPath = new PathString("/MESAccount/MESLogin");
                      //options.LoginPath = new PathString("/MESHome");
                      //options.AccessDeniedPath = new PathString("/MESHome");
                      options.ReturnUrlParameter = "RequestPath";
                      options.SlidingExpiration = true;
                  }
            );

            services.AddScoped<IUserManager, UserManager>();            
            services.AddHttpClient();
            services.AddCustomizedMvc(GlobalConfiguration.Modules);
            services.Configure<RazorViewEngineOptions>(
                options => { options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander()); });
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddTransient<IRazorViewRenderer, RazorViewRenderer>();
            services.AddTransient<ISessionService, SessionService>();

            services.AddControllersWithViews();
           
            services.AddRazorPages(); 
            services.AddMvc();

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-Token");

            // register Database Connection
            services.AddTransient<IDBContextConnection, DBContextConnection>();
            //services.AddTransient<IUserService, UserService>();

            foreach (var module in GlobalConfiguration.Modules)
            {
                if (module.Assembly == null)
                {
                    continue;
                }
                var moduleInitializerType = module.Assembly.GetTypes()
                   .FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
                if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                {
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
                    services.AddSingleton(typeof(IModuleInitializer), moduleInitializer);
                    moduleInitializer.ConfigureServices(services);
                }
            }
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Auto Mapper Configurations
            // services.AddAutoMapper(typeof(CoreStartup));
            services.AddAutoMapper(typeof(MappingConfig), typeof(FileMapConfig));

            //string secretKey = "mysite_supersecret_secretkey!8050";//line 1

            //SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));//line 2
            
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //        .AddJwtBearer(options =>
            //        {
            //            //options.Audience = "/MESHome";
            //            //options.Authority = "/MESHome";
            //            options.TokenValidationParameters = new TokenValidationParameters()
            //            {
            //                ValidateIssuerSigningKey = true,
            //                IssuerSigningKey = SigningKey,
            //                ValidateIssuer = false,
            //                ValidateAudience = false
            //            };
            //        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void CoreConfigure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();
            //app.ConfigureSession();

            //app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCustomizedStaticFiles(env);
            app.UseRequestLocalization();

            app.UseAuthorization();

            //// global cors policy
            //app.UseCors(x => x
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());

            //// custom jwt auth middleware
            //app.UseMiddleware<JwtMiddleware>();

            string secretKey = "mysite_supersecret_secretkey!8050";//line 1

            SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));//line 2

            //app.UseMiddleware<TokenProviderMiddleware>(Options.Create(new TokenProviderOptions
            //{
            //    SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256),
            //}));//line 3

            //app.AddAuthentication(new JwtBearerOptions
            //{
            //    //AutomaticAuthenticate = true,
            //    //AutomaticChallenge = true,
            //    Audience = "/MESHome",
            //    Authority = "/MESHome",
            //    TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = SigningKey,
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //    }
            //});//line 4
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                // name: "areas",
                // pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapControllerRoute(
                // name: "Admin",
                // pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture=ko}/{controller=MESAccount}/{action=MESLogin}/{id?}");

            });
             var moduleInitializers = app.ApplicationServices.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.Configure(app, env);
            }

        }
    }
}