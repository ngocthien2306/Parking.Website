using AutoMapper.Configuration;
using InfrastructureCore;
using InfrastructureCore.Modules;
using InfrastructureCore.Web.ModelBinders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace InfrastructureCore.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static readonly IModuleConfigurationManager _modulesConfig = new ModuleConfigurationManager();

        public static IServiceCollection AddModules(this IServiceCollection services, string contentRootPath)
        {
            // No need module.json at the moment. Consider put as embbeded resource if needed
            //const string moduleManifestName = "module.json";
            //var modulesFolder = Path.Combine(contentRootPath, "Modules");
            foreach (var module in _modulesConfig.GetModules())
            {
                //var moduleFolder = new DirectoryInfo(Path.Combine(modulesFolder, module.Id));
                //var moduleManifestPath = Path.Combine(moduleFolder.FullName, moduleManifestName);
                //if (!File.Exists(moduleManifestPath))
                //{
                //    throw new MissingModuleManifestException($"The manifest for the module '{moduleFolder.Name}' is not found.", moduleFolder.Name);
                //}

                //using (var reader = new StreamReader(moduleManifestPath))
                //{
                //    string content = reader.ReadToEnd();
                //    dynamic moduleMetadata = JsonConvert.DeserializeObject(content);
                //    module.Name = moduleMetadata.name;
                //}

                if (!module.IsActive)
                {
                    continue;
                }
                
                if (!module.IsBundledWithHost)
                {
                    TryLoadModuleAssembly(module.Id, module);
                    if (module.Assembly == null)
                    {
                        throw new Exception($"Cannot find main assembly for module {module.Id}");
                    }
                }
                else
                {
                    //module.Assembly = Assembly.Load(new AssemblyName(module.Id));

                    //const string binariesFolderName = "bin";
                    var binariesFolderName = Directory.GetCurrentDirectory();
                    //const string binariesFolderName = "E:/HLFXFrameworkModular/Publish";
                    var binariesFolderPath = Path.Combine(binariesFolderName);                    
                    var binariesFolder = new DirectoryInfo(binariesFolderPath);                   

                    Console.WriteLine(binariesFolder);
                    if (Directory.Exists(binariesFolderPath))
                    {
                        
                        foreach (var file in binariesFolder.GetFileSystemInfos(string.Format("{0}.dll", module.Id), SearchOption.AllDirectories))
                        {
                            
                            Assembly assembly;
                            try
                            {
                                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                                Console.WriteLine("qua line nay");
                            }
                            catch (FileLoadException)
                            {
                                // Get loaded assembly. This assembly might be loaded
                                assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

                                if (assembly == null)
                                {
                                    throw;
                                }

                                string loadedAssemblyVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
                                string tryToLoadAssemblyVersion = FileVersionInfo.GetVersionInfo(file.FullName).FileVersion;
                                Console.WriteLine("qua line nay1");
                                // Or log the exception somewhere and don't add the module to list so that it will not be initialized
                                if (tryToLoadAssemblyVersion != loadedAssemblyVersion)
                                {
                                    throw new Exception($"Cannot load {file.FullName} {tryToLoadAssemblyVersion} because {assembly.Location} {loadedAssemblyVersion} has been loaded");
                                }
                            }

                            if (Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name) == module.Id)
                            {
                                module.Assembly = assembly;
                                Console.WriteLine("qua line nay2");
                            }
                        }
                    }
                }

                GlobalConfiguration.Modules.Add(module);
            }

            return services;
        }

        private static void TryLoadModuleAssembly(string moduleFolderPath, ModuleInfo module)
        {
            const string binariesFolderName = "bin";
            var binariesFolderPath = Path.Combine(moduleFolderPath, binariesFolderName);
            var binariesFolder = new DirectoryInfo(binariesFolderPath);

            if (Directory.Exists(binariesFolderPath))
            {
                foreach (var file in binariesFolder.GetFileSystemInfos("*.dll", SearchOption.AllDirectories))
                {
                    Assembly assembly;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    }
                    catch (FileLoadException)
                    {
                        // Get loaded assembly. This assembly might be loaded
                        assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));

                        if (assembly == null)
                        {
                            throw;
                        }

                        string loadedAssemblyVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
                        string tryToLoadAssemblyVersion = FileVersionInfo.GetVersionInfo(file.FullName).FileVersion;

                        // Or log the exception somewhere and don't add the module to list so that it will not be initialized
                        if (tryToLoadAssemblyVersion != loadedAssemblyVersion)
                        {
                            throw new Exception($"Cannot load {file.FullName} {tryToLoadAssemblyVersion} because {assembly.Location} {loadedAssemblyVersion} has been loaded");
                        }
                    }

                    if (Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name) == module.Id)
                    {
                        module.Assembly = assembly;
                    }
                }
            }
        }

        public static IServiceCollection AddCustomizedDataStore(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContextPool<SimplDbContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            //        b => b.MigrationsAssembly("InfrastructureCore.Web")));
            return services;
        }

        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules)
        {
            var mvcBuilder = services
                .AddMvc(o =>
                {
                    o.EnableEndpointRouting = false;
                    o.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
                })
                //.AddRazorRuntimeCompilation()
                .AddViewLocalization()
                .AddModelBindingMessagesLocalizer(services)
                .AddDataAnnotationsLocalization(o =>
                {
                    var factory = services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                    //var L = factory.Create(null);
                    //o.DataAnnotationLocalizerProvider = (t, f) => L;
                })
                .AddNewtonsoftJson();

            //foreach (var module in modules.Where(x => !x.IsBundledWithHost))
            //{
            //    AddApplicationPart(mvcBuilder, module.Assembly);
            //}
            foreach (var module in modules.Where(x => x.IsBundledWithHost && x.IsActive))
            {
                if (null == module.Assembly)
                {
                    continue;
                }
                AddApplicationPart(mvcBuilder, module.Assembly);
            }

            return services;
        }

        /// <summary>
        /// Localize ModelBinding messages, e.g. when user enters string value instead of number...
        /// these messages can't be localized like data attributes
        /// </summary>
        /// <param name="mvc"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IMvcBuilder AddModelBindingMessagesLocalizer
            (this IMvcBuilder mvc, IServiceCollection services)
        {
            return mvc.AddMvcOptions(o =>
            {
                //var factory = services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
               // var L = factory.Create(null);

                //o.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => L["The value '{0}' is invalid.", x]);
                //o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor((x) => L["The field {0} must be a number.", x]);
                //o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) => L["A value for the '{0}' property was not provided.", x]);
                //o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => L["The value '{0}' is not valid for {1}.", x, y]);
                //o.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => L["A value is required."]);
                //o.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => L["A non-empty request body is required."]);
                //o.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) => L["The value '{0}' is not valid.", x]);
                //o.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => L["The value provided is invalid."]);
                //o.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => L["The field must be a number."]);
                //o.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) => L["The supplied value is invalid for {0}.", x]);
                //o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => L["Null value is invalid."]);
            });
        }

        private static void AddApplicationPart(IMvcBuilder mvcBuilder, Assembly assembly)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            foreach (var part in partFactory.GetApplicationParts(assembly))
            {
                mvcBuilder.PartManager.ApplicationParts.Add(part);
            }

            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: false);
            foreach (var relatedAssembly in relatedAssemblies)
            {
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(relatedAssembly);
                foreach (var part in partFactory.GetApplicationParts(relatedAssembly))
                {
                    mvcBuilder.PartManager.ApplicationParts.Add(part);
                }
            }
        }

        private static Task HandleRemoteLoginFailure(RemoteFailureContext ctx)
        {
            ctx.Response.Redirect("/login");
            ctx.HandleResponse();
            return Task.CompletedTask;
        }
    }
}
