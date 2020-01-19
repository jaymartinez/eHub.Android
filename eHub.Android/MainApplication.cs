using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Autofac;

namespace eHub.Android
{
    [Application]
    public class MainApplication : Application
    {
        public static MainApplication Instance { get; private set; }

        static IContainer _container;
        public IContainer Container => _container;

        public MainApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Instance = this;
        }

        public static MainApplication GetInstance(Context context)
        {
            return (MainApplication)context.ApplicationContext;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _container = ContainerConfig.Configure();
        }
    }
}