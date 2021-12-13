using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using eHub.Android.Fragments;
using System;
using static AndroidX.Fragment.App.FragmentManager;
using Fragment = AndroidX.Fragment.App.Fragment;

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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var hf = new HomeFragment();
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.main_container, hf, "home")
                .Commit();
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