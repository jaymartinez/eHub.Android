using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Android.Models;
using eHub.Common.Models;
using eHub.Common.Services;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class PoolControlFragment : Fragment
    {
        ImageView _toggleSwitch, _lightToggleSwitch;
        TextView _messageText, _lightMessageText, _poolLabel, _lightLabel;

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
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Title = "Pool Control";

            var curPumpStatus = PinState.OFF;
            var curLightStatus = PinState.OFF;

            _toggleSwitch = view.FindViewById<ImageView>(Resource.Id.pool_toggle_image);
            _lightToggleSwitch = view.FindViewById<ImageView>(Resource.Id.pool_lights_toggle_image);
            _messageText = view.FindViewById<TextView>(Resource.Id.pool_message_text);
            _poolLabel = view.FindViewById<TextView>(Resource.Id.pool_pump_label);
            _lightMessageText = view.FindViewById<TextView>(Resource.Id.pool_lights_message_text);
            _lightLabel = view.FindViewById<TextView>(Resource.Id.pool_light_label);

            var loadingDialog = Dialogs.SimpleAlert(Context, "Loading...", "", "");
            loadingDialog.Show();
            var pinged = await PoolService.Ping();
            loadingDialog.Hide();

            if (pinged)
            {
                curPumpStatus = await GetStatus(Pin.PoolPump);
                curLightStatus = await GetStatus(Pin.PoolLight);

                _toggleSwitch.Visibility = ViewStates.Visible;
                _lightToggleSwitch.Visibility = ViewStates.Visible;
                _poolLabel.Visibility = ViewStates.Visible;
                _lightLabel.Visibility = ViewStates.Visible;

                _toggleSwitch.SetOnClickListener(new OnClickListener(v =>
                {
                    Dialogs.Confirm(Context, "Confirm", "Are you sure?", "Yes", async (confirm) =>
                    {
                        if (!confirm)
                        {
                            ToggleImage(curPumpStatus, ToggleImageType.Pump);
                        }
                        else
                        {
                            var toggleResult = await PoolService.Toggle(Pin.PoolPump);
                            ToggleImage(toggleResult.State, ToggleImageType.Pump);
                        }
                    }, "No").Show();

                }));

                ToggleImage(curPumpStatus, ToggleImageType.Pump);
                ToggleImage(curLightStatus, ToggleImageType.Light);
            }
            else
            {
                _messageText.Visibility = ViewStates.Visible;
                _messageText.Text = "Unable to communicate with pool";
                _toggleSwitch.Visibility = ViewStates.Gone;
                _lightMessageText.Visibility = ViewStates.Gone;
                _lightToggleSwitch.Visibility = ViewStates.Gone;
            }
        }

        void ToggleImage(int state, ToggleImageType type)
        {
            var color = new Color(
                    ContextCompat.GetColor(Context, Resource.Color.greenLabel));

            switch (type)
            {
                case ToggleImageType.Pump:
                    _messageText.Visibility = ViewStates.Visible;
                    if (state == PinState.ON)
                    {
                        _toggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_on_80);
                        _messageText.Text = "On";
                        _messageText.SetTextColor(color);
                    }
                    else
                    {
                        color = new Color(
                            ContextCompat.GetColor(Context, Resource.Color.redLabel));
                        _toggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_off_80);
                        _messageText.SetTextColor(color);
                        _messageText.Text = "Off";
                    }
                    _toggleSwitch.RequestLayout();
                    break;
                case ToggleImageType.Light:
                    _lightMessageText.Visibility = ViewStates.Visible;
                    if (state == PinState.ON)
                    {
                        _lightToggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_on_80);
                        _lightMessageText.Text = "On";
                        _lightMessageText.SetTextColor(color);
                    }
                    else
                    {
                        color = new Color(
                            ContextCompat.GetColor(Context, Resource.Color.redLabel));
                        _lightToggleSwitch.SetImageResource(Resource.Drawable.icons8_switch_off_80);
                        _lightMessageText.SetTextColor(color);
                        _lightMessageText.Text = "Off";
                    }
                    break;
            }

            switch (state)
            {
                case PinState.ON:
                    break;
                case PinState.OFF:
                    break;
            }
        }

        async Task<int> GetStatus(int pin)
        {
            var result = await PoolService.GetPinStatus(pin);
            return result.State;
        }
    }
}