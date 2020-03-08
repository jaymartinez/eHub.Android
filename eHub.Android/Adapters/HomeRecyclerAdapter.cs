using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Android.Models;
using eHub.Common.Models;
using eHub.Common.Services;

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

        public List<HomeCellItem> Items { get; set; } = new List<HomeCellItem>();


        public WeakReference ActivityRef { get; set; }

        [Inject] IPoolService PoolService { get; set; }

        public HomeRecyclerAdapter(List<HomeCellItem> items)
        {
            EhubInjector.InjectProperties(this);

            _mainUiHandler = new Handler(Looper.MainLooper);
            Items = items;
        }

        public override int ItemCount => Items?.Count ?? 0;

        public override int GetItemViewType(int position)
        {
            var item = Items[position] as HomeCellItem;

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
            var item = Items[position] as HomeCellItem;

            switch (item.CellTypeObj)
            {
                case CellType.Schedule:
                    var schCell = holder as ScheduleCell;
                    var startTime = new TimeSpan(item.ScheduleCellItem.Schedule.StartHour, item.ScheduleCellItem.Schedule.StartMinute, 0);
                    var endTime = new TimeSpan(item.ScheduleCellItem.Schedule.EndHour, item.ScheduleCellItem.Schedule.EndMinute, 0);
                    schCell.StartButton.Text = startTime.ToString(@"%h\:mm");
                    schCell.EndButton.Text = endTime.ToString(@"%h\:mm");
                    schCell.EnabledCheckbox.Checked = item.ScheduleCellItem.Schedule.IsActive;

                    schCell.StartButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.ScheduleCellItem.StartTapped.Invoke(v as Button);
                    }));

                    schCell.EndButton.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.ScheduleCellItem.EndTapped.Invoke(v as Button);
                    }));

                    schCell.EnabledCheckbox.SetOnClickListener(new OnClickListener(v =>
                    {
                        item.ScheduleCellItem.EnabledCheckboxTapped.Invoke(v as CheckBox);
                    }));
                    break;

                case CellType.Pool:
                    var poolCell = holder as PoolCell;

                    // Initial states
                    SetOnOffLabelColor(poolCell.StatusTextView, item.PoolItem.PoolPump);
                    SetButtonBackground(poolCell.OnOffButton, item.PoolItem.PoolPump.State);
                    UpdateImageButtonState(poolCell.LightButton, item.PoolItem.PoolLight.State);

                    poolCell.LightButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var poolLight = await PoolService.Toggle(Pin.PoolLight);
                        if (poolLight != null)
                        {
                            UpdateImageButtonState(poolCell.LightButton, poolLight.State);
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

                        if (curPoolState == PinState.ON && (heaterStatus == PinState.ON
                            || boosterStatus == PinState.ON
                            || spaStatus == PinState.ON))
                        {
                            Toast.MakeText(v.Context, "One of the other pumps are still on, turn those off first!", 
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
                                    var poolToggle = await PoolService.Toggle(Pin.PoolPump);
                                    if (poolToggle != null)
                                    {
                                        SetOnOffLabelColor(poolCell.StatusTextView, poolToggle);
                                        SetButtonBackground(b, poolToggle.State);
                                    }
                                }
                            }, "No").Show();
                    }));

                    break;

                case CellType.Spa:
                    var spaCell = holder as SpaCell;

                    // Initial states
                    UpdateImageButtonState(spaCell.LightButton, item.SpaItem.SpaLight.State);
                    SetOnOffLabelColor(spaCell.StatusTextView, item.SpaItem.SpaPump);
                    SetButtonBackground(spaCell.OnOffButton, item.SpaItem.SpaPump.State);

                    spaCell.LightButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var spaLight = await PoolService.Toggle(Pin.SpaLight);
                        if (spaLight != null)
                        {
                            UpdateImageButtonState(spaCell.LightButton, spaLight.State);
                        }
                    }));

                    spaCell.OnOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var btn = v as Button;
                        var spaToggle = await PoolService.Toggle(Pin.SpaPump);
                        if (spaToggle != null)
                        {
                            SetOnOffLabelColor(spaCell.StatusTextView, spaToggle);
                            SetButtonBackground(btn, spaToggle.State);
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

                        var toggle = await PoolService.Toggle(item.SingleSwitchItem.PinNumber);
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

        async Task<int> GetStatus(int pin)
        {
            var result = await PoolService.GetPinStatus(pin);
            return result.State;
        }

        void SetButtonBackground(Button b, int state)
        {
            _mainUiHandler.Post(() =>
            {
                if (state == PinState.ON)
                {
                    b.SetBackgroundResource(Resource.Drawable.rounded_corners_green_8dp);
                    b.Text = "ON";
                }
                else
                {
                    b.SetBackgroundResource(Resource.Drawable.rounded_corners_bluegray_8dp);
                    b.Text = "OFF";
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
            public CheckBox EnabledCheckbox { get; }

            public ScheduleCell(View view)
                : base(view)
            {
                StartButton = view.FindViewById<Button>(Resource.Id.schedule_cell_begin_btn);
                EndButton = view.FindViewById<Button>(Resource.Id.schedule_cell_end_btn);
                EnabledCheckbox = view.FindViewById<CheckBox>(Resource.Id.schedule_enabled_cb);
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
            public ImageButton LightButton { get; }

            public PoolCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.pool_cell_status_data_lbl);
                OnOffButton = view.FindViewById<Button>(Resource.Id.pool_cell_pump_btn);
                LightButton = view.FindViewById<ImageButton>(Resource.Id.pool_cell_light_btn);
            }
        }

        class SpaCell : EquipmentCell
        {
            public ImageButton LightButton { get; }

            public SpaCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.spa_cell_status_data_lbl);
                OnOffButton = view.FindViewById<Button>(Resource.Id.spa_cell_onoff_btn);
                LightButton = view.FindViewById<ImageButton>(Resource.Id.spa_cell_light_btn);
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