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
                    var poolCell = holder as EquipmentCell;
                    poolCell.OnOffSwitch.Checked = item.PoolItem.PoolPump.State == PinState.ON;

                    SetOnOffLabelColor(poolCell.StatusTextView, item.PoolItem.PoolPump.State, true);

                    // Set light tap listener
                    poolCell.LightImageView.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var light = await PoolService.Toggle(Pin.PoolLight);
                        if (light != null && v is ImageView img)
                        {
                            SetLightImageResource(img, light.State);
                        }
                    }));

                    poolCell.OnOffSwitch.SetOnCheckedChangeListener(new OnCheckChangedListener(async (v, r) =>
                    {
                        var heaterStatus = await GetStatus(Pin.Heater);
                        var boosterStatus = await GetStatus(Pin.BoosterPump);
                        var spaStatus = await GetStatus(Pin.SpaPump);
                        var curPoolState = await GetStatus(Pin.PoolPump);
                        var onOffStr = curPoolState == PinState.ON  ? "off" : "on";

                        if (curPoolState == PinState.ON && v.Checked
                            || curPoolState == PinState.OFF && !v.Checked)
                        {
                            return;
                        }

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
                                        SetOnOffLabelColor(poolCell.StatusTextView, poolToggle.State, true);
                                    }
                                }
                                else
                                {
                                    v.Checked = curPoolState == PinState.ON ? true : false;
                                }
                            }, "No").Show();
                    }));

                    // Initial state
                    SetLightImageResource(poolCell.LightImageView, item.PoolItem.PoolLight.State);
                    break;

                case CellType.Spa:
                    var spaCell = holder as EquipmentCell;
                    spaCell.OnOffSwitch.Checked = item.SpaItem.SpaPump.State == PinState.ON;

                    SetOnOffLabelColor(spaCell.StatusTextView, item.SpaItem.SpaPump.State, true);

                    spaCell.LightImageView.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var spaLight = await PoolService.Toggle(Pin.SpaLight);
                        if (spaLight != null && v is ImageView img)
                        {
                            SetLightImageResource(img, spaLight.State);
                        }
                    }));

                    spaCell.OnOffSwitch.SetOnCheckedChangeListener(new OnCheckChangedListener(async (v, r) =>
                    {
                        var spaToggle = await PoolService.Toggle(Pin.SpaPump);
                        if (spaToggle != null)
                        {
                            SetOnOffLabelColor(spaCell.StatusTextView, spaToggle.State, true);
                        }
                    }));

                    // Initial state
                    SetLightImageResource(spaCell.LightImageView, item.SpaItem.SpaLight.State);
                    break;

                case CellType.GroundLights:
                case CellType.Booster:
                case CellType.Heater:
                    var eqmtCell = holder as EquipmentCell;
                    var checkPool = item.CellTypeObj == CellType.Booster || item.CellTypeObj == CellType.Heater;
                    eqmtCell.OnOffSwitch.Checked = item.SingleSwitchItem.State == PinState.ON;

                    SetOnOffLabelColor(eqmtCell.StatusTextView, item.SingleSwitchItem.State, false);

                    eqmtCell.OnOffSwitch.SetOnCheckedChangeListener(new OnCheckChangedListener(async (v, r) =>
                    {
                        if (checkPool)
                        {
                            var curStatus = await GetStatus(item.SingleSwitchItem.PinNumber);

                            // Make sure the pool pump is on first!
                            var poolPumpStatus = await GetStatus(Pin.PoolPump);
                            if (curStatus == PinState.OFF && poolPumpStatus == PinState.OFF)
                            {
                                Toast.MakeText(v.Context, "Wait! The pool pump needs to be on first!", 
                                    ToastLength.Short).Show();
                                v.Checked = false;
                                return;
                            }
                        }

                        var toggle = await PoolService.Toggle(item.SingleSwitchItem.PinNumber);
                        if (toggle != null)
                        {
                            SetOnOffLabelColor(eqmtCell.StatusTextView, toggle.State, false);
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

        void SetOnOffLabelColor(TextView v, int state, bool spaOrPool)
        {
            var offColor = new Color(
                ContextCompat.GetColor(v.Context, Resource.Color.redLabel));
            var onColor = new Color(
                ContextCompat.GetColor(v.Context, Resource.Color.greenLabel));

            _mainUiHandler.Post(() =>
            {
                if (state == PinState.ON)
                {
                    v.Text = spaOrPool ? "PUMP ON" : "ON";
                    v.SetTextColor(onColor);
                }
                else
                {
                    v.Text = spaOrPool ? "PUMP OFF" : "OFF";
                    v.SetTextColor(offColor);
                }
            });
        }

        void SetLightImageResource(ImageView v, int state)
        {
            _mainUiHandler.Post(() =>
            {
                if (state == PinState.ON)
                {
                    v.SetImageResource(Resource.Drawable.icons8_light_on_96);
                }
                else
                {
                    v.SetImageResource(Resource.Drawable.icons8_light_off_96);
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
            public ImageView LightImageView { get; set; }
            public TextView StatusTextView { get; set; }
            public Switch OnOffSwitch { get; set; }

            public EquipmentCell(View view)
                : base(view) { }
        }

        class PoolCell : EquipmentCell
        {
            public PoolCell(View view)
                : base(view)
            {
                LightImageView = view.FindViewById<ImageView>(Resource.Id.pool_cell_light_switch_img);
                StatusTextView = view.FindViewById<TextView>(Resource.Id.pool_cell_status_data_lbl);
                OnOffSwitch = view.FindViewById<Switch>(Resource.Id.pool_cell_onoff_switch);
            }
        }

        class SpaCell : EquipmentCell
        {
            public SpaCell(View view)
                : base(view)
            {
                LightImageView = view.FindViewById<ImageView>(Resource.Id.spa_cell_light_switch_img);
                StatusTextView = view.FindViewById<TextView>(Resource.Id.spa_cell_status_data_lbl);
                OnOffSwitch = view.FindViewById<Switch>(Resource.Id.spa_cell_onoff_switch);
            }
        }

        class BoosterCell : EquipmentCell
        {
            public BoosterCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.booster_cell_status_data_lbl);
                OnOffSwitch = view.FindViewById<Switch>(Resource.Id.booster_cell_onoff_switch);
            }
        }

        class HeaterCell : EquipmentCell
        {
            public HeaterCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.heater_cell_status_data_lbl);
                OnOffSwitch = view.FindViewById<Switch>(Resource.Id.heater_cell_onoff_switch);
            }
        }

        class GroundLightsCell : EquipmentCell
        {
            public GroundLightsCell(View view)
                : base(view)
            {
                StatusTextView = view.FindViewById<TextView>(Resource.Id.groundlights_cell_status_data_lbl);
                OnOffSwitch = view.FindViewById<Switch>(Resource.Id.groundlights_cell_onoff_switch);
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