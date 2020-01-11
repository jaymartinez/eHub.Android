using Autofac;
using eHub.Common.Api;
using eHub.Common.Services;
using System.Linq;
using System.Reflection;

namespace eHub.Android
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WebInterface>()
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

            return builder.Build();
        }
    }
}