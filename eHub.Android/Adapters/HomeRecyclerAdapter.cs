using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Fragments;
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

        List<HomeCellItem> _items;

        [Inject] IPoolService PoolService { get; set; }

        public HomeRecyclerAdapter(List<HomeCellItem> items)
        {
            EhubInjector.InjectProperties(this);

            _mainUiHandler = new Handler(Looper.MainLooper);
            _items = items;
        }

        public override int ItemCount => _items?.Count ?? 0;

        public override int GetItemViewType(int position)
        {
            var item = _items[position] as HomeCellItem;

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
            var item = _items[position] as HomeCellItem;

            switch (item.CellTypeObj)
            {
                case CellType.Schedule:
                    var schCell = holder as ScheduleCell;
                    var startTime = new TimeSpan(item.PoolScheduleObj.StartHour, item.PoolScheduleObj.StartMinute, 0);
                    var endTime = new TimeSpan(item.PoolScheduleObj.EndHour, item.PoolScheduleObj.EndMinute, 0);
                    schCell.StartButton.Text = startTime.ToString(@"%h\:mm");
                    schCell.EndButton.Text = endTime.ToString(@"%h\:mm");
                    schCell.EnabledCheckbox.Enabled = item.PoolScheduleObj.IsActive;
                    break;

                case CellType.Pool:
                    var poolCell = holder as EquipmentCell;
                    poolCell.OnOffSwitch.Selected = item.PoolItem.PoolPump.State == PinState.ON;

                    poolCell.LightImageView.SetOnClickListener(new OnClickListener(v =>
                    {
                    }));

                    if (item.PoolItem.PoolLight.State == PinState.ON)
                    {
                        poolCell.LightImageView.SetImageResource(Resource.Drawable.icons8_light_on_48);
                    }
                    else
                    {
                        poolCell.LightImageView.SetImageResource(Resource.Drawable.icons8_light_off_48);
                    }
                    break;

                case CellType.Spa:
                    var spaCell = holder as EquipmentCell;
                    spaCell.OnOffSwitch.Selected = item.SpaItem.SpaPump.State == PinState.ON;

                    spaCell.LightImageView.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var st = await PoolService.Toggle(Pin.SpaLight);
                    }));

                    if (item.SpaItem.SpaLight.State == PinState.ON)
                    {
                        spaCell.LightImageView.SetImageResource(Resource.Drawable.icons8_light_on_48);
                    }
                    else
                    {
                        spaCell.LightImageView.SetImageResource(Resource.Drawable.icons8_light_off_48);
                    }
                    break;

                case CellType.GroundLights:
                case CellType.Booster:
                case CellType.Heater:
                    var eqmtCell = holder as EquipmentCell;
                    eqmtCell.OnOffSwitch.Selected = item.SingleSwitchItem.State == PinState.ON;
                    if (item.SingleSwitchItem.PinNumber == Pin.GroundLights)
                    {
                    }
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