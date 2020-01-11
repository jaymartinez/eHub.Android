using System;
using Android.App;
using Android.Runtime;
using Autofac;

namespace eHub.Android
{
    [Application]
    public class MainApplication : Application
    {
        public static MainApplication Instance { get; private set; }

        static IContainer _container;

        public MainApplication(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Instance = this;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _container = ContainerConfig.Configure();
        }
    }
}