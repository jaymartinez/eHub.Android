using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;

using static Android.Support.V4.App.FragmentManager;
using System.Threading.Tasks;
using eHub.Common.Services;
using eHub.Common.Api;
using System.Collections.Generic;
using eHub.Common.Models;
using System.Linq;
using Autofac;
using Android.Content.PM;
using Android.Content;
using Android.Util;
using Android.Views;
using eHub.Android.Fragments;

namespace eHub.Android
{
    [Activity(
        Label = "@string/app_name", 
        Theme = "@style/AppTheme", 
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var homeFrag = new HomeFragment();
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_container, homeFrag, "MainMenu")
                .Commit();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }
    }
}