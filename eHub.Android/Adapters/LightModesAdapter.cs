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
    public class LightModesAdapter: RecyclerView.Adapter
    {
        const int CellId = 1;

        readonly Handler _mainUiHandler;
        readonly IPoolService _poolService;

        public List<PoolLightGridItem> Items { get; set; } = new List<PoolLightGridItem>();

        public WeakReference ActivityRef { get; set; }

        public LightModesAdapter(List<PoolLightGridItem> items, IPoolService poolService)
        {
            _mainUiHandler = new Handler(Looper.MainLooper);
            _poolService = poolService;
            Items = items;
        }

        public override int ItemCount => Items?.Count ?? 0;

        public override int GetItemViewType(int position)
        {
            return CellId;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = Items[position];
            holder.ItemView.SetOnClickListener(new OnClickListener(v =>
            {
                
            }));

            /*
            lightModeCell.PoolLightAmericanModeButton.SetOnClickListener(new OnClickListener(async v =>
            {
                var result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(
                    new PoolLightModel(PoolLightMode.American), lightModeCell.PoolLightSelectedLightModeText);
                if (result != null)
                {
                    v.SetBackgroundColor(selectedColor);
                    DeactivateOtherLightButtons(lightModeCell.ItemView.Context, lightModeCell.PoolLightModeBtnContainer, v.Id);
                }
            }));
            */
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
            public LinearLayout PoolLightModeBtnContainer { get; }
            public TextView PoolLightExpandCollapseLabel { get; }

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
            public LinearLayout SpaLightModeBtnContainer { get; }
            public TextView SpaLightExpandCollapseLabel { get; }

            public LightModesCell(View view)
                : base (view)
            {
                PoolLightOnOffSwitch = view.FindViewById<Switch>(Resource.Id.pool_light_schedule_onoff_switch);
                PoolLightStartButton = view.FindViewById<Button>(Resource.Id.pool_light_schedule_begin_btn);
                PoolLightEndButton = view.FindViewById<Button>(Resource.Id.pool_light_schedule_end_btn);
                PoolLightSamModeButton = view.FindViewById<Button>(Resource.Id.pool_light_sam_mode_button);
                PoolLightPartyModeButton = view.FindViewById<Button>(Resource.Id.pool_light_party_mode_button);
                PoolLightRomanceModeButton = view.FindViewById<Button>(Resource.Id.pool_light_romance_mode_button);
                PoolLightCaribbeanModeButton = view.FindViewById<Button>(Resource.Id.pool_light_caribbean_mode_button);
                PoolLightAmericanModeButton = view.FindViewById<Button>(Resource.Id.pool_light_american_mode_button);
                PoolLightCaliSunsetModeButton = view.FindViewById<Button>(Resource.Id.pool_light_cali_sunset_mode_button);
                PoolLightRoyalModeButton = view.FindViewById<Button>(Resource.Id.pool_light_royal_mode_button);
                PoolLightBlueModeButton = view.FindViewById<Button>(Resource.Id.pool_light_blue_fixed_mode_button);
                PoolLightGreenModeButton = view.FindViewById<Button>(Resource.Id.pool_light_green_fixed_mode_button);
                PoolLightRedModeButton = view.FindViewById<Button>(Resource.Id.pool_light_red_fixed_mode_button);
                PoolLightWhiteModeButton = view.FindViewById<Button>(Resource.Id.pool_light_white_fixed_mode_button);
                PoolLightMagentaModeButton = view.FindViewById<Button>(Resource.Id.pool_light_magenta_fixed_mode_button);
                PoolLightHoldModeButton = view.FindViewById<Button>(Resource.Id.pool_light_hold_mode_button);
                PoolLightRecallModeButton = view.FindViewById<Button>(Resource.Id.pool_light_recall_mode_button);
                PoolLightSelectedLightModeText = view.FindViewById<TextView>(Resource.Id.pool_light_selected_light_mode_label);
                PoolLightModeBtnContainer = view.FindViewById<LinearLayout>(Resource.Id.pool_light_mode_btns_container);
                PoolLightExpandCollapseLabel = view.FindViewById<TextView>(Resource.Id.pool_light_modes_expand_textview);

                SpaLightOnOffSwitch = view.FindViewById<Switch>(Resource.Id.spa_light_schedule_onoff_switch);
                SpaLightStartButton = view.FindViewById<Button>(Resource.Id.spa_light_schedule_begin_btn);
                SpaLightEndButton = view.FindViewById<Button>(Resource.Id.spa_light_schedule_end_btn);
                SpaLightSamModeButton = view.FindViewById<Button>(Resource.Id.spa_light_sam_mode_button);
                SpaLightPartyModeButton = view.FindViewById<Button>(Resource.Id.spa_light_party_mode_button);
                SpaLightRomanceModeButton = view.FindViewById<Button>(Resource.Id.spa_light_romance_mode_button);
                SpaLightCaribbeanModeButton = view.FindViewById<Button>(Resource.Id.spa_light_caribbean_mode_button);
                SpaLightAmericanModeButton = view.FindViewById<Button>(Resource.Id.spa_light_american_mode_button);
                SpaLightCaliSunsetModeButton = view.FindViewById<Button>(Resource.Id.spa_light_cali_sunset_mode_button);
                SpaLightRoyalModeButton = view.FindViewById<Button>(Resource.Id.spa_light_royal_mode_button);
                SpaLightBlueModeButton = view.FindViewById<Button>(Resource.Id.spa_light_blue_fixed_mode_button);
                SpaLightGreenModeButton = view.FindViewById<Button>(Resource.Id.spa_light_green_fixed_mode_button);
                SpaLightRedModeButton = view.FindViewById<Button>(Resource.Id.spa_light_red_fixed_mode_button);
                SpaLightWhiteModeButton = view.FindViewById<Button>(Resource.Id.spa_light_white_fixed_mode_button);
                SpaLightMagentaModeButton = view.FindViewById<Button>(Resource.Id.spa_light_magenta_fixed_mode_button);
                SpaLightHoldModeButton = view.FindViewById<Button>(Resource.Id.spa_light_hold_mode_button);
                SpaLightRecallModeButton = view.FindViewById<Button>(Resource.Id.spa_light_recall_mode_button);
                SpaLightSelectedLightModeText = view.FindViewById<TextView>(Resource.Id.spa_light_selected_light_mode_label);
                SpaLightModeBtnContainer = view.FindViewById<LinearLayout>(Resource.Id.spa_light_mode_btns_container);
                SpaLightExpandCollapseLabel = view.FindViewById<TextView>(Resource.Id.spa_light_modes_expand_textview);
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
    }
}