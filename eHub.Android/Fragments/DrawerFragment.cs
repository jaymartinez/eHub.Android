
using Android.OS;
using Android.Support.V4.Widget;
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
            return inflater.Inflate(Resource.Layout.fragment_drawer, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("Home", Resource.Drawable.ic_home_blue_dark_48dp, MenuType.Home, StringConstants.Tag_Home),
                new MenuItem("Schedule", Resource.Drawable.ic_date_range_blue_dark_48dp, MenuType.PoolSchedule, StringConstants.Tag_PoolSchedule),
                new MenuItem("Pool", Resource.Drawable.ic_pool_blue_dark_48dp, MenuType.Pool, StringConstants.Tag_PoolControl),
                new MenuItem("Spa", Resource.Drawable.ic_hot_tub_blue_dark_48dp, MenuType.Spa, StringConstants.Tag_SpaControl),
                new MenuItem("Heater", Resource.Drawable.ic_graphic_eq_blue_dark_48dp, MenuType.Heater, StringConstants.Tag_Heater),
                new MenuItem("Booster Pump", Resource.Drawable.ic_dialpad_blue_dark_48dp, MenuType.BoosterPump, StringConstants.Tag_BoosterPump),
                new MenuItem("About", Resource.Drawable.ic_help_outline_blue_dark_48dp, MenuType.About, StringConstants.Tag_About)
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
                case MenuType.PoolSchedule:
                    frag = new PoolScheduleFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
                case MenuType.Pool:
                    frag = new PoolControlFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
                case MenuType.Spa:
                    frag = new SpaControlFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
                case MenuType.Heater:
                    frag = new HeaterFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
                case MenuType.Home:
                    frag = new HomeFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
                case MenuType.BoosterPump:
                    frag = new BoosterFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
                case MenuType.About:
                    frag = new AboutFragment();
                    ((MainActivity)Activity).Push(frag, menuItem.Tag);
                    break;
            }
        }
    }
}