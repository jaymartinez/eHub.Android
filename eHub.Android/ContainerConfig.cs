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

            builder.RegisterModule(new EhubModule());

            return builder.Build();
        }
    }
}