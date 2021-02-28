using Android.App;
using Autofac;
using eHub.Android.Models;
using eHub.Common.Api;
using eHub.Common.Models;
using eHub.Common.Services;
using System.IO;

namespace eHub.Android
{
    public class EhubModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<EhubInjector>()
                .SingleInstance()
                .AutoActivate();

            builder.Register(ctx =>
            {
                var packageInfo = Application.Context.PackageManager.GetPackageInfo(Application.Context.PackageName, 0);
                return new AppVersion { VersionName = packageInfo.VersionName, VersionNumber = packageInfo.LongVersionCode}; 
            }).As<AppVersion>();

            builder.Register(ctx =>
            {
                Configuration config = ctx.Resolve<Configuration>();
                return new WebInterface(config);
            })
            .As<IWebInterface>()
            .SingleInstance();

            builder.Register(ctx =>
            {
                IWebInterface webApi = ctx.Resolve<IWebInterface>();
                return new PoolApi(webApi);
            })
            .As<IPoolApi>()
            .SingleInstance();

            builder.Register(ctx =>
            {
                IPoolApi poolApi = ctx.Resolve<IPoolApi>();
                return new PoolService(poolApi);
            })
            .As<IPoolService>()
            .SingleInstance();
        }
    }
}