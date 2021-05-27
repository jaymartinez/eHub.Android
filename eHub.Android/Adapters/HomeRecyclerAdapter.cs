using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
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
        const int PoolId = 2;
        const int SpaId = 3;
        const int BoosterId = 4;
        const int HeaterId = 5;
        const int GroundLightsId = 6;
        const int AboutId = 7;

        readonly Handler _mainUiHandler;
        readonly IPoolService _poolService;

        public List<HomeCellItem> Items { get; set; } = new List<HomeCellItem>();

        public WeakReference ActivityRef { get; set; }

        public HomeRecyclerAdapter(List<HomeCellItem> items, IPoolService poolService)
        {
            EhubInjector.InjectProperties(this);

            _mainUiHandler = new Handler(Looper.MainLooper);
            _poolService = poolService;
            Items = items;
        }

        public override int ItemCount => Items?.Count ?? 0;

        public override int GetItemViewType(int position)
        {
            var item = Items[position];

            switch (item.CellTypeObj)
            {
                case CellType.Schedule:
                    return ScheduleId;
                case CellType.Pool:
                    return PoolId;
                case CellType.Spa:
                    return SpaId;
                case CellType.Booster:
                    return BoosterId;
                case CellType.Heater:
                    return HeaterId;
                case CellType.GroundLights:
                    return GroundLightsId;
                case CellType.About:
                    return AboutId;
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

                case CellType.Pool:
                    var poolCell = holder as PoolCell;
                    poolCell.Bind(item);
                    var selectedColor = new Color(ContextCompat.GetColor(poolCell.ItemView.Context, Resource.Color.orangeHolo));

                    // Initial states
                    SetOnOffLabelColor(poolCell.StatusTextView, item.PoolItem.PoolPump);
                    SetButtonBackground(poolCell.OnOffButton, item.PoolItem.PoolPump.State, CellType.Pool);
                    poolCell.LightOnOffSwitch.Checked = item.PoolItem.PoolLight.State == 1;
                    poolCell.SelectedLightModeText.Text = item.PoolItem.SelectedLightMode.ToLightModeText();

                    poolCell.LightOnOffSwitch.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.PoolItem.LightOnOffSwitchTapped.Invoke(v as Switch);
                    }));

                    poolCell.SamModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Sam), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.PartyModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Party), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.RomanceModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Romance), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.CaribbeanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Caribbean), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.AmericanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.American), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.CaliSunsetModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.CaliforniaSunset), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.RoyalModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Royal), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.BlueModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Blue), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.GreenModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Green), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.RedModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Red), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.WhiteModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.White), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.MagentaModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Magenta), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.HoldModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Hold), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    poolCell.RecallModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.PoolItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Recall), poolCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(poolCell.ItemView.Context, poolCell.LightModeBtnContainer, v.Id);
                        }
                    }));

                    poolCell.OnOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var b = v as Button;
                        var heaterStatus = await GetStatus(Pin.Heater);
                        var boosterStatus = await GetStatus(Pin.BoosterPump);
                        var spaStatus = await GetStatus(Pin.SpaPump);
                        var curPoolState = await GetStatus(Pin.PoolPump);
                        var onOffStr = curPoolState == PinState.ON  ? "off" : "on";

                        if (curPoolState == PinState.ON 
                            && (heaterStatus == PinState.ON || boosterStatus == PinState.ON))
                        {
                            Toast.MakeText(v.Context, "Make sure the heater and the booster pump are off first!", 
                                ToastLength.Short).Show();
                            return;
                        }

                        Dialogs.Confirm(poolCell.ItemView.Context,
                            "Are You Sure?",
                            $"Are you sure you want to turn it {onOffStr}?",
                            "Yes", async (confirmed) =>
                            {
                                if (confirmed)
                                {
                                    var poolToggle = await _poolService.Toggle(Pin.PoolPump);
                                    if (poolToggle != null)
                                    {
                                        SetOnOffLabelColor(poolCell.StatusTextView, poolToggle);
                                        SetButtonBackground(b, poolToggle.State, CellType.Pool);
                                    }
                                }
                            }, "No").Show();
                    }));

                    break;

                case CellType.Spa:
                    var spaCell = holder as SpaCell;
                    spaCell.Bind(item);

                    // Initial states
                    spaCell.LightOnOffSwitch.Checked = item.SpaItem.SpaLight.State == 1;
                    spaCell.SelectedLightModeText.Text = item.SpaItem.SelectedLightMode.ToLightModeText();
                    SetOnOffLabelColor(spaCell.StatusTextView, item.SpaItem.SpaPump);
                    SetButtonBackground(spaCell.OnOffButton, item.SpaItem.SpaPump.State, CellType.Spa);

                    selectedColor = new Color(ContextCompat.GetColor(spaCell.ItemView.Context, Resource.Color.orangeHolo));

                    spaCell.LightOnOffSwitch.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.SpaItem.LightOnOffSwitchTapped.Invoke(v as Switch);
                    }));

                    spaCell.SamModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Sam), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.PartyModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Party), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.RomanceModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Romance), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.CaribbeanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Caribbean), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.AmericanModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.American), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.CaliSunsetModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.CaliforniaSunset), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.RoyalModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Royal), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.BlueModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Blue), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.GreenModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Green), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.RedModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Red), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.WhiteModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.White), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.MagentaModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Magenta), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.HoldModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Hold), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));
                    spaCell.RecallModeButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        if (await item.SpaItem.LightModeButtonTapped.Invoke(new PoolLightModel(PoolLightMode.Recall), spaCell.SelectedLightModeText))
                        {
                            v.SetBackgroundColor(selectedColor);
                            DeactivateOtherLightButtons(spaCell.ItemView.Context, spaCell.LightModeBtnContainer, v.Id);
                        }
                    }));

                    spaCell.OnOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var btn = v as Button;
                        var spaToggle = await _poolService.Toggle(Pin.SpaPump);
                        if (spaToggle != null)
                        {
                            SetOnOffLabelColor(spaCell.StatusTextView, spaToggle);
                            SetButtonBackground(btn, spaToggle.State, CellType.Spa);
                        }
                    }));

                    break;

                case CellType.GroundLights:
                case CellType.Booster:
                case CellType.Heater:
                    var eqmtCell = holder as EquipmentCell;
                    var checkPool = item.CellTypeObj == CellType.Booster || item.CellTypeObj == CellType.Heater;

                    SetOnOffLabelColor(eqmtCell.StatusTextView, item.SingleSwitchItem);
                    SetButtonBackground(eqmtCell.OnOffButton, item.SingleSwitchItem.State);

                    eqmtCell.OnOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var btn = v as Button;
                        if (checkPool)
                        {
                            var curStatus = await GetStatus(item.SingleSwitchItem.PinNumber);
                            var poolPumpStatus = await GetStatus(Pin.PoolPump);

                            // Make sure the pool pump is on first!
                            if (curStatus == PinState.OFF && poolPumpStatus == PinState.OFF)
                            {
                                Toast.MakeText(v.Context, "Wait! The pool pump needs to be on first!", 
                                    ToastLength.Short).Show();
                                return;
                            }
                        }

                        var toggle = await _poolService.Toggle(item.SingleSwitchItem.PinNumber);
                        if (toggle != null)
                        {
                            SetOnOffLabelColor(eqmtCell.StatusTextView, toggle);
                            SetButtonBackground(btn, toggle.State);
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
        void DeactivateOtherLightButtons(Context context, LinearLayout lightBtnContainer, int currentButtonId)
        {
            var defaultColor = new Color(ContextCompat.GetColor(context, Resource.Color.blue_gray_400));
            if (lightBtnContainer.ChildCount > 0)
            {
                for (var i = 0; i < lightBtnContainer.ChildCount; i++)
                {
                    var child = lightBtnContainer.GetChildAt(i);
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

        void SetOnOffLabelColor(TextView v, PiPin pin)
        {
            var offColor = new Color(
                ContextCompat.GetColor(v.Context, Resource.Color.orangeHolo));
            var onColor = new Color(
                ContextCompat.GetColor(v.Context, Resource.Color.greenLabel));

            _mainUiHandler.Post(() =>
            {
                if (pin.State == PinState.ON)
                {
                    v.Text = $"Active at {pin.DateActivated.ToLocalTime().ToShortTimeString()}";
                    v.SetTextColor(onColor);
                }
                else
                {
                    v.Text = "OFF";
                    v.SetTextColor(offColor);
                }
            });
        }

        void UpdateImageButtonState(ImageButton ib, int state)
        {
            _mainUiHandler.Post(() =>
            {
                if (state == PinState.ON)
                {
                    ib.SetBackgroundResource(Resource.Drawable.rounded_corners_green_8dp);
                    ib.SetImageResource(Resource.Drawable.icons8_light_on_96);
                }
                else
                {
                    ib.SetBackgroundResource(Resource.Drawable.rounded_corners_bluegray_8dp);
                    ib.SetImageResource(Resource.Drawable.icons8_bluelight_off_96);
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
                case PoolId:
                    var poolView = inflater.Inflate(Resource.Layout.item_pool_cell, parent, false);
                    return new PoolCell(poolView);
                case SpaId:
                    var spaView = inflater.Inflate(Resource.Layout.item_spa_cell, parent, false);
                    return new SpaCell(spaView);
                case BoosterId:
                    var boosterView = inflater.Inflate(Resource.Layout.item_booster_cell, parent, false);
                    return new BoosterCell(boosterView);
                case HeaterId:
                    var heaterView = inflater.Inflate(Resource.Layout.item_heater_cell, parent, false);
                    return new HeaterCell(heaterView);
                case GroundLightsId:
                    var glView = inflater.Inflate(Resource.Layout.item_groundlights_cell, parent, false);
                    return new GroundLightsCell(glView);
                case AboutId:
                    var aboutView = inflater.Inflate(Resource.Layout.item_about_cell, parent, false);
                    return new AboutCell(aboutView);
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

        class PoolCell : EquipmentCell
        {
            public Switch LightOnOffSwitch { get; }
            public Button SamModeButton { get; }
            public Button PartyModeButton { get; }
            public Button RomanceModeButton { get; }
            public Button CaribbeanModeButton { get; }
            public Button AmericanModeButton { get; }
            public Button CaliSunsetModeButton { get; }
            public Button RoyalModeButton { get; }
            public Button BlueModeButton { get; }
            public Button GreenModeButton { get; }
            public Button RedModeButton { get; }
            public Button WhiteModeButton { get; }
            public Button MagentaModeButton { get; }
            public Button HoldModeButton { get; }
            public Button RecallModeButton { get; }
            public TextView SelectedLightModeText { get; }
            public LinearLayout LightModeBtnContainer { get; }

            public PoolCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.pool_cell_status_data_lbl);
                OnOffButton = view.FindViewById<Button>(Resource.Id.pool_cell_pump_btn);
                LightOnOffSwitch = view.FindViewById<Switch>(Resource.Id.pool_cell_light_onoff_switch);
                SamModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_sam_mode_button);
                PartyModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_party_mode_button);
                RomanceModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_romance_mode_button);
                CaribbeanModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_caribbean_mode_button);
                AmericanModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_american_mode_button);
                CaliSunsetModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_cali_sunset_mode_button);
                RoyalModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_royal_mode_button);
                BlueModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_blue_fixed_mode_button);
                GreenModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_green_fixed_mode_button);
                RedModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_red_fixed_mode_button);
                WhiteModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_white_fixed_mode_button);
                MagentaModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_magenta_fixed_mode_button);
                HoldModeButton = view.FindViewById<Button>(Resource.Id.pool_cell_hold_mode_button);
                RecallModeButton = view.FindViewById<Button>(Resource.Id.pool_recall_mode_button);
                SelectedLightModeText = view.FindViewById<TextView>(Resource.Id.pool_cell_selected_light_mode_label);
                LightModeBtnContainer = view.FindViewById<LinearLayout>(Resource.Id.pool_cell_light_mode_btns_container);
            }

            internal void Bind(HomeCellItem item)
            {
                var selectedColor = new Color(ContextCompat.GetColor(ItemView.Context, Resource.Color.orangeHolo));

                switch (item.PoolItem.SelectedLightMode)
                {
                    case PoolLightMode.American:
                        AmericanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Blue:
                        BlueModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.CaliforniaSunset:
                        CaliSunsetModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Caribbean:
                        CaribbeanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Green:
                        GreenModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Magenta:
                        MagentaModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Party:
                        PartyModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Red:
                        RedModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Romance:
                        RomanceModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Royal:
                        RoyalModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Sam:
                        SamModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.White:
                        WhiteModeButton.SetBackgroundColor(selectedColor);
                        break;
                }
            }
        }

        class SpaCell : EquipmentCell
        {
            public Switch LightOnOffSwitch { get; }
            public Button SamModeButton { get; }
            public Button PartyModeButton { get; }
            public Button RomanceModeButton { get; }
            public Button CaribbeanModeButton { get; }
            public Button AmericanModeButton { get; }
            public Button CaliSunsetModeButton { get; }
            public Button RoyalModeButton { get; }
            public Button BlueModeButton { get; }
            public Button GreenModeButton { get; }
            public Button RedModeButton { get; }
            public Button WhiteModeButton { get; }
            public Button MagentaModeButton { get; }
            public Button HoldModeButton { get; }
            public Button RecallModeButton { get; }
            public TextView SelectedLightModeText { get; }
            public LinearLayout LightModeBtnContainer { get; }

            public SpaCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.spa_cell_status_data_lbl);
                OnOffButton = view.FindViewById<Button>(Resource.Id.spa_cell_onoff_btn);
                LightOnOffSwitch = view.FindViewById<Switch>(Resource.Id.spa_cell_light_onoff_switch);
                SamModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_sam_mode_button);
                PartyModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_party_mode_button);
                RomanceModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_romance_mode_button);
                CaribbeanModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_caribbean_mode_button);
                AmericanModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_american_mode_button);
                CaliSunsetModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_cali_sunset_mode_button);
                RoyalModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_royal_mode_button);
                BlueModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_blue_fixed_mode_button);
                GreenModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_green_fixed_mode_button);
                RedModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_red_fixed_mode_button);
                WhiteModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_white_fixed_mode_button);
                MagentaModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_magenta_fixed_mode_button);
                HoldModeButton = view.FindViewById<Button>(Resource.Id.spa_cell_hold_mode_button);
                RecallModeButton = view.FindViewById<Button>(Resource.Id.spa_recall_mode_button);
                SelectedLightModeText = view.FindViewById<TextView>(Resource.Id.spa_cell_selected_light_mode_label);
                LightModeBtnContainer = view.FindViewById<LinearLayout>(Resource.Id.spa_cell_light_mode_btns_container);
            }

            internal void Bind(HomeCellItem item)
            {
                var selectedColor = new Color(ContextCompat.GetColor(ItemView.Context, Resource.Color.orangeHolo));

                switch (item.SpaItem.SelectedLightMode)
                {
                    case PoolLightMode.American:
                        AmericanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Blue:
                        BlueModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.CaliforniaSunset:
                        CaliSunsetModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Caribbean:
                        CaribbeanModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Green:
                        GreenModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Magenta:
                        MagentaModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Party:
                        PartyModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Red:
                        RedModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Romance:
                        RomanceModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Royal:
                        RoyalModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.Sam:
                        SamModeButton.SetBackgroundColor(selectedColor);
                        break;
                    case PoolLightMode.White:
                        WhiteModeButton.SetBackgroundColor(selectedColor);
                        break;
                }
            }
        }

        class BoosterCell : EquipmentCell
        {
            public BoosterCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.booster_cell_status_data_lbl);
                OnOffButton = view.FindViewById<Button>(Resource.Id.booster_cell_onoff_btn);
            }
        }

        class HeaterCell : EquipmentCell
        {
            public HeaterCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.heater_cell_status_data_lbl);
                OnOffButton = view.FindViewById<Button>(Resource.Id.heater_cell_onoff_btn);
            }
        }

        class GroundLightsCell : EquipmentCell
        {
            public GroundLightsCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.groundlights_cell_status_data_lbl);
                OnOffButton = view.FindViewById<Button>(Resource.Id.groundlights_cell_onoff_btn);
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