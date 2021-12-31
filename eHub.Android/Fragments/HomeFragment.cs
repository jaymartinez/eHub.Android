using Android.OS;
using Android.Views;
using Android.Widget;
using eHub.Android.Models;
using eHub.Common.Models;
using eHub.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Fragment = AndroidX.Fragment.App.Fragment;
using eHub.Common.Helpers;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.BottomSheet;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment, SwipeRefreshLayout.IOnRefreshListener
    {
        RecyclerView _recyclerView;
        SwipeRefreshLayout _refreshLayout;
        ProgressBar _progressBar;
        TextView _statusLabel;
        BottomSheetDialog _bottomSheet;
        IPoolService _poolService;

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

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        async Task ProcessView()
        {
            _progressBar.Visibility = ViewStates.Visible;
            if (await PoolService.Ping())
            {
                _poolService = PoolService;
                _statusLabel.Visibility = ViewStates.Gone;

                var items = await RefreshView();
                if (items != null)
                {
                    var adapter = new HomeRecyclerAdapter(items, _poolService, this);
                    _recyclerView.SetAdapter(adapter);
                }
            }
            else
            {
                _statusLabel.Visibility = ViewStates.Visible;
                _poolService = new MockPoolService();
                var mockItems = await RefreshView();
                if (mockItems != null)
                {
                    var adapter = new HomeRecyclerAdapter(mockItems, _poolService, this);
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


            var bs = LayoutInflater.Inflate(Resource.Layout.bottomsheet_light_color_legend, null);
            _bottomSheet = new BottomSheetDialog(Context);
            _bottomSheet.SetContentView(bs);
        }

        async Task<List<HomeCellItem>> RefreshView()
        {
            //var waterTemp = await poolService.GetWaterTemp();
            var allPins = await _poolService.GetAllStatuses();
            var sched = await _poolService.GetSchedule();
            var serverPoolLightMode = await _poolService.GetCurrentPoolLightMode();
            var serverSpaLightMode = await _poolService.GetCurrentSpaLightMode();
            var curPoolLightSched = await _poolService.GetPoolLightSchedule();
            var curSpaLightSched = await _poolService.GetSpaLightSchedule();

            if (allPins == null || sched == null)
            {
                Toast.MakeText(Context, "Failed to get pin status and schedule from server!!!", ToastLength.Long);
                _progressBar.Visibility = ViewStates.Gone;
                return null;
            }

            var booster1 = allPins.FirstOrDefault(_ => _.PinNumber == Pin.BoosterPump_1);
            var booster2 = allPins.FirstOrDefault(_ => _.PinNumber == Pin.BoosterPump_2);

            var devicesItem = new DeviceCellItem(allPins.ToList());

            var aboutItem = new HomeCellItem(CellType.About)
            {
                AboutTapped = () =>
                {
                    Dialogs.SimpleAlert(Context, "About", $"Version: {AppVersion.VersionName}\nBuild: {AppVersion.VersionNumber}").Show();
                }
            };

            var lightModesItem = new LightModesCellItem(curPoolLightSched, curSpaLightSched)
            {
                LightLegendTapped = () =>
                {
                    if (!_bottomSheet.IsShowing)
                    {
                        _bottomSheet.Create();
                        _bottomSheet.Show();
                    }
                },
                PoolLightModeButtonTapped = async (model, selectedModeLabel) =>
                {
                    // Get the state again
                    var currentPoolLightMode = await _poolService.GetCurrentPoolLightMode();
                    return await OnLightModeButtonTapped(model, currentPoolLightMode, selectedModeLabel);
                },
                SpaLightModeButtonTapped = async (model, selectedModeLabel) =>
                {
                    // Get the state again
                    var currentSpaLightMode = await _poolService.GetCurrentSpaLightMode();
                    return await OnLightModeButtonTapped(model, currentSpaLightMode, selectedModeLabel);
                },
                PoolLightScheduleStartTapped = async (btn) =>
                {
                    var curSched = await _poolService.GetPoolLightSchedule();
                    var picker = TimePickerFragment.CreateInstance(curSched.StartHour, curSched.StartMinute);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        var es = new EquipmentSchedule
                        {
                            StartHour = args.Hour,
                            StartMinute = args.Minute,
                            EndHour = curSched.EndHour,
                            EndMinute = curSched.EndMinute,
                            IsActive = curSched.IsActive
                        };

                        await SaveLightScheduleAsync(_poolService, es, true);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                PoolLightScheduleEndTapped = async (btn) =>
                {
                    var curSched = await _poolService.GetPoolLightSchedule();
                    var picker = TimePickerFragment.CreateInstance(curSched.EndHour, curSched.EndMinute);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        var es = new EquipmentSchedule
                        {
                            StartHour = curSched.StartHour,
                            StartMinute = curSched.StartMinute,
                            EndHour = args.Hour,
                            EndMinute = args.Minute,
                            IsActive = curSched.IsActive
                        };

                        await SaveLightScheduleAsync(_poolService, es, true);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },

                SpaLightScheduleStartTapped = async (btn) =>
                {
                    var curSched = await _poolService.GetSpaLightSchedule();
                    var picker = TimePickerFragment.CreateInstance(curSched.StartHour, curSched.StartMinute);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        var es = new EquipmentSchedule
                        {
                            StartHour = args.Hour,
                            StartMinute = args.Minute,
                            EndHour = curSched.EndHour,
                            EndMinute = curSched.EndMinute,
                            IsActive = curSched.IsActive
                        };

                        await SaveLightScheduleAsync(_poolService, es, false);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                SpaLightScheduleEndTapped = async (btn) =>
                {
                    var curSched = await _poolService.GetSpaLightSchedule();
                    var picker = TimePickerFragment.CreateInstance(curSched.EndHour, curSched.EndMinute);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        var es = new EquipmentSchedule
                        {
                            StartHour = curSched.StartHour,
                            StartMinute = curSched.StartMinute,
                            EndHour = args.Hour,
                            EndMinute = args.Minute,
                            IsActive = curSched.IsActive
                        };

                        await SaveLightScheduleAsync(_poolService, es, false);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },
                PoolLightScheduleOnOffSwitchTapped = async sw =>
                {
                    var curSched = await _poolService.GetPoolLightSchedule();

                    await SaveLightScheduleAsync(_poolService, new EquipmentSchedule
                    {
                        StartHour = curSched.StartHour,
                        EndHour = curSched.EndHour,
                        StartMinute = curSched.StartMinute,
                        EndMinute = curSched.EndMinute,
                        IsActive = !curSched.IsActive
                    }, true);

                    curSched = await _poolService.GetPoolLightSchedule();
                    sw.Checked = curSched.IsActive;
                },
                SpaLightScheduleOnOffSwitchTapped = async sw =>
                {
                    var curSched = await _poolService.GetSpaLightSchedule();

                    await SaveLightScheduleAsync(_poolService, new EquipmentSchedule
                    {
                        StartHour = curSched.StartHour,
                        EndHour = curSched.EndHour,
                        StartMinute = curSched.StartMinute,
                        EndMinute = curSched.EndMinute,
                        IsActive = !curSched.IsActive
                    }, false);

                    curSched = await _poolService.GetSpaLightSchedule();
                    sw.Checked = curSched.IsActive;
                },
                SelectedPoolLightMode = serverPoolLightMode.CurrentPoolLightMode,
                SelectedSpaLightMode = serverSpaLightMode.CurrentPoolLightMode
            };

            var schedCell = new ScheduleCellItem(sched)
            {
                StartTapped = async (btn) =>
                {
                    var curSched = await _poolService.GetSchedule();
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

                        await SaveScheduleAsync(_poolService, ps);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                EndTapped = async (btn) =>
                {
                    var curSched = await _poolService.GetSchedule();
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

                        await SaveScheduleAsync(_poolService, ps);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },
                OnOffSwitchTapped = async (sw) =>
                {
                    var state = await _poolService.ToggleMasterSwitch();
                    sw.Checked = state == 1;
                },
                IncludeBoosterTapped = async (cb) =>
                {
                    var state = await _poolService.ToggleIncludeBoosterSwitch();
                    cb.Checked = state == 1;
                }
            };

            var boosterCell = new BoosterCellItem(booster1, booster2);

            return new List<HomeCellItem>()
            {
                new HomeCellItem(schedCell, CellType.Schedule),
                new HomeCellItem(devicesItem, CellType.DeviceControl),
                new HomeCellItem(lightModesItem, CellType.LightModes),
                aboutItem
            };
        }

        async Task<PoolLightModel> OnLightModeButtonTapped(
            PoolLightModel model, 
            PoolLightServerModel serverModel,
            TextView statusText) 
        {
            var state = (await _poolService.GetPinStatus(model.PinNumber))?.State ?? PinState.OFF;
            if (state == PinState.OFF)
            {
                Toast.MakeText(Context, "Turn the light on before changing light modes", ToastLength.Long).Show();
                return null;
            }

            if (model.Mode == PoolLightMode.Recall && serverModel.PreviousPoolLightMode == PoolLightMode.NotSet)
            {
                Toast.MakeText(Context, "There is no previous light mode saved yet.", ToastLength.Long).Show();
                return null;
            }

            if (model.Mode == serverModel.CurrentPoolLightMode)
            {
                Toast.MakeText(Context, "You are already on that mode!", ToastLength.Long).Show();
                return null;
            }

            _progressBar.Visibility = ViewStates.Visible;

            var alert = Dialogs.SimpleAlert(Context, "Applying theme", model.Mode.ToLightModeText(), "");
            var numCycles = model.PowerCycles * 2;
            if (model.Mode == PoolLightMode.Recall)
            {
                alert = Dialogs.SimpleAlert(Context, "Applying last theme", serverModel.PreviousPoolLightMode.ToLightModeText(), "");
                numCycles = (int)serverModel.PreviousPoolLightMode * 2;
            }

            alert.Show();
            alert.SetCancelable(false);
            alert.SetCanceledOnTouchOutside(false);
            for (var i = 0; i < numCycles; i++)
            {
                var toggleResult = await _poolService.Toggle(model.PinNumber);
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }

            // After applying mode the light takes a second to come back on.
            await Task.Delay(TimeSpan.FromSeconds(1));

            var modeToSave = model.Mode == PoolLightMode.Recall ?
                serverModel.PreviousPoolLightMode : model.Mode;

            if (model.LightType == LightType.Pool)
            {
                model.Mode = await Task.Run(async () =>
                { 
                    return await _poolService.SavePoolLightMode(modeToSave);
                });
            }
            else
            {
                model.Mode = await Task.Run(async () =>
                { 
                    return await _poolService.SaveSpaLightMode(modeToSave);
                });
            }
            statusText.Text = model.Mode.ToLightModeText();

            _progressBar.Visibility = ViewStates.Gone;
            alert.Hide();

            return model;
        }

        async Task SaveLightScheduleAsync(IPoolService poolService, EquipmentSchedule es, bool isPoolLight)
        {
            var startDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, es.StartHour, es.StartMinute, 0);
            var endDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, es.EndHour, es.EndMinute, 0);

            var result = default(EquipmentSchedule);
            if (isPoolLight)
            {
                result = await poolService.SetPoolLightSchedule(startDateTime, endDateTime, es.IsActive);
            }
            else
            {
                result = await poolService.SetSpaLightSchedule(startDateTime, endDateTime, es.IsActive);
            }

            if (result != null)
            {
                Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
            }
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