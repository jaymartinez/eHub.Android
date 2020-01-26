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
        TextView _messageText;

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

            _toggleSwitch = view.FindViewById<ImageView>(Resource.Id.pool_toggle_image);
            _messageText = view.FindViewById<TextView>(Resource.Id.pool_message_text);

            var loadingDialog = Dialogs.SimpleAlert(Context, "Loading...", "", "");
            loadingDialog.Show();
            var pinged = await PoolService.Ping();
            loadingDialog.Hide();

            if (pinged)
            {
                curStatus = await GetPoolStatus();
                _messageText.Visibility = ViewStates.Gone;
            }
            else
            {
                _messageText.Visibility = ViewStates.Visible;
                _messageText.Text = "Unable to communicate with pool";
                _toggleSwitch.Visibility = ViewStates.Gone;
            }

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
                        var toggleResult = await PoolService.Toggle(Pin.SpaLight);
                        ToggleImage(toggleResult.State);
                    }
                }, "No").Show();

            }));

            ToggleImage(curStatus);
        }

        void ToggleImage(int state)
        {
            switch (state)
            {
                case PinState.ON:
                    _toggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_on_80);
                    _messageText.Text = "On";
                    //_messageText.SetTextColor()
                    break;
                case PinState.OFF:
                    _toggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_off_80);
                    //_messageText.SetTextColor()
                    break;
            }
            _toggleSwitch.RequestLayout();
        }

        async Task<int> GetPoolStatus()
        {
            var result = await PoolService.GetPinStatus(Pin.SpaLight);
            return result.State;
        }
    }
}