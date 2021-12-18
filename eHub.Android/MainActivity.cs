using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using eHub.Android.Fragments;
using System;
using Google.Android.Material.BottomNavigation;
using static AndroidX.Fragment.App.FragmentManager;
using Fragment = AndroidX.Fragment.App.Fragment;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Android.Views;

namespace eHub.Android
{
    [Activity(
        Icon = "@drawable/icon",
        Label = "@string/app_name",
        Theme = "@style/splash_theme",
        MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        bool _doubleBackPress;

        public static MainActivity Instance { get; private set; }
        static BottomNavigationView _bottomNavigation;
        static Toolbar _toolbar;

        protected override void OnStart()
        {
            base.OnStart();
            ((AppCompatActivity)this).SupportActionBar?.Hide();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_toolbar);
            _bottomNavigation = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            _bottomNavigation.ItemSelected += OnMenuItemSelected;

            LoadFragment(Resource.Id.menu_home_button);
        }

        void LoadFragment(int id)
        {
            var frag = SupportFragmentManager.BeginTransaction();
            switch (id)
            {
                case Resource.Id.menu_home_button:
                    var hf = new HomeFragment();
                    frag.Replace(Resource.Id.main_container, hf);
                    break;
                case Resource.Id.menu_equipment_button:
                    var ef = new EquipmentFragment();
                    frag.Replace(Resource.Id.main_container, ef);
                    break;
            }
            frag.Commit();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_save:
                    Console.WriteLine("Appbar save button tapped!!!");
                    break;
            }

            return false;
        }

        void OnMenuItemSelected (object sender, Google.Android.Material.Navigation.NavigationBarView.ItemSelectedEventArgs e)
        {
            LoadFragment(e.Item.ItemId);
        }

        public override void OnBackPressed()
        {
            if (_doubleBackPress)
            {
                FinishAffinity();
            }

            _doubleBackPress = true;
            Toast.MakeText(this, "Press BACK again to exit app", ToastLength.Short).Show();

            var handler = new Handler();
            Action action = () =>
            {
                _doubleBackPress = false;
            };
            handler.PostDelayed(action, 2000);
        }

        public void PopToRoot()
        {
            if (SupportFragmentManager.BackStackEntryCount == 0)
                return;

            RunOnUiThread(() =>
            {
                var firstEntry = SupportFragmentManager.GetBackStackEntryAt(0);
                SupportFragmentManager.PopBackStackImmediate(firstEntry.Id, 0);
            });
        }

        public void Push(Fragment fragment, string tag)
        {
            GC.Collect();
            PopToRoot();

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
    }
}