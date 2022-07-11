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
            //var allPins = await _poolService.GetAllStatuses();
            var poolModel = await _poolService.GetPool(); //allPins.FirstOrDefault(_ => _.PinType == PinType.PoolPump) as PoolSpaModel;
            var spaModel = await _poolService.GetSpa(); //allPins.FirstOrDefault(_ => _.PinType == PinType.SpaPump) as PoolSpaModel;
            var boosterModel = await _poolService.GetBoosterPump(); //allPins.FirstOrDefault(_ => _.PinType == PinType.BoosterPump) as BoosterPumpModel;
            var heaterModel = await _poolService.GetHeater();
            var serverPoolLightMode = poolModel.Light.CurrentMode;  //await _poolService.GetCurrentLightMode(LightType.Pool);
            var serverSpaLightMode = spaModel.Light.CurrentMode;
            var curPoolLightSched = poolModel.Light.Schedule;
            var curSpaLightSched = spaModel.Light.Schedule;


            var devicesItem = new DeviceCellItem(poolModel, spaModel, boosterModel, heaterModel);//allPins.ToList());

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
                PoolLightModeButtonTapped = async (mode) =>
                {
                    // Get the state again
                    var currentPoolLightMode = await _poolService.GetCurrentLightMode(LightType.Pool);
                    var pool = await _poolService.GetPool();
                    var lightModel = new LightModel(mode, LightType.Pool);
                    return await OnLightModeButtonTapped(lightModel, currentPoolLightMode, pool.Light.State);
                },
                SpaLightModeButtonTapped = async (mode) =>
                {
                    // Get the state again
                    var currentSpaLightMode = await _poolService.GetCurrentLightMode(LightType.Spa);
                    var lightModel = new LightModel(mode, LightType.Spa);
                    var spa = await _poolService.GetSpa();
                    return await OnLightModeButtonTapped(lightModel, currentSpaLightMode, spa.Light.State);
                },
                PoolLightScheduleStartTapped = async (btn) =>
                {
                    // Get the current schedule from the server
                    var pool = await _poolService.GetPool();
                    var picker = TimePickerFragment.CreateInstance(pool.Schedule.StartTime.Hours, pool.Schedule.StartTime.Minutes);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var selectedTime = new TimeSpan(args.Hour, args.Minute, 0);

                        // update the new start time
                        pool.Schedule.StartTime = selectedTime;

                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = pool.Schedule.StartTime.ToString(@"%h\:mm");
                        });

                        await _poolService.SavePool(pool); //SaveScheduleAsync(_poolService, pool.Schedule);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                PoolLightScheduleEndTapped = async (btn) =>
                {
                    var pool = await _poolService.GetPool();
                    var picker = TimePickerFragment.CreateInstance(pool.Light.Schedule.EndTime.Hours, pool.Light.Schedule.EndTime.Minutes);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var selectedTime = new TimeSpan(args.Hour, args.Minute, 0);

                        // update the new end time
                        pool.Light.Schedule.EndTime = selectedTime;

                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = pool.Light.Schedule.EndTime.ToString(@"%h\:mm");
                        });

                        await _poolService.SavePool(pool); //SaveScheduleAsync(_poolService, pool.Light.Schedule);
                        Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },

                SpaLightScheduleStartTapped = async (btn) =>
                {
                    var spa = await _poolService.GetSpa();
                    var picker = TimePickerFragment.CreateInstance(spa.Light.Schedule.StartTime.Hours, spa.Light.Schedule.StartTime.Minutes);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var selectedTime = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = selectedTime.ToString(@"%h\:mm");
                        });

                        spa.Light.Schedule.StartTime = selectedTime;

                        await _poolService.SaveSpa(spa); //SaveScheduleAsync(_poolService, spa.Light.Schedule);
                        Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                SpaLightScheduleEndTapped = async (btn) =>
                {
                    var spa = await _poolService.GetSpa();
                    var picker = TimePickerFragment.CreateInstance(spa.Light.Schedule.EndTime.Hours, spa.Light.Schedule.EndTime.Minutes);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        spa.Light.Schedule.EndTime = time;

                        await _poolService.SaveSpa(spa); //SaveScheduleAsync(_poolService, pool.Light.Schedule);
                        Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
                        //await SaveScheduleAsync(_poolService, spa.Light.Schedule);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },
                PoolLightScheduleOnOffSwitchTapped = async sw =>
                {
                    var pool = await _poolService.GetPool();
                    pool.Light.Schedule.IsActive = !pool.Light.Schedule.IsActive;

                    await _poolService.SavePool(pool); //SaveScheduleAsync(_poolService, pool.Light.Schedule);

                    pool = await _poolService.GetPool();
                    sw.Checked = pool.Light.Schedule.IsActive;
                },
                SpaLightScheduleOnOffSwitchTapped = async sw =>
                {
                    var spa = await _poolService.GetSpa();
                    spa.Light.Schedule.IsActive = !spa.Light.Schedule.IsActive;

                    await _poolService.SaveSpa(spa);
                    //await SaveScheduleAsync(_poolService, spa.Light.Schedule);

                    spa = await _poolService.GetSpa();
                    sw.Checked = spa.Light.Schedule.IsActive;
                },
                SelectedPoolLightMode = serverPoolLightMode,
                SelectedSpaLightMode = serverSpaLightMode
            };

            var schedCell = new ScheduleCellItem(poolModel.Schedule, boosterModel.Schedule)
            {
                StartTapped = (btn, curSched) =>
                {
                    var picker = TimePickerFragment.CreateInstance(curSched.StartTime.Hours, curSched.StartTime.Minutes);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);

                        switch (curSched.Type)
                        {
                            case ScheduleType.Pool:
                                var pool = await _poolService.GetPool();
                                pool.Light.Schedule.StartTime = time;
                                await _poolService.SavePool(pool);
                                break;
                            case ScheduleType.Booster:
                                var booster = await _poolService.GetBoosterPump();
                                booster.Schedule.StartTime = time;
                                await _poolService.SaveBoosterPump(booster);
                                break;
                        }

                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                EndTapped = (btn, curSched) =>
                {
                    var picker = TimePickerFragment.CreateInstance(curSched.EndTime.Hours, curSched.EndTime.Minutes);

                    picker.OnTimeSelected = async (args) =>
                    {
                        var time = new TimeSpan(args.Hour, args.Minute, 0);

                        switch (curSched.Type)
                        {
                            case ScheduleType.Pool:
                                var pool = await _poolService.GetPool();
                                pool.Light.Schedule.EndTime = time;
                                await _poolService.SavePool(pool);
                                break;
                            case ScheduleType.Booster:
                                var booster = await _poolService.GetBoosterPump();
                                booster.Schedule.EndTime = time;
                                await _poolService.SaveBoosterPump(booster);
                                break;
                        }
                        Activity.RunOnUiThread(() =>
                        {
                            btn.Text = time.ToString(@"%h\:mm");
                        });

                        Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },
                
                OnButtonTapped = async (onBtn, offBtn, curSched) =>
                {
                    if (curSched.IsActive)
                    {
                        // Already on so bail out
                        return;
                    }

                    switch (curSched.Type)
                    {
                        case ScheduleType.Pool:
                            var pool = await _poolService.GetPool();
                            pool.Schedule.IsActive = true;
                            await _poolService.SavePool(pool);
                            break;
                        case ScheduleType.Booster:
                            var booster = await _poolService.GetBoosterPump();
                            booster.Schedule.IsActive = true;
                            await _poolService.SaveBoosterPump(booster);
                            break;
                    }

                    ButtonHelper.ToggleOnOffButtonStyle(
                        onBtn, 
                        offBtn, 
                        PinState.ON,
                        Context);
                },
                OffButtonTapped = async (onBtn, offBtn, curSched) =>
                {
                    if (!curSched.IsActive)
                    {
                        // Already off so bail out
                        return;
                    }

                    switch (curSched.Type)
                    {
                        case ScheduleType.Pool:
                            var pool = await _poolService.GetPool();
                            pool.Schedule.IsActive = false;
                            await _poolService.SavePool(pool);
                            break;
                        case ScheduleType.Booster:
                            var booster = await _poolService.GetBoosterPump();
                            booster.Schedule.IsActive = false;
                            await _poolService.SaveBoosterPump(booster);
                            break;
                    }

                    ButtonHelper.ToggleOnOffButtonStyle(
                        onBtn, 
                        offBtn, 
                        PinState.OFF,
                        Context);
                }
            };

            return new List<HomeCellItem>()
            {
                new HomeCellItem(schedCell, CellType.Schedule),
                new HomeCellItem(devicesItem, CellType.DeviceControl),
                new HomeCellItem(lightModesItem, CellType.LightModes),
                aboutItem
            };
        }

        async Task<LightModel> OnLightModeButtonTapped(LightModel model, LightServerModel serverModel, PinState curState) 
        {
            if (curState == PinState.OFF)
            {
                Toast.MakeText(Context, "Turn the light on before changing light modes", ToastLength.Long).Show();
                return null;
            }

            if (model.Mode == LightModeType.Recall && serverModel.PreviousMode == LightModeType.NotSet)
            {
                Toast.MakeText(Context, "There is no previous light mode saved yet.", ToastLength.Long).Show();
                return null;
            }

            if (model.Mode == serverModel.CurrentMode)
            {
                Toast.MakeText(Context, "You are already on that mode!", ToastLength.Long).Show();
                return null;
            }

            _progressBar.Visibility = ViewStates.Visible;

            var alert = Dialogs.SimpleAlert(Context, "Applying theme", model.Mode.ToLightModeText(), "");
            var numCycles = model.PowerCycles * 2;
            if (model.Mode == LightModeType.Recall)
            {
                alert = Dialogs.SimpleAlert(Context, "Applying last theme", serverModel.PreviousMode?.ToLightModeText(), "");
                numCycles = (int)serverModel.PreviousMode * 2;
            }

            alert.Show();
            alert.SetCancelable(false);
            alert.SetCanceledOnTouchOutside(false);
            for (var i = 0; i < numCycles; i++)
            {
                var toggleResult = await _poolService.Toggle(model.PinType);
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }

            // After applying mode the light takes a second to come back on.
            await Task.Delay(TimeSpan.FromSeconds(1));

            var modeToSave = model.Mode == LightModeType.Recall ?
                serverModel.PreviousMode : model.Mode;

            model.Mode = (await Task.Run(async () =>
            { 
                return await _poolService.SaveLightMode(modeToSave.Value, model.LightType);
            })).CurrentMode;

            _progressBar.Visibility = ViewStates.Gone;
            alert.Hide();

            return model;
        }

        /*
        async Task SaveScheduleAsync(IPoolService poolService, EquipmentSchedule es)
        {
            var result = await poolService.SetSchedule(es);
            if (result != null)
            {
                Toast.MakeText(Context, "Schedule Saved", ToastLength.Short).Show();
            }
        }
        */
    }
}