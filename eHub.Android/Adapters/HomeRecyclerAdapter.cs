using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using eHub.Android.Fragments;
using eHub.Android.Listeners;
using eHub.Android.Models;
using eHub.Common.Models;
using eHub.Common.Services;
using Switch = AndroidX.AppCompat.Widget.SwitchCompat;

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
                    schCell.Bind(item.ScheduleCellItem, _mainUiHandler, _poolService);
                    break;

                case CellType.LightModes:
                    var lightModeCell = holder as LightModesCell;
                    var poolLightStart = item.LightModesItem.PoolLightSchedule.StartTime;
                    var poolLightEnd = item.LightModesItem.PoolLightSchedule.EndTime;
                    var spaLightStart = item.LightModesItem.SpaLightSchedule.StartTime;
                    var spaLightEnd = item.LightModesItem.SpaLightSchedule.EndTime;

                    lightModeCell.PoolLightStartButton.Text = poolLightStart.ToString(@"%h\:mm");
                    lightModeCell.PoolLightEndButton.Text = poolLightEnd.ToString(@"%h\:mm");
                    lightModeCell.SpaLightStartButton.Text = spaLightStart.ToString(@"%h\:mm");
                    lightModeCell.SpaLightEndButton.Text = spaLightEnd.ToString(@"%h\:mm");

                    lightModeCell.PoolLightOnOffSwitch.Checked = item.LightModesItem.PoolLightSchedule.IsActive;
                    lightModeCell.SpaLightOnOffSwitch.Checked = item.LightModesItem.SpaLightSchedule.IsActive;

                    Dictionary<string, Tuple<Button, CardView>> BuildLightDict(GridLayout gridLayout)
                    {
                        // store off light controls
                        var dict = new Dictionary<string, Tuple<Button, CardView>>();
                        for (var i = 0; i < gridLayout.ChildCount; i++)
                        {
                            if (gridLayout.GetChildAt(i) is RelativeLayout rl)
                            {
                                var btn = default(Button);
                                var cv = default(CardView);
                                for (var j = 0; j < rl.ChildCount; j++)
                                {
                                    if (rl.GetChildAt(j) is Button button)
                                    {
                                        btn = button;
                                    }
                                    else if (rl.GetChildAt(j) is CardView cardView)
                                    {
                                        cv = cardView;
                                    }
                                }

                                if (!dict.ContainsKey(rl.Tag.ToString()))
                                {
                                    dict.Add(rl.Tag.ToString(), new Tuple<Button, CardView>(btn, cv));
                                }
                            }
                        }

                        return dict;
                    }

                    var poolLightDict = BuildLightDict(lightModeCell.PoolLightModeGrid);
                    var spaLightDict = BuildLightDict(lightModeCell.SpaLightModeGrid);

                    lightModeCell.Bind(item, poolLightDict, spaLightDict);

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

                    var selectedColor = new Color(ContextCompat.GetColor(lightModeCell.ItemView.Context, Resource.Color.orangeHolo));

                    void DeactivateOtherSelection(string tagToIgnore, Dictionary<string, Tuple<Button, CardView>> dict)
                    {
                        foreach (var kvp in dict)
                        {
                            if (kvp.Key == tagToIgnore) continue;

                            if (kvp.Value.Item2 != null)
                            {
                                kvp.Value.Item2.Visibility = ViewStates.Gone;
                            }
                        }
                    }

                    void SetupLightClickHandlers(Dictionary<string, Tuple<Button, CardView>> dict, LightType lightType)
                    {
                        foreach (var kvp in dict)
                        {
                            kvp.Value.Item1.SetOnClickListener(new OnClickListener(async v =>
                            {
                                var curMode = Enum.Parse<LightModeType>(kvp.Key);
                                var result = default(LightModel);
                                if (lightType == LightType.Pool)
                                {
                                    result = await item.LightModesItem.PoolLightModeButtonTapped.Invoke(curMode);
                                }
                                else
                                {
                                    result = await item.LightModesItem.SpaLightModeButtonTapped.Invoke(curMode);
                                }

                                if (result != null)
                                {
                                    // select the mode returned
                                    if (dict.TryGetValue(((int)result.Mode).ToString(), out var entry)
                                        && entry.Item2 != null)
                                    {
                                        entry.Item2.Visibility = ViewStates.Visible;
                                        DeactivateOtherSelection(((int)result.Mode).ToString(), dict);

                                    }
                                }
                                
                            }));
                        }
                    }

                    SetupLightClickHandlers(poolLightDict, LightType.Pool);
                    SetupLightClickHandlers(spaLightDict, LightType.Spa);
                    
                    break;

                case CellType.DeviceControl:
                    var devicesCell = holder as DevicesCell;

                    // Set initial states
                    var pool = item.DevicesItem.PoolModel;
                    var spa = item.DevicesItem.SpaModel;
                    var booster = item.DevicesItem.BoosterModel;
                    var heater = item.DevicesItem.HeaterModel;

                    ButtonHelper.ToggleOnOffButtonStyle(devicesCell.PoolOnButton, devicesCell.PoolOffButton, pool.State, devicesCell.ItemView.Context);
                    ButtonHelper.ToggleOnOffButtonStyle(devicesCell.SpaOnButton, devicesCell.SpaOffButton, spa.State, devicesCell.ItemView.Context);
                    ButtonHelper.ToggleOnOffButtonStyle(devicesCell.BoosterOnButton, devicesCell.BoosterOffButton, booster.State, devicesCell.ItemView.Context);
                    ButtonHelper.ToggleOnOffButtonStyle(devicesCell.HeaterOnButton, devicesCell.HeaterOffButton, heater.State, devicesCell.ItemView.Context);
                    ButtonHelper.ToggleOnOffButtonStyle(devicesCell.PoolLightOnButton, devicesCell.PoolLightOffButton, pool.Light.State, devicesCell.ItemView.Context);
                    ButtonHelper.ToggleOnOffButtonStyle(devicesCell.SpaLightOnButton, devicesCell.SpaLightOffButton, spa.Light.State, devicesCell.ItemView.Context);

                    devicesCell.PoolOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var pool = await _poolService.GetPool();

                        // if already on bail out
                        if (pool.State == PinState.ON)
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
                                    var poolToggle = await _poolService.Toggle(PinType.PoolPump);
                                    if (poolToggle != null)
                                    {
                                        ButtonHelper.ToggleOnOffButtonStyle(
                                            onButton,
                                            devicesCell.PoolOffButton,
                                            poolToggle.State,
                                            devicesCell.ItemView.Context);
                                    }
                                }
                            }, "No").Show();
                    }));

                    devicesCell.PoolOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var pool = await _poolService.GetPool();

                        // if already off bail out
                        if (pool.State == PinState.OFF)
                        {
                            return;
                        }

                        var offButton = v as Button;
                        var heater = await _poolService.GetHeater();
                        var booster = await _poolService.GetBoosterPump();

                        if (pool.State == PinState.ON
                            && (heater.State == PinState.ON || booster.State == PinState.ON))
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
                                    var poolToggle = await _poolService.Toggle(PinType.PoolPump);
                                    if (poolToggle != null)
                                    {
                                        ButtonHelper.ToggleOnOffButtonStyle(
                                            devicesCell.PoolOnButton,
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
                        var booster = await _poolService.GetBoosterPump();

                        if (booster.State == PinState.ON)
                        {
                            return;
                        }

                        // Make sure the pool pump is on first!
                        var pool = await _poolService.GetPool();
                        if (pool.State == PinState.OFF)
                        {
                            Toast.MakeText(v.Context, "Wait! The pool pump needs to be on first!",
                                ToastLength.Short).Show();
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.BoosterPump);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                onButton,
                                devicesCell.BoosterOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.BoosterOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var booster = await _poolService.GetBoosterPump();

                        if (booster.State == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.BoosterPump);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                devicesCell.BoosterOnButton,
                                offButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.HeaterOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var heater = await _poolService.GetHeater();

                        if (heater.State == PinState.ON)
                        {
                            return;
                        }

                        // Make sure the pool pump is on first!
                        var pool = await _poolService.GetPool();
                        if (pool.State == PinState.OFF)
                        {
                            Toast.MakeText(v.Context, "Wait! The pool pump needs to be on first!",
                                ToastLength.Short).Show();
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.Heater);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                onButton,
                                devicesCell.HeaterOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.HeaterOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var heater = await _poolService.GetHeater();

                        if (heater.State == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.Heater);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                devicesCell.HeaterOnButton,
                                offButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var spa = await _poolService.GetSpa();

                        if (spa.State == PinState.ON)
                        {
                            return;
                        }

                        var spaToggle = await _poolService.Toggle(PinType.SpaPump);
                        if (spaToggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                onButton,
                                devicesCell.SpaOffButton,
                                spaToggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var spa = await _poolService.GetSpa();

                        if (spa.State == PinState.OFF)
                        {
                            return;
                        }

                        var spaToggle = await _poolService.Toggle(PinType.SpaPump);
                        if (spaToggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                devicesCell.SpaOnButton,
                                offButton,
                                spaToggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.PoolLightOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var poolLight = (await _poolService.GetPool()).Light;

                        if (poolLight.State == PinState.ON)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.PoolLight);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                onButton,
                                devicesCell.PoolLightOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.PoolLightOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var poolLight = (await _poolService.GetPool()).Light;

                        if (poolLight.State == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.PoolLight);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                devicesCell.PoolLightOnButton,
                                offButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaLightOnButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var onButton = v as Button;
                        var spaLight = (await _poolService.GetSpa()).Light;

                        if (spaLight.State == PinState.ON)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.SpaLight);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                onButton,
                                devicesCell.SpaLightOffButton,
                                toggle.State,
                                devicesCell.ItemView.Context);
                        }
                    }));

                    devicesCell.SpaLightOffButton.SetOnClickListener(new OnClickListener(async v =>
                    {
                        var offButton = v as Button;
                        var spaLight = (await _poolService.GetSpa()).Light;

                        if (spaLight.State == PinState.OFF)
                        {
                            return;
                        }

                        var toggle = await _poolService.Toggle(PinType.SpaLight);
                        if (toggle != null)
                        {
                            ButtonHelper.ToggleOnOffButtonStyle(
                                devicesCell.SpaLightOnButton,
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

        void SetButtonBackground(Button b, PinState state, CellType? cellType = null)
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
            public Button PoolOnButton { get; }
            public Button PoolOffButton { get; }
            public Button PoolStartButton { get; }
            public Button PoolEndButton { get; }

            public Button BoosterOnButton { get; }
            public Button BoosterOffButton { get; }
            public Button BoosterStartButton { get; }
            public Button BoosterEndButton { get; }

            public ScheduleCell(View view)
                : base(view)
            {
                PoolStartButton = view.FindViewById<Button>(Resource.Id.schedule_cell_pool_begin_btn);
                PoolEndButton = view.FindViewById<Button>(Resource.Id.schedule_cell_pool_end_btn);
                PoolOnButton = view.FindViewById<Button>(Resource.Id.schedule_cell_pool_on_btn);
                PoolOffButton = view.FindViewById<Button>(Resource.Id.schedule_cell_pool_off_btn);

                BoosterStartButton = view.FindViewById<Button>(Resource.Id.schedule_cell_booster_begin_btn);
                BoosterEndButton = view.FindViewById<Button>(Resource.Id.schedule_cell_booster_end_btn);
                BoosterOnButton = view.FindViewById<Button>(Resource.Id.schedule_cell_booster_on_btn);
                BoosterOffButton = view.FindViewById<Button>(Resource.Id.schedule_cell_booster_off_btn);
            }

            internal void Bind(ScheduleCellItem item, Handler handler, IPoolService poolService)
            {
                var poolStartTime = new TimeSpan(item.PoolSchedule.StartTime.Hours, item.PoolSchedule.StartTime.Minutes, 0);
                var poolEndTime = new TimeSpan(item.PoolSchedule.EndTime.Hours, item.PoolSchedule.EndTime.Minutes, 0);
                var boosterStartTime = new TimeSpan(item.BoosterSchedule.StartTime.Hours, item.BoosterSchedule.StartTime.Minutes, 0);
                var boosterEndTime = new TimeSpan(item.BoosterSchedule.EndTime.Hours, item.BoosterSchedule.EndTime.Minutes, 0);

                PoolStartButton.Text = poolStartTime.ToString(@"%h\:mm");
                PoolEndButton.Text = poolEndTime.ToString(@"%h\:mm");

                BoosterStartButton.Text = boosterStartTime.ToString(@"%h\:mm");
                BoosterEndButton.Text = boosterEndTime.ToString(@"%h\:mm");

                // initial pool timer on/off button states
                ButtonHelper.ToggleOnOffButtonStyle(
                    PoolOnButton, 
                    PoolOffButton, 
                    item.PoolSchedule.IsActive ? PinState.ON : PinState.OFF, 
                    ItemView.Context);

                // initial booster timer on/off button states
                ButtonHelper.ToggleOnOffButtonStyle(
                    BoosterOnButton, 
                    BoosterOffButton, 
                    item.BoosterSchedule.IsActive ? PinState.ON : PinState.OFF, 
                    ItemView.Context);
                
                // Set up click handlers for the on/off buttons
                PoolStartButton.SetOnClickListener(new OnClickListener(async v =>
                {
                    var pool = await poolService.GetPool();
                    pool.Schedule.Type = ScheduleType.Pool;
                    item.StartTapped.Invoke(v as Button, pool.Schedule);
                }));
                PoolEndButton.SetOnClickListener(new OnClickListener(async v =>
                {
                    var pool = await poolService.GetPool();
                    pool.Schedule.Type = ScheduleType.Pool;
                    item.EndTapped.Invoke(v as Button, pool.Schedule);
                }));

                BoosterStartButton.SetOnClickListener(new OnClickListener(async v =>
                {
                    var booster = await poolService.GetBoosterPump();
                    booster.Schedule.Type = ScheduleType.Booster;
                    item.StartTapped.Invoke(v as Button, booster.Schedule);
                }));
                BoosterEndButton.SetOnClickListener(new OnClickListener(async v =>
                {
                    var booster = await poolService.GetBoosterPump();
                    booster.Schedule.Type = ScheduleType.Booster;
                    item.EndTapped.Invoke(v as Button, booster.Schedule);
                }));

                PoolOnButton.SetOnClickListener(new OnClickListener(async onButton =>
                {
                    var pool = await poolService.GetPool();
                    pool.Schedule.Type = ScheduleType.Pool;
                    item.OnButtonTapped.Invoke(onButton as Button, PoolOffButton, pool.Schedule);
                }));
                PoolOffButton.SetOnClickListener(new OnClickListener(async offButton =>
                {
                    var pool = await poolService.GetPool();
                    pool.Schedule.Type = ScheduleType.Pool;
                    item.OffButtonTapped.Invoke(PoolOnButton, offButton as Button, pool.Schedule);
                }));
                BoosterOnButton.SetOnClickListener(new OnClickListener(async onButton =>
                {
                    var booster = await poolService.GetBoosterPump();
                    booster.Schedule.Type = ScheduleType.Booster;
                    item.OnButtonTapped.Invoke(onButton as Button, BoosterOffButton, booster.Schedule);
                }));
                BoosterOffButton.SetOnClickListener(new OnClickListener(async offButton =>
                {
                    var booster = await poolService.GetBoosterPump();
                    booster.Schedule.Type = ScheduleType.Booster;
                    item.OffButtonTapped.Invoke(BoosterOnButton, offButton as Button, booster.Schedule);
                }));
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
            public TextView PoolLightSelectedLightModeText { get; }
            public GridLayout PoolLightModeGrid { get; }
            public ImageView PoolLightLegendImageView { get; }
            public Switch SpaLightOnOffSwitch { get; }
            public Button SpaLightStartButton { get; }
            public Button SpaLightEndButton { get; }
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
                PoolLightSelectedLightModeText = view.FindViewById<TextView>(Resource.Id.pool_light_selected_light_mode_label);
                SpaLightLegendImageView = view.FindViewById<ImageView>(Resource.Id.spa_light_modes_legend);
                SpaLightModeGrid = view.FindViewById<GridLayout>(Resource.Id.spa_light_grid_layout);
                SpaLightOnOffSwitch = view.FindViewById<Switch>(Resource.Id.spa_light_schedule_onoff_switch);
                SpaLightStartButton = view.FindViewById<Button>(Resource.Id.spa_light_schedule_begin_btn);
                SpaLightEndButton = view.FindViewById<Button>(Resource.Id.spa_light_schedule_end_btn);
                SpaLightSelectedLightModeText = view.FindViewById<TextView>(Resource.Id.spa_light_selected_light_mode_label);
            }

            internal void Bind(
                HomeCellItem item, 
                Dictionary<string, Tuple<Button, CardView>> poolLightDict,
                Dictionary<string, Tuple<Button, CardView>> spaLightDict)
            {
                void SetSelectedGridItem(Dictionary<string, Tuple<Button, CardView>> dict, LightModeType selectedLightMode)
                {
                    foreach (var kvp in dict)
                    {
                        if (kvp.Value.Item2 == null) continue;

                        if (Enum.TryParse<LightModeType>(kvp.Key, out var mode)
                            && mode.Equals(selectedLightMode))
                        {
                            kvp.Value.Item2.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            kvp.Value.Item2.Visibility = ViewStates.Gone;
                        }

                    }
                }

                SetSelectedGridItem(poolLightDict, item.LightModesItem.SelectedPoolLightMode);
                SetSelectedGridItem(spaLightDict, item.LightModesItem.SelectedSpaLightMode);
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

    internal static class ButtonHelper
    {
        public static void ToggleOnOffButtonStyle(
            Button onButton,
            Button offButton,
            PinState state,
            Context context)
        {
            new Handler(Looper.MainLooper).Post(() =>
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
    }

}