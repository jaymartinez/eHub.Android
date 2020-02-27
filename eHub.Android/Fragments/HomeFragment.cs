using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Models;
using eHub.Common.Services;
using System.Collections.Generic;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener
    {
        TextView _poolStatusLbl;
        TextView _spaStatusLbl;
        TextView _boosterStatusLbl;
        TextView _heaterStatusLbl;

        ImageView _poolLightStatusBulb; 
        ImageView _spaLightStatusBulb; 
        ImageView _groundLightStatusBulb;

        Button _refreshButton;
        MainMenuAdapter _adapter;
        RecyclerView _recyclerView;
        SwipeRefreshLayout _refreshLayout;

        [Inject] IPoolService PoolService { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }

        public void OnRefresh()
        {
            _refreshLayout.Refreshing = false;
        }

        public override void OnStop()
        {
            base.OnStop();

            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Show();
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var statusLabel = view.FindViewById<TextView>(Resource.Id.home_status_label);
            var loadingDialog = Dialogs.SimpleAlert(Context, "Loading...", "", "");
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Hide();

            //loadingDialog.Show();
            if (await PoolService.Ping())
            {
                // do something
            }
            else
            {
                // do something else
            }
            //loadingDialog.Hide();

            var items = new List<MenuItem>
            {
                new MenuItem("Schedule", Resource.Drawable.ic_date_range_blue_dark_48dp, MenuType.PoolSchedule, StringConstants.Tag_PoolSchedule),
                new MenuItem("Pool", Resource.Drawable.ic_pool_blue_dark_48dp, MenuType.Pool, StringConstants.Tag_PoolControl),
                new MenuItem("Spa", Resource.Drawable.ic_hot_tub_blue_dark_48dp, MenuType.Spa, StringConstants.Tag_SpaControl),
                new MenuItem("Heater", Resource.Drawable.ic_graphic_eq_blue_dark_48dp, MenuType.Heater, StringConstants.Tag_Heater),
                new MenuItem("Booster Pump", Resource.Drawable.ic_dialpad_blue_dark_48dp, MenuType.BoosterPump, StringConstants.Tag_BoosterPump),
                new MenuItem("About", Resource.Drawable.ic_help_outline_blue_dark_48dp, MenuType.About, StringConstants.Tag_About)
            };

            _adapter = new MainMenuAdapter(items);

            _adapter.MenuTapped = OnItemTap;

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.home_recycler_view);
            _refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.home_refresh_layout);

            _refreshLayout.SetOnRefreshListener(this);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            _recyclerView.AddItemDecoration(new DividerItemDecoration(Context, LinearLayoutManager.Vertical));
            _recyclerView.SetAdapter(_adapter);
        }

        void OnItemTap(MenuItem menuItem)
        {
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