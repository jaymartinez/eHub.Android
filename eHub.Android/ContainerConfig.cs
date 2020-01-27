using Autofac;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using eHub.Common.Models;

namespace eHub.Android
{
    public class ContainerConfig
    {
        public IContainer Configure()
        {
            var builder = new ContainerBuilder();

            var config = GetAppConfiguration(builder);
            builder.RegisterInstance(config);
            builder.RegisterInstance(config.Environment);

            builder.RegisterModule(new EhubModule());

            return builder.Build();
        }

        Configuration GetAppConfiguration(ContainerBuilder builder)
        {
            var fileName = "eHub.Android.appsettings.json";

            if (fileName is null)
                throw new ArgumentNullException(nameof(fileName));

            using (var stream = GetResourceStream(fileName))
            {
                var config = GetConfig(stream);

                //if (Uri.IsWellFormedUriString(config.Environment.ApiBaseRoute, UriKind.Absolute))
                if (config == null || config.Environment == null || string.IsNullOrEmpty(config.Environment.ApiBaseRoute))
                {
                    throw new Exception("config.ApiBaseRoute is not formatted correctly");
                }

                return config;
            }
        }

        Stream GetResourceStream(string filename)
        {
            var embeddedResourceStream = Assembly.GetAssembly(GetType())
                .GetManifestResourceStream(filename);

            if (embeddedResourceStream == null)
                throw new Exception($"Could not find {filename}.");

            return embeddedResourceStream;
        }

        Configuration GetConfig(Stream resourceStream)
        {
            if (resourceStream is null)
                throw new ArgumentNullException(nameof(resourceStream));

            using (var streamReader = new StreamReader(resourceStream))
            {
                var json = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<Configuration>(json);
            }
        }
    }
}