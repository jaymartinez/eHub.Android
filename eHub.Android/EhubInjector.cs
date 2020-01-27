using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;

namespace eHub.Android
{
    public class EhubInjector
    {
        static EhubInjector _instance;
        readonly ILifetimeScope _container;

        public EhubInjector(ILifetimeScope container)
        {
            _container = container;
            _instance = this;
        }

        public static void InjectProperties<T>(T instance)
        {
            _instance._container.InjectProperties(instance, new ServicePropertySelector());
        }

        public static void InjectMethod<T>(T instance)
            where T : class
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            var methodBindingFlags = BindingFlags.InvokeMethod
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly
                | BindingFlags.Instance;

            var instanceType = typeof(T);
            var method = instanceType.GetMethods(methodBindingFlags)
                .FirstOrDefault(x => x.GetCustomAttribute(typeof(InjectAttribute), false) != null);

            if (method is null)
                throw new ArgumentException($"Instance type of {instanceType.Name} does not contain a method to inject.");

            var methodParameters = method.GetParameters();

            if (methodParameters == null || methodParameters.Length == 0)
                return;

            var parameters = method.GetParameters()
                .Select(prm => _instance._container.Resolve(prm.ParameterType))
                .ToArray();

            method.Invoke(instance, parameters);
        }

        public static Task InjectMethodAsync<T>(T instance)
            where T : class
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            var methodBindingFlags = BindingFlags.InvokeMethod
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly
                | BindingFlags.Instance;

            var instanceType = instance.GetType();
            var method = instanceType.GetMethods(methodBindingFlags)
                .FirstOrDefault(x => x.GetCustomAttribute(typeof(InjectAttribute), false) != null);

            if (method is null)
                throw new ArgumentException($"Instance type of {instanceType.Name} does not contain a method to inject.");

            var methodParameters = method.GetParameters();

            if (methodParameters == null || methodParameters.Length == 0)
                return Task.CompletedTask;

            var parameters = method.GetParameters()
                .Select(prm => _instance._container.Resolve(prm.ParameterType))
                .ToArray();

            var result = method.Invoke(instance, parameters);

            if (result is Task t)
                return t;

            return Task.CompletedTask;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {

    }
}