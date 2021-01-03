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
using System.Linq;
using System.Threading.Tasks;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener
    {
        RecyclerView _recyclerView;
        SwipeRefreshLayout _refreshLayout;
        ProgressBar _progressBar;
        TextView _statusLabel;

        [Inject] IPoolService PoolService { get; set; }
        [Inject] AppVersion AppVersion { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }

        public async void OnRefresh()
        {
            await ProcessView();
            _refreshLayout.Refreshing = false;
        }

        public override async void OnResume()
        {
            base.OnResume();
            await ProcessView(); 
        }

        async Task ProcessView()
        {
            _progressBar.Visibility = ViewStates.Visible;
            var adapter = default(HomeRecyclerAdapter); 
            if (await PoolService.Ping())
            {
                _statusLabel.Visibility = ViewStates.Gone;

                var items = await RefreshView(PoolService);
                if (items != null)
                {
                    adapter = new HomeRecyclerAdapter(items, PoolService);
                    _recyclerView.SetAdapter(adapter);
                }
            }
            else
            {
                _statusLabel.Visibility = ViewStates.Visible;
                var mockPoolService = new MockPoolService();
                var mockItems = await RefreshView(mockPoolService);
                if (mockItems != null)
                {
                    adapter = new HomeRecyclerAdapter(mockItems, mockPoolService);
                    _recyclerView.SetAdapter(adapter);
                }
            }

            _progressBar.Visibility = ViewStates.Gone;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            _statusLabel = view.FindViewById<TextView>(Resource.Id.home_status_label);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.home_recycler_view);
            _refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.home_refresh_layout);
            _progressBar = view.FindViewById<ProgressBar>(Resource.Id.home_progress_bar);

            _refreshLayout.SetOnRefreshListener(this);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            _recyclerView.AddItemDecoration(new DividerItemDecoration(Context, LinearLayoutManager.Vertical));
        }

        async Task<List<HomeCellItem>> RefreshView(IPoolService poolService)
        {
            var allPins = await poolService.GetAllStatuses();
            var sched = await poolService.GetSchedule();

            if (allPins == null || sched == null)
            {
                Toast.MakeText(Context, "Failed to get pin status and schedule from server!!!", ToastLength.Long);
                _progressBar.Visibility = ViewStates.Gone;
                return null;
            }

            var pool = allPins.FirstOrDefault(_ => _.PinNumber == Pin.PoolPump);
            var poolLight = allPins.FirstOrDefault(_ => _.PinNumber == Pin.PoolLight);
            var spa = allPins.FirstOrDefault(_ => _.PinNumber == Pin.SpaPump);
            var spaLight = allPins.FirstOrDefault(_ => _.PinNumber == Pin.SpaLight);
            var booster = allPins.FirstOrDefault(_ => _.PinNumber == Pin.BoosterPump);
            var heater = allPins.FirstOrDefault(_ => _.PinNumber == Pin.Heater);
            var groundLights = allPins.FirstOrDefault(_ => _.PinNumber == Pin.GroundLights);

            var aboutItem = new HomeCellItem(CellType.About)
            {
                AboutTapped = () =>
                {
                    Dialogs.SimpleAlert(Context, "About", $"Version: {AppVersion.VersionName}\nBuild: {AppVersion.VersionNumber}").Show();
                }
            };

            var poolCell = new PoolCellItem(pool, poolLight);
            var spaCell = new SpaCellItem(spa, spaLight);

            var schedCell = new ScheduleCellItem(sched)
            {
                StartTapped = async (btn) =>
                {
                    var curSched = await poolService.GetSchedule();
                    var picker = TimePickerFragment.CreateInstance(curSched.StartHour, curSched.StartMinute);

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
                            EndHour = curSched.EndHour,
                            EndMinute = curSched.EndMinute,
                            IsActive = curSched.IsActive,
                            IncludeBooster = curSched.IncludeBooster
                        };

                        await SaveScheduleAsync(poolService, ps);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                EndTapped = async (btn) =>
                {
                    var curSched = await poolService.GetSchedule();
                    var picker = TimePickerFragment.CreateInstance(curSched.EndHour, curSched.EndMinute);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        var ps = new PoolSchedule
                        {
                            StartHour = curSched.StartHour,
                            StartMinute = curSched.StartMinute,
                            EndHour = args.Hour,
                            EndMinute = args.Minute,
                            IsActive = curSched.IsActive,
                            IncludeBooster = curSched.IncludeBooster
                        };

                        await SaveScheduleAsync(poolService, ps);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },
                OnOffSwitchTapped = async (sw) =>
                {
                    var state = await poolService.ToggleMasterSwitch();
                    sw.Checked = state == 1;
                },
                IncludeBoosterTapped = async (cb) =>
                {
                    var state = await poolService.ToggleIncludeBoosterSwitch();
                    cb.Checked = state == 1;
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

        async Task SaveScheduleAsync(IPoolService poolService, PoolSchedule ps)
        {
            var startDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ps.StartHour, ps.StartMinute, 0);
            var endDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ps.EndHour, ps.EndMinute, 0);

            var result = await poolService.SetSchedule(startDateTime, endDateTime, ps.IsActive, ps.IncludeBooster);

            if (result != null)
            {
                Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
            }
        }
    }
}