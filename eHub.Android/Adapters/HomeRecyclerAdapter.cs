using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Fragments;
using eHub.Android.Listeners;
using eHub.Android.Models;
using eHub.Common.Helpers;
using eHub.Common.Models;
using eHub.Common.Services;
using Switch = Android.Support.V7.Widget.SwitchCompat; 

namespace eHub.Android
{
    public class HomeRecyclerAdapter : RecyclerView.Adapter
    {
        const int ScheduleId = 1;
        const int AboutId = 7;
        const int DevicesId = 8;
        const int LightModesCellId = 9;

        readonly Handler _mainUiHandler;
        readonly IPoolService _poolService;
        readonly HomeFragment _homeFragment;

        public List<HomeCellItem> Items { get; set; } = new List<HomeCellItem>();

        public WeakReference ActivityRef { get; set; }

        public HomeRecyclerAdapter(List<HomeCellItem> items, IPoolService poolService, HomeFragment homeFragment)
        {
            _mainUiHandler = new Handler(Looper.MainLooper);
            _poolService = poolService;
            Items = items;
            _homeFragment = homeFragment;
        }

        public override int ItemCount => Items?.Count ?? 0;

        public override int GetItemViewType(int position)
        {
            var item = Items[position];

            switch (item.CellTypeObj)
            {
                case CellType.Schedule:
                    return ScheduleId;
                case CellType.About:
                    return AboutId;
                case CellType.DeviceControl:
                    return DevicesId;
                case CellType.LightModes:
                    return LightModesCellId;
                default:
                    return -1;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = Items[position];

            switch (item.CellTypeObj)
            {
                case CellType.Schedule:
                    var schCell = holder as ScheduleCell;
                    var startTime = new TimeSpan(item.ScheduleCellItem.Schedule.StartHour, item.ScheduleCellItem.Schedule.StartMinute, 0);
                    var endTime = new TimeSpan(item.ScheduleCellItem.Schedule.EndHour, item.ScheduleCellItem.Schedule.EndMinute, 0);
                    schCell.StartButton.Text = startTime.ToString(@"%h\:mm");
                    schCell.EndButton.Text = endTime.ToString(@"%h\:mm");
                    schCell.OnOffSwitch.Checked = item.ScheduleCellItem.Schedule.IsActive;
                    schCell.IncludeBoosterCheckbox.Checked = item.ScheduleCellItem.Schedule.IncludeBooster;

                    schCell.StartButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.ScheduleCellItem.StartTapped.Invoke(v as Button);
                    }));

