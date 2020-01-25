using Autofac;
using eHub.Common.Api;
using eHub.Common.Services;

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
        }
    }
}