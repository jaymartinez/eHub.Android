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
using eHub.Common.Helpers;
using eHub.Android.Listeners;
using Fragment = AndroidX.Fragment.App.Fragment;
using AndroidX.SwipeRefreshLayout.Widget;
using AndroidX.RecyclerView.Widget;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment
    {
        ProgressBar _progressBar;
        TextView _statusLabel;
        TimePicker _poolTimer, _boosterTimer;
        Button _poolSaveBtn, _boosterSaveBtn;

        [Inject] private IPoolService _poolService { get; set; }
        [Inject] private AppVersion _appVersion { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            //HasOptionsMenu = true;

            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            _statusLabel = view.FindViewById<TextView>(Resource.Id.home_status_label);

            _progressBar = view.FindViewById<ProgressBar>(Resource.Id.home_progress_bar);

            _poolTimer = view.FindViewById<TimePicker>(Resource.Id.home_pool_timer);

            _boosterTimer = view.FindViewById<TimePicker>(Resource.Id.home_booster_timer);

            _boosterSaveBtn?.SetOnClickListener(new OnClickListener(async v =>
            {
                // TODO save booster timer
                /*
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
                */
                var curSched = await _poolService.GetSchedule();
                var ps = new PoolSchedule
                {
                    StartHour = _boosterTimer.Hour,
                    StartMinute = _boosterTimer.Minute,
                    EndHour = curSched.EndHour,
                    EndMinute = curSched.EndMinute,
                    IsActive = curSched.IsActive,
                    IncludeBooster = curSched.IncludeBooster
                };
                await SaveScheduleAsync(_poolService, ps);
                //var time = new TimeSpan(_boosterTimer.Hour, _boosterTimer.Minute, 0);

            }));

            _poolTimer.ChildViewAdded += (sender, e) =>
            {
                Console.WriteLine($"CHILD VIEW ADDED TO POOLTIMER: {e.Child.GetType()}");
            };
        }

        private void _poolTimer_ChildViewAdded(object sender, ViewGroup.ChildViewAddedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async void OnRefresh()
        {
            await ProcessView();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        public override async void OnResume()
        {
            base.OnResume();
            await ProcessView(); 
        }

        async Task ProcessView()
        {
            _progressBar.Visibility = ViewStates.Visible;
            if (await _poolService.Ping())
            {
                _statusLabel.Visibility = ViewStates.Gone;

                var items = await RefreshView(_poolService);
                if (items != null)
                {
                }
            }
            else
            {
                _statusLabel.Visibility = ViewStates.Visible;
                var mockPoolService = new MockPoolService();
                var mockItems = await RefreshView(mockPoolService);
                if (mockItems != null)
                {
                }
            }

            _progressBar.Visibility = ViewStates.Gone;
        }

        async Task<List<HomeCellItem>> RefreshView(IPoolService poolService)
        {
            //var waterTemp = await poolService.GetWaterTemp();
            var allPins = await poolService.GetAllStatuses();
            var poolSched = await poolService.GetSchedule();
            var boosterSched = await poolService.GetBoosterSchedule();

            Activity.RunOnUiThread(() =>
            {
                _poolTimer.Hour = poolSched.StartHour;
                _poolTimer.Minute = poolSched.StartMinute;
                _boosterTimer.Hour = boosterSched.StartHour;
                _boosterTimer.Minute = boosterSched.StartMinute;
            });

            var serverPoolLightMode = await poolService.GetCurrentPoolLightMode();
            var serverSpaLightMode = await poolService.GetCurrentSpaLightMode();
            var curPoolLightSched = await poolService.GetPoolLightSchedule();
            var curSpaLightSched = await poolService.GetSpaLightSchedule();

            if (allPins == null || poolSched == null || boosterSched == null)
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
                    Dialogs.SimpleAlert(Context, "About", $"Version: {_appVersion.VersionName}\nBuild: {_appVersion.VersionNumber}").Show();
                }
            };

            var lightModesItem = new LightModesCellItem(curPoolLightSched, curSpaLightSched)
            {
                PoolLightModeButtonTapped = async (model, selectedModeLabel) =>
                {
                    // Get the state again
                    var currentPoolLightMode = await poolService.GetCurrentPoolLightMode();
                    return await OnLightModeButtonTapped(model, currentPoolLightMode, selectedModeLabel, poolService, Pin.PoolLight, LightType.Pool);
                },
                SpaLightModeButtonTapped = async (model, selectedModeLabel) =>
                {
                    // Get the state again
                    var currentSpaLightMode = await poolService.GetCurrentSpaLightMode();
                    return await OnLightModeButtonTapped(model, currentSpaLightMode, selectedModeLabel, poolService, Pin.SpaLight, LightType.Spa);
                },
                PoolLightScheduleStartTapped = async (btn) =>
                {
                    var curSched = await poolService.GetPoolLightSchedule();
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

                        await SaveLightScheduleAsync(poolService, es, true);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                PoolLightScheduleEndTapped = async (btn) =>
                {
                    var curSched = await poolService.GetPoolLightSchedule();
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

                        await SaveLightScheduleAsync(poolService, es, true);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },

                SpaLightScheduleStartTapped = async (btn) =>
                {
                    var curSched = await poolService.GetSpaLightSchedule();
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

                        await SaveLightScheduleAsync(poolService, es, false);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                },
                SpaLightScheduleEndTapped = async (btn) =>
                {
                    var curSched = await poolService.GetSpaLightSchedule();
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

                        await SaveLightScheduleAsync(poolService, es, false);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                },
                PoolLightScheduleOnOffSwitchTapped = async sw =>
                {
                    var curSched = await poolService.GetPoolLightSchedule();

                    await SaveLightScheduleAsync(poolService, new EquipmentSchedule
                    {
                        StartHour = curSched.StartHour,
                        EndHour = curSched.EndHour,
                        StartMinute = curSched.StartMinute,
                        EndMinute = curSched.EndMinute,
                        IsActive = !curSched.IsActive
                    }, true);

                    curSched = await poolService.GetPoolLightSchedule();
                    sw.Checked = curSched.IsActive;
                },
                SpaLightScheduleOnOffSwitchTapped = async sw =>
                {
                    var curSched = await poolService.GetSpaLightSchedule();

                    await SaveLightScheduleAsync(poolService, new EquipmentSchedule
                    {
                        StartHour = curSched.StartHour,
                        EndHour = curSched.EndHour,
                        StartMinute = curSched.StartMinute,
                        EndMinute = curSched.EndMinute,
                        IsActive = !curSched.IsActive
                    }, false);

                    curSched = await poolService.GetSpaLightSchedule();
                    sw.Checked = curSched.IsActive;
                },
                SelectedPoolLightMode = serverPoolLightMode.CurrentPoolLightMode,
                SelectedSpaLightMode = serverSpaLightMode.CurrentPoolLightMode
            };

            var schedCell = new ScheduleCellItem(poolSched)
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
            TextView statusText, 
            IPoolService poolService,
            int lightPin,
            LightType lightType)
        {
            var state = (await poolService.GetPinStatus(lightPin))?.State ?? PinState.OFF;
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
                var toggleResult = await poolService.Toggle(lightPin);
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }

            // After applying mode the light takes a second to come back on.
            await Task.Delay(TimeSpan.FromSeconds(1));

            var modeToSave = model.Mode == PoolLightMode.Recall ?
                serverModel.PreviousPoolLightMode : model.Mode;

            if (lightType == LightType.Pool)
            {
                model.Mode = await Task.Run(async () =>
                { 
                    return await poolService.SavePoolLightMode(modeToSave);
                });
            }
            else
            {
                model.Mode = await Task.Run(async () =>
                { 
                    return await poolService.SaveSpaLightMode(modeToSave);
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