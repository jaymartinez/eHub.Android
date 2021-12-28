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

        public List<PoolLightModel> Items { get; set; } = new List<PoolLightModel>();

        public WeakReference ActivityRef { get; set; }

        public LightModesAdapter(List<PoolLightModel> items)
        {
            _mainUiHandler = new Handler(Looper.MainLooper);
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
            var v = inflater.Inflate(Resource.Layout.item_light_gridview_cell, parent, false);
            return new LightViewHolder(v);

            /*

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
            */
        }
    }

    class LightViewHolder : RecyclerView.ViewHolder
    {
        public LightViewHolder(View itemView) : base(itemView)
        {
        }
    }
}