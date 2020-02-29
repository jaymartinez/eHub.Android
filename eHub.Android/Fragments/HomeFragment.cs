using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Models;
using eHub.Common.Models;
using eHub.Common.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener
    {
        HomeRecyclerAdapter _adapter;
        RecyclerView _recyclerView;
        SwipeRefreshLayout _refreshLayout;
        ProgressBar _progressBar;

        [Inject] IPoolService PoolService { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }

        public async void OnRefresh()
        {
            await OnRefreshAsync();
        }

        async Task OnRefreshAsync()
        {
            _adapter.Items = await RefreshView();
            _adapter.NotifyDataSetChanged();
            _refreshLayout.Refreshing = false;
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var statusLabel = view.FindViewById<TextView>(Resource.Id.home_status_label);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.home_recycler_view);
            _refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.home_refresh_layout);
            _progressBar = view.FindViewById<ProgressBar>(Resource.Id.home_progress_bar);

            _refreshLayout.SetOnRefreshListener(this);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            _recyclerView.AddItemDecoration(new DividerItemDecoration(Context, LinearLayoutManager.Horizontal));

            _progressBar.Visibility = ViewStates.Visible;
            if (await PoolService.Ping())
            {
                statusLabel.Visibility = ViewStates.Gone;
                _refreshLayout.Visibility = ViewStates.Visible;

                var items = await RefreshView();
                _adapter = new HomeRecyclerAdapter(items);
                _recyclerView.SetAdapter(_adapter);
            }
            else
            {
                statusLabel.Visibility = ViewStates.Visible;
                _refreshLayout.Visibility = ViewStates.Gone;
            }

            _progressBar.Visibility = ViewStates.Gone;
        }

        async Task<List<HomeCellItem>> RefreshView()
        {
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
                AboutTapped = () =>
                {
                    Dialogs.SimpleAlert(Context, "About", "Version: 1.1.1").Show();
                }
            };

            var poolCell = new PoolCellItem(pool, poolLight);
            var spaCell = new SpaCellItem(spa, spaLight);

            var schedCell = new ScheduleCellItem(sched)
            {
                StartTapped = (btn) =>
                {
                    var curStartHour = sched.StartHour;
                    var curStartMin = sched.StartMinute;
                    var curEndHour = sched.EndHour;
                    var curEndMin = sched.EndMinute;

                    var picker = TimePickerFragment.CreateInstance(curStartHour, curStartMin);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        var ps = new PoolSchedule
                        {
                            StartHour = args.Hour,
                            StartMinute = args.Minute,
                            EndHour = curEndHour,
                            EndMinute = curEndMin
                        };

                        await SaveScheduleAsync(ps);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                EndTapped = (btn) =>
                {
                    var curStartHour = sched.StartHour;
                    var curStartMin = sched.StartMinute;
                    var curEndHour = sched.EndHour;
                    var curEndMin = sched.EndMinute;

                    var picker = TimePickerFragment.CreateInstance(curEndHour, curEndMin);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        var ps = new PoolSchedule
                        {
                            StartHour = curStartHour,
                            StartMinute = curStartMin,
                            EndHour = args.Hour,
                            EndMinute = args.Minute
                        };

                        await SaveScheduleAsync(ps);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                }
            };

            return new List<HomeCellItem>(7)
            {
                new HomeCellItem(schedCell, CellType.Schedule),
                new HomeCellItem(poolCell, CellType.Pool),
                new HomeCellItem(spaCell, CellType.Spa),
                new HomeCellItem(booster, CellType.Booster),
                new HomeCellItem(heater, CellType.Heater),
                new HomeCellItem(groundLights, CellType.GroundLights),
                aboutItem
            };
        }

        async Task SaveScheduleAsync(PoolSchedule ps)
        {
            var startDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ps.StartHour, ps.StartMinute, 0);
            var endDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ps.EndHour, ps.EndMinute, 0);

            var result = await PoolService.SetSchedule(startDateTime, endDateTime, ps.IsActive);

            if (result != null)
            {
                Toast.MakeText(Context, "Schedule Saved", ToastLength.Short);
            }
        }
    }
}