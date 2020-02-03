using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Models;
using eHub.Common.Services;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class BoosterFragment : Fragment
    {
        ImageView _toggleSwitch;
        TextView _messageText;

        [Inject] IPoolService PoolService { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_booster, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Title = "Booster Pump Control";

            var boosterStatus = PinState.OFF;

            _toggleSwitch = view.FindViewById<ImageView>(Resource.Id.booster_toggle_image);
            _messageText = view.FindViewById<TextView>(Resource.Id.booster_message_text);

            var loadingDialog = Dialogs.SimpleAlert(Context, "Loading...", "", "");
            loadingDialog.Show();
            var pinged = await PoolService.Ping();
            loadingDialog.Hide();

            if (pinged)
            {
                boosterStatus = await GetStatus(Pin.BoosterPump);

                _toggleSwitch.Visibility = ViewStates.Visible;

                _toggleSwitch.SetOnClickListener(new OnClickListener(async v =>
                {
                    var curStatus = await GetStatus(Pin.BoosterPump);

                    // Make sure the pool pump is on first!
                    var poolPumpStatus = await GetStatus(Pin.PoolPump);
                    if (curStatus == PinState.OFF && poolPumpStatus == PinState.OFF)
                    {
                        Dialogs.SimpleAlert(Context, "Wait!", "The pool pump needs to be on first!").Show();
                        return;
                    }

                    var toggleResult = await PoolService.Toggle(Pin.BoosterPump);
                    ToggleImage(toggleResult.State);
                }));

                ToggleImage(boosterStatus);
            }
            else
            {
                _messageText.Visibility = ViewStates.Visible;
                _messageText.Text = "Unable to communicate with booster pump";
                _toggleSwitch.Visibility = ViewStates.Gone;
            }
        }
        void ToggleImage(int state)
        {
            var color = new Color(
                    ContextCompat.GetColor(Context, Resource.Color.greenLabel));

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
        }

        async Task<int> GetStatus(int pin)
        {
            var result = await PoolService.GetPinStatus(pin);
            return result.State;
        }
    }
}