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
using Android.Support.V4.Widget;

using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using eHub.Android.Listeners;
using System;

namespace eHub.Android
{
    [Activity(
        Label = "@string/app_name", 
        Theme = "@style/AppTheme", 
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        Toolbar _toolbar;
        DrawerLayout _drawer;
        ActionBarDrawerToggle _drawerToggle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_toolbar);

            _drawer = FindViewById<DrawerLayout>(Resource.Id.main_drawer_layout);
            _drawerToggle = new ActionBarDrawerToggle(this, _drawer, _toolbar, Resource.String.openDrawer, Resource.String.closeDrawer);
            _drawer.AddDrawerListener(_drawerToggle);
            _drawerToggle.DrawerIndicatorEnabled = true;
            _drawerToggle.SyncState();

            //_toolbar.SetNavigationOnClickListener(new OnClickListener(v => HandleBackNav()));

            var df = new DrawerFragment();
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_navigation_container, df, "MainMenu")
                .Commit();

            var hf = new HomeFragment();
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_container, hf, "Home")
                .Commit();

        }

        private void HandleBackNav()
        {
            //todo
        }
    }
}