                    schCell.EndButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.ScheduleCellItem.EndTapped.Invoke(v as Button);
                    }));

                    schCell.OnOffSwitch.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.ScheduleCellItem.OnOffSwitchTapped.Invoke(v as Switch);
                    }));

                    schCell.IncludeBoosterCheckbox.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.ScheduleCellItem.IncludeBoosterTapped.Invoke(v as CheckBox);
                    }));
                    break;

                case CellType.LightModes:
                    var lightModeCell = holder as LightModesCell;
                    var poolLightStart = new TimeSpan(item.LightModesItem.PoolLightSchedule.StartHour, 
                        item.LightModesItem.PoolLightSchedule.StartMinute, 0);
                    var poolLightEnd = new TimeSpan(item.LightModesItem.PoolLightSchedule.EndHour, 
                        item.LightModesItem.PoolLightSchedule.EndMinute, 0);
                    var spaLightStart = new TimeSpan(item.LightModesItem.SpaLightSchedule.StartHour, 
                        item.LightModesItem.SpaLightSchedule.StartMinute, 0);
                    var spaLightEnd = new TimeSpan(item.LightModesItem.SpaLightSchedule.EndHour, 
                        item.LightModesItem.SpaLightSchedule.EndMinute, 0);

                    lightModeCell.PoolLightStartButton.Text = poolLightStart.ToString(@"%h\:mm");
                    lightModeCell.PoolLightEndButton.Text = poolLightEnd.ToString(@"%h\:mm");
                    lightModeCell.SpaLightStartButton.Text = spaLightStart.ToString(@"%h\:mm");
                    lightModeCell.SpaLightEndButton.Text = spaLightEnd.ToString(@"%h\:mm");

                    lightModeCell.PoolLightOnOffSwitch.Checked = item.LightModesItem.PoolLightSchedule.IsActive;
                    lightModeCell.SpaLightOnOffSwitch.Checked = item.LightModesItem.SpaLightSchedule.IsActive;

                    lightModeCell.Bind(item);

                    lightModeCell.PoolLightLegendImageView.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.LightLegendTapped.Invoke();
                    }));
                    lightModeCell.SpaLightLegendImageView.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.LightLegendTapped.Invoke();
                    }));

                    lightModeCell.PoolLightStartButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.PoolLightScheduleStartTapped.Invoke(v as Button);
                    }));
                    lightModeCell.PoolLightEndButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.PoolLightScheduleEndTapped.Invoke(v as Button);
                    }));

                    lightModeCell.SpaLightStartButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.SpaLightScheduleStartTapped.Invoke(v as Button);
                    }));
                    lightModeCell.SpaLightEndButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.SpaLightScheduleEndTapped.Invoke(v as Button);
                    }));

                    lightModeCell.PoolLightOnOffSwitch.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.PoolLightScheduleOnOffSwitchTapped.Invoke(v as Switch);
                    }));
                    lightModeCell.SpaLightOnOffSwitch.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.LightModesItem.SpaLightScheduleOnOffSwitchTapped.Invoke(v as Switch);
                    }));

                    //var poolLightFrag = LightModesGridViewFragment.CreateInstance(item.LightModesItem.PoolLightModelList);
                    //_homeFragment.ChildFragmentManager.BeginTransaction()
                    //    .SetTransition((int)FragmentTransit.FragmentOpen)
                    //    .Replace(Resource.Id.light_mode_gridview_container_pool, poolLightFrag, "pool_light_frag")
                    //    .Commit();

                    //var spaLightFrag = LightModesGridViewFragment.CreateInstance(item.LightModesItem.SpaLightModelList);
                    //_homeFragment.ChildFragmentManager.BeginTransaction()
                    //    .SetTransition((int)FragmentTransit.FragmentOpen)
                    //    .Replace(Resource.Id.light_mode_gridview_container_spa, spaLightFrag, "spa_light_frag")
                    //    .Commit();


                    var selectedColor = new Color(ContextCompat.GetColor(lightModeCell.ItemView.Context, Resource.Color.orangeHolo));
                    #region Pool Light Mode Events
                    lightModeCell.PoolLightSamModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Sam, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);

                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightPartyModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Party, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);

                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightRomanceModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Romance, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightCaribbeanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Caribbean, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightAmericanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.American, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null) 
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightCaliSunsetModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.CaliforniaSunset, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightRoyalModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Royal, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightBlueModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Blue, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightGreenModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Green, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightRedModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Red, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightWhiteModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.White, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightMagentaModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Magenta, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightHoldModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Hold, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.PoolLightRecallModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Recall, LightType.Pool), lightModeCell.PoolLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeGrid, v.Id);
                        }
                    }));
                    #endregion

                    #region Spa Light Mode Events

                    lightModeCell.SpaLightSamModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Sam, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);

                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightPartyModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Party, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);

                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightRomanceModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Romance, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightCaribbeanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Caribbean, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightAmericanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.American, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightCaliSunsetModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.CaliforniaSunset, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightRoyalModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Royal, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightBlueModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Blue, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightGreenModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Green, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightRedModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Red, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightWhiteModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.White, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightMagentaModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Magenta, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);
                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightHoldModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Hold, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);

                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    lightModeCell.SpaLightRecallModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(
                            new PoolLightModel(PoolLightMode.Recall, LightType.Spa), lightModeCell.SpaLightSelectedLightModeText);

                        if (result != null)
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.SpaLightModeGrid, v.Id);
                        }
                    }));
                    #endregion

                    break;

                case CellType.DeviceControl:
                    var devicesCell = holder as DevicesCell;

                    // Set initial states
                    var pool = item.DevicesItem.DevicePins.Find(_ => _.PinNumber == Pin.PoolPump_1);
                    var spa = item.DevicesItem.DevicePins.Find(_ => _.PinNumber == Pin.SpaPump_1);
                    var booster = item.DevicesItem.DevicePins.Find(_ => _.PinNumber == Pin.BoosterPump_1);
                    var heater = item.DevicesItem.DevicePins.Find(_ => _.PinNumber == Pin.Heater);
                    var poolLight = item.DevicesItem.DevicePins.Find(_ => _.PinNumber == Pin.PoolLight);
                    var spaLight = item.DevicesItem.DevicePins.Find(_ => _.PinNumber == Pin.SpaLight);

                    ToggleOnOffButtonStyle(devicesCell.PoolOnButton, devicesCell.PoolOffButton, pool.State, devicesCell.ItemView.Context);
                    ToggleOnOffButtonStyle(devicesCell.SpaOnButton, devicesCell.SpaOffButton, spa.State, devicesCell.ItemView.Context);
                    ToggleOnOffButtonStyle(devicesCell.BoosterOnButton, devicesCell.BoosterOffButton, booster.State, devicesCell.ItemView.Context);
                    ToggleOnOffButtonStyle(devicesCell.HeaterOnButton, devicesCell.HeaterOffButton, heater.State, devicesCell.ItemView.Context);
                    ToggleOnOffButtonStyle(devicesCell.PoolLightOnButton, devicesCell.PoolLightOffButton, poolLight.State, devicesCell.ItemView.Context);
                    ToggleOnOffButtonStyle(devicesCell.SpaLightOnButton, devicesCell.SpaLightOffButton, spaLight.State, devicesCell.ItemView.Context);

                    devicesCell.PoolOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var curPool1State = await GetStatus(Pin.PoolPump_1);
                        var curPool2State = await GetStatus(Pin.PoolPump_2);

                        // if both are already on bail out
                        if (curPool1State == PinState.ON && curPool2State == PinState.ON)
                        {
                            return;
                        }

                        var onButton = v as Button;
                        Dialogs.Confirm(devicesCell.ItemView.Context,
                            "Are You Sure?",
                            $"Are you sure you want to turn it on?",
                            "Yes", async (confirmed) =>
                            {
                                if (confirmed)
                                {
                                    // toggling pool1 or pool2 will turn the both on at the same time on the server
                                    var poolToggle = await _poolService.Toggle(Pin.PoolPump_1);
                                    if (poolToggle != null)
                                    {
                                        ToggleOnOffButtonStyle(onButton,
                                            devicesCell.PoolOffButton,
                                            poolToggle.State,
                                            devicesCell.ItemView.Context);
                                    }
                                }
                            }, "No").Show();
                    }));

                    devicesCell.PoolOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var curPool1State = await GetStatus(Pin.PoolPump_1);
                        var curPool2State = await GetStatus(Pin.PoolPump_2);

                        // if both are already off bail out
                        if (curPool1State == PinState.OFF
                            && curPool2State == PinState.OFF)
                        {
                            return;
                        }

                        var offButton = v as Button;
                        var heaterStatus = await GetStatus(Pin.Heater);
                        var booster1Status = await GetStatus(Pin.BoosterPump_1);
                        var booster2Status = await GetStatus(Pin.BoosterPump_2);

                        if (curPool1State == PinState.ON && curPool2State == PinState.ON
                            && (heaterStatus == PinState.ON || booster1Status == PinState.ON || booster2Status == PinState.ON))
                        {
                            Toast.MakeText(v.Context, "Make sure the heater and the booster pump are off first!",
                                ToastLength.Short).Show();
                            return;
                        }

                        Dialogs.Confirm(devicesCell.ItemView.Context,
                            "Are You Sure?",
                            $"Are you sure you want to turn it off?",
                            "Yes", async (confirmed) =>
                            {
                                if (confirmed)
                                {
                                    // toggling pool1 or pool2 will turn the both on at the same time on the server
                                    var poolToggle = await _poolService.Toggle(Pin.PoolPump_1);
                                    if (poolToggle != null)
                                    {
                                        ToggleOnOffButtonStyle(devicesCell.PoolOnButton,
                                            offButton,
                                            poolToggle.State,
                                            devicesCell.ItemView.Context);
                                    }
                                }
                            }, "No").Show();
                    }));

                    devicesCell.BoosterOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var curStatus = await GetStatus(Pin.BoosterPump_1);

                        if (curStatus == PinState.ON)
                        {
                            return;
                        }

                        // Make sure the pool pump is on first!
                        var poolPumpStatus = await GetStatus(Pin.PoolPump_1);
                        if (poolPumpStatus == PinState.OFF)
                        {
                            Toast.MakeText(v.Context, "Wait! The pool pump needs to be on first!",
                                ToastLength.Short).Show();
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.BoosterPump_1);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(onButton,
                                devicesCell.BoosterOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.BoosterOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var curStatus = await GetStatus(Pin.BoosterPump_1);

                        if (curStatus == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.BoosterPump_1);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(devicesCell.BoosterOnButton,
                                offButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.HeaterOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var curStatus = await GetStatus(Pin.Heater);

                        if (curStatus == PinState.ON)
                        {
                            return;
                        }

                        // Make sure the pool pump is on first!
                        var poolPumpStatus = await GetStatus(Pin.PoolPump_1);
                        if (poolPumpStatus == PinState.OFF)
                        {
                            Toast.MakeText(v.Context, "Wait! The pool pump needs to be on first!",
                                ToastLength.Short).Show();
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.Heater);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(onButton,
                                devicesCell.HeaterOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.HeaterOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var curStatus = await GetStatus(Pin.Heater);

                        if (curStatus == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.Heater);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(devicesCell.HeaterOnButton,
                                offButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var curStatus = await GetStatus(Pin.SpaPump_1);

                        if (curStatus == PinState.ON)
                        {
                            return;
                        }

                        var spaToggle = await _poolService.Toggle(Pin.SpaPump_1);
                        if (spaToggle != null)
                        {
                            ToggleOnOffButtonStyle(onButton,
                                devicesCell.SpaOffButton,
                                spaToggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var curStatus = await GetStatus(Pin.SpaPump_1);

                        if (curStatus == PinState.OFF)
                        {
                            return;
                        }

                        var spaToggle = await _poolService.Toggle(Pin.SpaPump_1);
                        if (spaToggle != null)
                        {
                            ToggleOnOffButtonStyle(devicesCell.SpaOnButton,
                                offButton,
                                spaToggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.PoolLightOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var curStatus = await GetStatus(Pin.PoolLight);

                        if (curStatus == PinState.ON)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.PoolLight);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(onButton,
                                devicesCell.PoolLightOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.PoolLightOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var curStatus = await GetStatus(Pin.PoolLight);

                        if (curStatus == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.PoolLight);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(devicesCell.PoolLightOnButton,
                                offButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaLightOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var curStatus = await GetStatus(Pin.SpaLight);

                        if (curStatus == PinState.ON)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.SpaLight);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(onButton,
                                devicesCell.SpaLightOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaLightOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var curStatus = await GetStatus(Pin.SpaLight);

                        if (curStatus == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(Pin.SpaLight);
                        if (toggle != null)
                        {
                            ToggleOnOffButtonStyle(devicesCell.SpaLightOnButton,
                                offButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    break;

                case CellType.About:
                    var aboutCell = holder as AboutCell;
                    aboutCell.ItemView.SetOnClickListener(new OnClickListener(v =>
                    {
                        _mainUiHandler.Post(() =>
                        {
                            item.AboutTapped.Invoke();
                        });
                    }));
                    break;
            }
        }

        /// <summary>
        /// Deactivate all buttons except currently selected light button
        /// </summary>
        /// <param name="currentButton"></param>
        void DeactivateOtherLightButtons(Context context, GridLayout grid, int currentButtonId)
        {
            var defaultColor = new Color(ContextCompat.GetColor(context, Resource.Color.orangeHolo));
            if (grid.ChildCount > 0)
            {
                for (var i = 0; i < grid.ChildCount; i++)
                {
                    var child = grid.GetChildAt(i);
                    if (child is Button button && button.Id != currentButtonId)
                    {
                        button.SetBackgroundColor(defaultColor);
                    }
                }
            }
        }

        async Task<int> GetStatus(int pin)
        {
            var result = await _poolService.GetPinStatus(pin);
            return result.State;
        }

        void ToggleOnOffButtonStyle(
            Button onButton, 
            Button offButton, 
            int state, 
            Context context)
        {
            _mainUiHandler.Post(() =>
            {
                var onTextColor = ContextCompat.GetColor(
                    context, Resource.Color.material_blue_grey_800);

                var offTextColor = ContextCompat.GetColor(
                    context, Resource.Color.material_grey_300);

                if (state == PinState.ON)
                {
                    onButton.SetBackgroundResource(Resource.Color.greenLabel);
                    onButton.SetTextColor(new Color(onTextColor));

                    offButton.SetBackgroundResource(Resource.Color.blue_gray_400);
                    offButton.SetTextColor(new Color(offTextColor));
                }
                else
                {
                    offButton.SetBackgroundResource(Resource.Color.redLabel);
                    offButton.SetTextColor(new Color(offTextColor));

                    onButton.SetBackgroundResource(Resource.Color.blue_gray_400);
                    onButton.SetTextColor(new Color(offTextColor));
                }
            });
        }

        void SetButtonBackground(Button b, int state, CellType? cellType = null)
        {
            _mainUiHandler.Post(() =>
            {
                if (state == PinState.ON)
                {
                    b.SetBackgroundResource(Resource.Drawable.rounded_corners_green_8dp);
                    b.Text = cellType == CellType.Pool || cellType == CellType.Spa 
                        ? "TURN PUMP OFF" : "TURN OFF";
                }
                else
                {
                    b.SetBackgroundResource(Resource.Drawable.rounded_corners_bluegray_8dp);
                    b.Text = cellType == CellType.Pool || cellType == CellType.Spa 
                        ? "TURN PUMP ON" : "TURN ON";
                }
            });
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);

            switch (viewType)
            {
                case ScheduleId:
                    var schView = inflater.Inflate(Resource.Layout.item_schedule_cell, parent, false);
                    return new ScheduleCell(schView);
                case AboutId:
                    var aboutView = inflater.Inflate(Resource.Layout.item_about_cell, parent, false);
                    return new AboutCell(aboutView);
                case DevicesId:
                    var devicesView = inflater.Inflate(Resource.Layout.item_devices_cell, parent, false);
                    return new DevicesCell(devicesView);
                case LightModesCellId:
                    var lightModesView = inflater.Inflate(Resource.Layout.item_light_modes_cell, parent, false);
                    return new LightModesCell(lightModesView);
            }

            return null;
        }

        class ScheduleCell : RecyclerView.ViewHolder
        {
            public Button StartButton { get; }
            public Button EndButton { get; }
            public CheckBox IncludeBoosterCheckbox { get; }
            public Switch OnOffSwitch { get; }

            public ScheduleCell(View view)
                : base(view)
            {
                StartButton = view.FindViewById<Button>(Resource.Id.schedule_cell_begin_btn);
                EndButton = view.FindViewById<Button>(Resource.Id.schedule_cell_end_btn);
                IncludeBoosterCheckbox = view.FindViewById<CheckBox>(Resource.Id.schedule_cell_include_booster_cb);
                OnOffSwitch = view.FindViewById<Switch>(Resource.Id.schedule_onoff_switch);
            }
        }

        class EquipmentCell : RecyclerView.ViewHolder
        {
            public TextView StatusTextView { get; set; }
            public Button OnOffButton { get; set; }

            public EquipmentCell(View view)
                : base(view) { }
        }

        class DevicesCell : RecyclerView.ViewHolder
        {
            public Button PoolOnButton { get; }
            public Button PoolOffButton { get; }
            public Button SpaOnButton { get; }
            public Button SpaOffButton { get; }
            public Button BoosterOnButton { get; }
            public Button BoosterOffButton { get; }
            public Button HeaterOnButton { get; }
            public Button HeaterOffButton { get; }
            public Button PoolLightOnButton { get; }
            public Button PoolLightOffButton { get; }
            public Button SpaLightOnButton { get; }
            public Button SpaLightOffButton { get; }

            public DevicesCell(View view)
                : base(view)
            {
                PoolOnButton = view.FindViewById<Button>(Resource.Id.device_cell_pool_on_btn);
                PoolOffButton = view.FindViewById<Button>(Resource.Id.device_cell_pool_off_btn);
                SpaOnButton = view.FindViewById<Button>(Resource.Id.device_cell_spa_on_btn);
                SpaOffButton = view.FindViewById<Button>(Resource.Id.device_cell_spa_off_btn);
                BoosterOnButton = view.FindViewById<Button>(Resource.Id.device_cell_booster_on_btn);
                BoosterOffButton = view.FindViewById<Button>(Resource.Id.device_cell_booster_off_btn);
                HeaterOnButton = view.FindViewById<Button>(Resource.Id.device_cell_heater_on_btn);
                HeaterOffButton = view.FindViewById<Button>(Resource.Id.device_cell_heater_off_btn);
                PoolLightOnButton = view.FindViewById<Button>(Resource.Id.device_cell_pool_light_on_btn);
                PoolLightOffButton = view.FindViewById<Button>(Resource.Id.device_cell_pool_light_off_btn);
                SpaLightOnButton = view.FindViewById<Button>(Resource.Id.device_cell_spa_light_on_btn);
                SpaLightOffButton = view.FindViewById<Button>(Resource.Id.device_cell_spa_light_off_btn);
            }
        }

        class LightModesCell : RecyclerView.ViewHolder
        {
            public Switch PoolLightOnOffSwitch { get; }
            public Button PoolLightStartButton { get; }
            public Button PoolLightEndButton { get; }
            public Button PoolLightSamModeButton { get; }
            public Button PoolLightPartyModeButton { get; }
            public Button PoolLightRomanceModeButton { get; }
            public Button PoolLightCaribbeanModeButton { get; }
            public Button PoolLightAmericanModeButton { get; }
            public Button PoolLightCaliSunsetModeButton { get; }
            public Button PoolLightRoyalModeButton { get; }
            public Button PoolLightBlueModeButton { get; }
            public Button PoolLightGreenModeButton { get; }
            public Button PoolLightRedModeButton { get; }
            public Button PoolLightWhiteModeButton { get; }
            public Button PoolLightMagentaModeButton { get; }
            public Button PoolLightHoldModeButton { get; }
            public Button PoolLightRecallModeButton { get; }
            public TextView PoolLightSelectedLightModeText { get; }
            public GridLayout PoolLightModeGrid { get; }
            public ImageView PoolLightLegendImageView { get; }

            public Switch SpaLightOnOffSwitch { get; }
            public Button SpaLightStartButton { get; }
            public Button SpaLightEndButton { get; }
            public Button SpaLightSamModeButton { get; }
            public Button SpaLightPartyModeButton { get; }
            public Button SpaLightRomanceModeButton { get; }
            public Button SpaLightCaribbeanModeButton { get; }
            public Button SpaLightAmericanModeButton { get; }
            public Button SpaLightCaliSunsetModeButton { get; }
            public Button SpaLightRoyalModeButton { get; }
            public Button SpaLightBlueModeButton { get; }
            public Button SpaLightGreenModeButton { get; }
            public Button SpaLightRedModeButton { get; }
            public Button SpaLightWhiteModeButton { get; }
            public Button SpaLightMagentaModeButton { get; }
            public Button SpaLightHoldModeButton { get; }
            public Button SpaLightRecallModeButton { get; }
            public TextView SpaLightSelectedLightModeText { get; }
            public GridLayout SpaLightModeGrid { get; }
            public ImageView SpaLightLegendImageView { get; }

            public LightModesCell(View view)
                : base (view)
            {
                PoolLightLegendImageView = view.FindViewById<ImageView>(Resource.Id.pool_light_modes_legend);
                PoolLightModeGrid = view.FindViewById<GridLayout>(Resource.Id.pool_light_grid_layout);
                PoolLightOnOffSwitch = view.FindViewById<Switch>(Resource.Id.pool_light_schedule_onoff_switch);
                PoolLightStartButton = view.FindViewById<Button>(Resource.Id.pool_light_schedule_begin_btn);
                PoolLightEndButton = view.FindViewById<Button>(Resource.Id.pool_light_schedule_end_btn);
                PoolLightSamModeButton = view.FindViewById<Button>(Resource.Id.pool_light_sam_btn);
                PoolLightPartyModeButton = view.FindViewById<Button>(Resource.Id.pool_light_party_btn);
                PoolLightRomanceModeButton = view.FindViewById<Button>(Resource.Id.pool_light_romance_btn);
                PoolLightCaribbeanModeButton = view.FindViewById<Button>(Resource.Id.pool_light_caribbean_btn);
                PoolLightAmericanModeButton = view.FindViewById<Button>(Resource.Id.pool_light_american_btn);
                PoolLightCaliSunsetModeButton = view.FindViewById<Button>(Resource.Id.pool_light_sunset_btn);
                PoolLightRoyalModeButton = view.FindViewById<Button>(Resource.Id.pool_light_royal_btn);
                PoolLightBlueModeButton = view.FindViewById<Button>(Resource.Id.pool_light_blue_btn);
                PoolLightGreenModeButton = view.FindViewById<Button>(Resource.Id.pool_light_green_btn);
                PoolLightRedModeButton = view.FindViewById<Button>(Resource.Id.pool_light_red_btn);
                PoolLightWhiteModeButton = view.FindViewById<Button>(Resource.Id.pool_light_white_btn);
                PoolLightMagentaModeButton = view.FindViewById<Button>(Resource.Id.pool_light_magenta_btn);
                PoolLightHoldModeButton = view.FindViewById<Button>(Resource.Id.pool_light_hold_btn);
                PoolLightRecallModeButton = view.FindViewById<Button>(Resource.Id.pool_light_recall_btn);
                PoolLightSelectedLightModeText = view.FindViewById<TextView>(Resource.Id.pool_light_selected_light_mode_label);

                SpaLightLegendImageView = view.FindViewById<ImageView>(Resource.Id.spa_light_modes_legend);
                SpaLightModeGrid = view.FindViewById<GridLayout>(Resource.Id.spa_light_grid_layout);
                SpaLightOnOffSwitch = view.FindViewById<Switch>(Resource.Id.spa_light_schedule_onoff_switch);
                SpaLightStartButton = view.FindViewById<Button>(Resource.Id.spa_light_schedule_begin_btn);
                SpaLightEndButton = view.FindViewById<Button>(Resource.Id.spa_light_schedule_end_btn);
                SpaLightSamModeButton = view.FindViewById<Button>(Resource.Id.spa_light_sam_btn);
                SpaLightPartyModeButton = view.FindViewById<Button>(Resource.Id.spa_light_party_btn);
                SpaLightRomanceModeButton = view.FindViewById<Button>(Resource.Id.spa_light_romance_btn);
                SpaLightCaribbeanModeButton = view.FindViewById<Button>(Resource.Id.spa_light_caribbean_btn);
                SpaLightAmericanModeButton = view.FindViewById<Button>(Resource.Id.spa_light_american_btn);
                SpaLightCaliSunsetModeButton = view.FindViewById<Button>(Resource.Id.spa_light_sunset_btn);
                SpaLightRoyalModeButton = view.FindViewById<Button>(Resource.Id.spa_light_royal_btn);
                SpaLightBlueModeButton = view.FindViewById<Button>(Resource.Id.spa_light_blue_btn);
                SpaLightGreenModeButton = view.FindViewById<Button>(Resource.Id.spa_light_green_btn);
                SpaLightRedModeButton = view.FindViewById<Button>(Resource.Id.spa_light_red_btn);
                SpaLightWhiteModeButton = view.FindViewById<Button>(Resource.Id.spa_light_white_btn);
                SpaLightMagentaModeButton = view.FindViewById<Button>(Resource.Id.spa_light_magenta_btn);
                SpaLightHoldModeButton = view.FindViewById<Button>(Resource.Id.spa_light_hold_btn);
                SpaLightRecallModeButton = view.FindViewById<Button>(Resource.Id.spa_light_recall_btn);
                SpaLightSelectedLightModeText = view.FindViewById<TextView>(Resource.Id.spa_light_selected_light_mode_label);
            }

            internal void Bind(HomeCellItem item)
            {
                var selectedColor = new Color(ContextCompat.GetColor(ItemView.Context, Resource.Color.orangeHolo));

                switch (item.LightModesItem.SelectedPoolLightMode)
                {
                    case PoolLightMode.American:
                        PoolLightAmericanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Blue:
                        PoolLightBlueModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.CaliforniaSunset:
                        PoolLightCaliSunsetModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Caribbean:
                        PoolLightCaribbeanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Green:
                        PoolLightGreenModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Magenta:
                        PoolLightMagentaModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Party:
                        PoolLightPartyModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Red:
                        PoolLightRedModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Romance:
                        PoolLightRomanceModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Royal:
                        PoolLightRoyalModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Sam:
                        PoolLightSamModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.White:
                        PoolLightWhiteModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Hold:
                        PoolLightHoldModeButton.SetBackgroundColor(selectedColor);
                        break;
                }

                switch (item.LightModesItem.SelectedSpaLightMode)
                {
                    case PoolLightMode.American:
                        SpaLightAmericanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Blue:
                        SpaLightBlueModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.CaliforniaSunset:
                        SpaLightCaliSunsetModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Caribbean:
                        SpaLightCaribbeanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Green:
                        SpaLightGreenModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Magenta:
                        SpaLightMagentaModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Party:
                        SpaLightPartyModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Red:
                        SpaLightRedModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Romance:
                        SpaLightRomanceModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Royal:
                        SpaLightRoyalModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Sam:
                        SpaLightSamModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.White:
                        SpaLightWhiteModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Hold:
                        SpaLightHoldModeButton.SetBackgroundColor(selectedColor);
                        break;

                }
            }
        }

        class AboutCell : RecyclerView.ViewHolder
        {
            public AboutCell(View view)
                : base(view)
            {
            }
        }
    }

}