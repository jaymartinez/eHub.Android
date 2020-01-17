
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using eHub.Android.Models;
using System;
using System.Collections.Generic;

using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class DrawerFragment : Fragment
    {
        RecyclerView _recyclerView;
        MainMenuAdapter _adapter;

        public WeakReference<DrawerLayout> Drawer { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var act = Activity as AppCompatActivity;
            var actionBar = act.SupportActionBar;

            //HasOptionsMenu = true;
            return inflater.Inflate(Resource.Layout.fragment_drawer, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("Pool", Resource.Drawable.ic_pool_blue_dark_48dp, MenuType.Pool, "pool"),
                new MenuItem("Spa", Resource.Drawable.ic_hot_tub_blue_dark_48dp, MenuType.Spa, "spa")
            };

            _adapter = new MainMenuAdapter(items);

            _adapter.MenuTapped = OnMenuTap;

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.main_menu_recycler_view);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            _recyclerView.AddItemDecoration(new DividerItemDecoration(Context, LinearLayoutManager.Vertical));
            _recyclerView.SetAdapter(_adapter);
        }


        void OnMenuTap(MenuItem menuItem)
        {
            if (Drawer.TryGetTarget(out var drawer))
            {
                Activity.RunOnUiThread(() =>
                {
                    drawer.CloseDrawers();
                });
            }

            Fragment frag = null;
            switch (menuItem.MenuType)
            {
                case MenuType.Pool:
                    frag = new PoolFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
                case MenuType.Spa:
                    break;
            }

            if (frag == null)
                return;

            //Activity
            //    .SupportFragmentManager
            //    .BeginTransaction()
            //    .Replace(Resource.Id.main_container, frag, "Pool")
            //    .AddToBackStack("Pool")
            //    .Commit();
        }
    }
}