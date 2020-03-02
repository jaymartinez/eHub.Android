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
                //TODO read from manifest
                return new AppVersion { VersionName = "1.2.1", VersionNumber = 1214 }; 
            }).As<AppVersion>();

            builder.Register(ctx =>
            {
                var config = ctx.Resolve<Configuration>();
                return new WebInterface(config);
            })
            .As<IWebInterface>()
            .SingleInstance();

            builder.Register(ctx =>
            {
                var webApi = ctx.Resolve<IWebInterface>();
                return new PoolApi(webApi);
            })
            .As<IPoolApi>()
            .SingleInstance();

            builder.Register(ctx =>
            {
                var poolApi = ctx.Resolve<IPoolApi>();
                return new PoolService(poolApi);
            })
            .As<IPoolService>()
            .SingleInstance();
        }
    }
}