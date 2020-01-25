using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Autofac.Core;
using eHub.Android.Fragments;
using eHub.Android.Models;
using System;
using System.Collections.Generic;
using static Android.Support.V4.App.FragmentManager;
using ActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Fragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace eHub.Android
{
    [Activity(
        Label = "@string/app_name",
        Theme = "@style/AppTheme",
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private MenuItem _currentRoot;
        bool MenuPressed { get; set; }
        bool _doubleBackPress = false;
        Toolbar _toolbar;
        DrawerLayout _drawer;
        ActionBarDrawerToggle _drawerToggle;
        Dictionary<string, Fragment> _pageMappings = new Dictionary<string, Fragment>();

        public static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
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

            var df = new DrawerFragment();
            df.Drawer = new WeakReference<DrawerLayout>(_drawer);
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_navigation_container, df, "MainMenu")
                .Commit();

            var hf = new HomeFragment();
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_container, hf, "home")
                .Commit();

            SetRoot(new MenuItem("Home", Resource.Drawable.ic_device_hub_blue_dark_48dp, MenuType.Home, "home"));
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount == 1)
            {
                if (_drawer.IsDrawerOpen((int)GravityFlags.Start))
                {
                    _drawer.CloseDrawers();
                }
                else
                {
                    if (_currentRoot.MenuType == MenuType.Home)
                        return;

                    SetRoot(new MenuItem("Home", Resource.Drawable.ic_device_hub_blue_dark_48dp, MenuType.Home, "home"));
                }
            }
            else if (SupportFragmentManager.BackStackEntryCount == 0)
            {
                // This would be in the case that we are on the login page
                return;
            }
            else
            {
                Pop();
            }
        }

        public void Push(Fragment fragment, string tag)
        {
            GC.Collect();
            RunOnUiThread(() =>
            {
                SupportFragmentManager
                    .BeginTransaction()
                    .SetTransition((int)FragmentTransit.FragmentOpen)
                    .Replace(Resource.Id.main_container, fragment, tag)
                    .AddToBackStack(tag)
                    .Commit();
            });
        }

        public void Pop(int count = 1)
        {
            GC.Collect();
            var removal = SupportFragmentManager.GetBackStackEntryAt(SupportFragmentManager.BackStackEntryCount - count);
            RunOnUiThread(() =>
            {
                SupportFragmentManager.PopBackStackImmediate(removal.Name, PopBackStackInclusive);
            });
        }

        /// <summary>
        /// Asks the container for the type passed in and returns the instance.
        /// </summary>
        /// <param name="instance"></param>
        /*
        public static R ResolveServiceForFragment<T,R>(Fragment instance, T objType)
        {
            var container = MainApplication.Instance.Container;
            var app = MainApplication.Instance;
            //MainApplication.Cont
            //_container.InjectProperties(instance, new ServicePropertySelector());
        }
        */

        public void SetRoot(MenuItem menuItem)
        {
            GC.Collect();

            RunOnUiThread(() =>
            {
                _drawer.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
                _drawer.CloseDrawers();
                SupportActionBar.Show();

                if (menuItem == null)
                    menuItem = new MenuItem("Home", Resource.Drawable.ic_pool_blue_dark_48dp, MenuType.Pool, "pool");

                // Don't do anything if the user selects the current page.
                if (_currentRoot != null && menuItem.MenuType == _currentRoot.MenuType)
                    return;

                var page = GetFragmentForType(menuItem.MenuType);

                if (SupportFragmentManager.BackStackEntryCount > 0 && _currentRoot != null)
                {
                    SupportFragmentManager.PopBackStackImmediate(_currentRoot.Tag, PopBackStackInclusive);
                }

                SupportFragmentManager.ExecutePendingTransactions();

                var tx = SupportFragmentManager
                    .BeginTransaction()
                    .SetTransition((int)FragmentTransit.FragmentOpen);

                tx.Replace(Resource.Id.main_container, page, menuItem.Tag)
                  .AddToBackStack(menuItem.Tag)
                  .Commit();

                _currentRoot = menuItem;
            });
        }

        void StorePageMapping(string name, Fragment page)
        {
            if (_pageMappings.ContainsKey(name))
                _pageMappings[name] = page;
            else
                _pageMappings.Add(name, page);
        }

        Fragment GetFragmentForType(MenuType type)
        {
            switch (type)
            {
                case MenuType.PoolSchedule:
                    return new PoolScheduleFragment();
                case MenuType.Pool:
                    return new PoolControlFragment();
                case MenuType.Home:
                    return new HomeFragment();
            }

            throw new ArgumentException("Unknown menu type");
        }
    }
}