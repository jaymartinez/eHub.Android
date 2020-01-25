using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Models;
using eHub.Common.Services;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class PoolControlFragment : Fragment
    {
        ImageView _toggleSwitch;

        [Inject] public IPoolService PoolService { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_pool_control, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var curStatus = PinState.OFF;
            var isConnected = true;

            var ping = await PoolService.Ping();
            if (ping.Count() > 0)
            {
                isConnected = false;
            }

            if (isConnected)
            {
                curStatus = await GetPoolStatus();
            }

            _toggleSwitch = view.FindViewById<ImageView>(Resource.Id.pool_toggle_image);
            _toggleSwitch.SetOnClickListener(new OnClickListener(v =>
            {
                Dialogs.Confirm(Context, "Confirm", "Are you sure?", "Yes", async (confirm) =>
                {
                    if (!confirm)
                    {
                        ToggleImage(curStatus);
                    }
                    else
                    {
                        var toggleResult = await PoolService.Toggle(EquipmentType.SpaLight);
                        ToggleImage(toggleResult.State);
                    }
                }, "No").Show();

            }));

            ToggleImage(curStatus);
        }

        void ToggleImage(PinState state)
        {
            switch (state)
            {
                case PinState.ON:
                    _toggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_on_80);
                    break;
                case PinState.OFF:
                    _toggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_off_80);
                    break;
            }
            _toggleSwitch.RequestLayout();
        }

        async Task<PinState> GetPoolStatus()
        {
            var result = await PoolService.GetPinStatus(EquipmentType.SpaLight);
            if (result == null)
            {

            }

            return result?.State ?? PinState.OFF;
        }
    }
}