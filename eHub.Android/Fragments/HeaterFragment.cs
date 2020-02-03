using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Models;
using eHub.Common.Services;
using System.Threading.Tasks;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class HeaterFragment : Fragment
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
            return inflater.Inflate(Resource.Layout.fragment_heater, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Title = "Heater Control";

            var heaterStatus = PinState.OFF;

            _toggleSwitch = view.FindViewById<ImageView>(Resource.Id.heater_toggle_image);
            _messageText = view.FindViewById<TextView>(Resource.Id.heater_message_text);

            var loadingDialog = Dialogs.SimpleAlert(Context, "Loading...", "", "");
            loadingDialog.Show();
            var pinged = await PoolService.Ping();
            loadingDialog.Hide();

            if (pinged)
            {
                heaterStatus = await GetStatus(Pin.Heater);

                _toggleSwitch.Visibility = ViewStates.Visible;

                _toggleSwitch.SetOnClickListener(new OnClickListener(async v =>
                {
                    var pumpStatus = await GetStatus(Pin.PoolPump);
                    if (pumpStatus == PinState.OFF)
                    {
                        Dialogs.SimpleAlert(Context, "Wait!", "The pool pump needs to be on first!");
                        return;
                    }

                    var toggleResult = await PoolService.Toggle(Pin.Heater);
                    ToggleImage(toggleResult.State);
                }));

                ToggleImage(heaterStatus);
            }
            else
            {
                _messageText.Visibility = ViewStates.Visible;
                _messageText.Text = "Unable to communicate with pool";
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