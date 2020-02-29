using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Models;
using eHub.Common.Models;
using eHub.Common.Services;
using System.Collections.Generic;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener
    {
        HomeRecyclerAdapter _adapter;
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

            loadingDialog.Show();
            if (await PoolService.Ping())
            {
                _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.home_recycler_view);
                _refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.home_refresh_layout);

                _refreshLayout.SetOnRefreshListener(this);
                _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
                _recyclerView.AddItemDecoration(new DividerItemDecoration(Context, LinearLayoutManager.Horizontal));
                _recyclerView.SetAdapter(_adapter);

                statusLabel.Visibility = ViewStates.Gone;
                _refreshLayout.Visibility = ViewStates.Visible;

                var sched = await PoolService.GetSchedule();
                var pool = await PoolService.GetPinStatus(Pin.PoolPump);
                var poolLight = await PoolService.GetPinStatus(Pin.PoolLight);
                var spa = await PoolService.GetPinStatus(Pin.SpaPump);
                var spaLight = await PoolService.GetPinStatus(Pin.SpaLight);
                var booster = await PoolService.GetPinStatus(Pin.BoosterPump);
                var heater = await PoolService.GetPinStatus(Pin.Heater);
                var groundLights = await PoolService.GetPinStatus(Pin.GroundLights);

                var aboutItem = new HomeCellItem(CellType.About)
                {
                    AboutClicked = () =>
                    {
                        var frag = new AboutFragment();
                        ((MainActivity)Activity).Push(frag, "about_page");
                    }
                };

                var poolCell = new PoolCellItem(pool, poolLight);
                var spaCell = new SpaCellItem(spa, spaLight);

                var items = new List<HomeCellItem>(7)
                {
                    new HomeCellItem(sched, CellType.Schedule),
                    new HomeCellItem(poolCell, CellType.Pool),
                    new HomeCellItem(spaCell, CellType.Spa),
                    new HomeCellItem(booster, CellType.Booster),
                    new HomeCellItem(heater, CellType.Heater),
                    new HomeCellItem(groundLights, CellType.GroundLights),
                    aboutItem
                };

                _adapter = new HomeRecyclerAdapter(items);
            }
            else
            {
                statusLabel.Visibility = ViewStates.Visible;
                _refreshLayout.Visibility = ViewStates.Gone;
            }

            loadingDialog.Hide();
        }
    }
}