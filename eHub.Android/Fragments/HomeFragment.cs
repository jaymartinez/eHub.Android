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
    public class HomeFragment : Fragment
    {
        TextView _poolStatusLbl;
        TextView _spaStatusLbl;
        TextView _boosterStatusLbl;
        TextView _heaterStatusLbl;

        ImageView _poolLightStatusBulb; 
        ImageView _spaLightStatusBulb; 
        ImageView _groundLightStatusBulb;

        Button _refreshButton;

        [Inject] IPoolService PoolService { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var loadingDialog = Dialogs.SimpleAlert(Context, "Loading...", "", "");
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Title = "Home";

            var statusLabel = view.FindViewById<TextView>(Resource.Id.home_status_label);
            var scrollView = view.FindViewById<ScrollView>(Resource.Id.home_scroll_view);

            _poolStatusLbl = view.FindViewById<TextView>(Resource.Id.home_poolstatus_label);
            _boosterStatusLbl = view.FindViewById<TextView>(Resource.Id.home_boosterstatus_label);
            _heaterStatusLbl = view.FindViewById<TextView>(Resource.Id.home_heaterstatus_label);
            _poolLightStatusBulb = view.FindViewById<ImageView>(Resource.Id.home_pool_light_btn);
            _spaLightStatusBulb = view.FindViewById<ImageView>(Resource.Id.home_spa_light_btn);
            _groundLightStatusBulb = view.FindViewById<ImageView>(Resource.Id.home_ground_lights_btn);
            _refreshButton = view.FindViewById<Button>(Resource.Id.home_refresh_btn);

            var poolSection = view.FindViewById<LinearLayout>(Resource.Id.home_pool_section);
            var boosterSection = view.FindViewById<LinearLayout>(Resource.Id.home_booster_section);
            var heaterSection = view.FindViewById<LinearLayout>(Resource.Id.home_heater_section);

            poolSection.SetOnClickListener(new OnClickListener(v =>
            {
                var frag = new PoolControlFragment();
                ((MainActivity)Activity).Push(frag, StringConstants.Tag_PoolControl);
            }));
            boosterSection.SetOnClickListener(new OnClickListener(v =>
            {
                var frag = new BoosterFragment();
                ((MainActivity)Activity).Push(frag, StringConstants.Tag_BoosterPump);
            }));
            heaterSection.SetOnClickListener(new OnClickListener(v =>
            {
                var frag = new HeaterFragment();
                ((MainActivity)Activity).Push(frag, StringConstants.Tag_Heater);
            }));

            _poolLightStatusBulb.SetOnClickListener(new OnClickListener(async v =>
            {
                var result = await PoolService.Toggle(Pin.PoolLight);
                if (result != null)
                {
                    if (result.State == PinState.ON)
                    {
                        _poolLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_on_48);
                    }
                    else
                    {
                        _poolLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_off_48);
                    }
                }
            }));
            _spaLightStatusBulb.SetOnClickListener(new OnClickListener(async v =>
            {
                var result = await PoolService.Toggle(Pin.SpaLight);
                if (result != null)
                {
                    if (result.State == PinState.ON)
                    {
                        _spaLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_on_48);
                    }
                    else
                    {
                        _spaLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_off_48);
                    }
                }
            }));
            _groundLightStatusBulb.SetOnClickListener(new OnClickListener(async v =>
            {
                var result = await PoolService.Toggle(Pin.GroundLights);
                if (result != null)
                {
                    if (result.State == PinState.ON)
                    {
                        _groundLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_on_48);
                    }
                    else
                    {
                        _groundLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_off_48);
                    }
                }
            }));

            _refreshButton.SetOnClickListener(new OnClickListener(async v =>
            {
                loadingDialog.Show();
                await RefreshStatuses();
                loadingDialog.Hide();
            }));

            loadingDialog.Show();

            if (await PoolService.Ping())
            {
                statusLabel.Visibility = ViewStates.Gone;
                scrollView.Visibility = ViewStates.Visible;

                await RefreshStatuses();
            }
            else
            {
                statusLabel.Visibility = ViewStates.Visible;
                scrollView.Visibility = ViewStates.Gone;
            }

            loadingDialog.Hide();
        }

        async Task RefreshStatuses()
        {
            var poolPump = await GetStatus(Pin.PoolPump);
            var booster = await GetStatus(Pin.BoosterPump);
            var poolLight = await GetStatus(Pin.PoolLight);
            var spaLight = await GetStatus(Pin.SpaLight);
            var groundLights = await GetStatus(Pin.GroundLights);
            var heater = await GetStatus(Pin.Heater);

            var offColor = new Color(
                ContextCompat.GetColor(Context, Resource.Color.material_grey_600));
            var onColor = new Color(
                ContextCompat.GetColor(Context, Resource.Color.greenLabel));

            if (poolPump.State == PinState.ON)
            {
                _poolStatusLbl.SetTextColor(onColor);
                _poolStatusLbl.Text = "On";
            }
            else
            {
                _poolStatusLbl.SetTextColor(offColor);
                _poolStatusLbl.Text = "Off";
            }

            if (booster.State == PinState.ON)
            {
                _boosterStatusLbl.SetTextColor(onColor);
                _boosterStatusLbl.Text = "On";
            }
            else
            {
                _boosterStatusLbl.SetTextColor(offColor);
                _boosterStatusLbl.Text = "Off";
            }

            if (heater.State == PinState.ON)
            {
                _heaterStatusLbl.SetTextColor(onColor);
                _heaterStatusLbl.Text = "On";
            }
            else
            {
                _heaterStatusLbl.SetTextColor(offColor);
                _heaterStatusLbl.Text = "Off";
            }

            if (poolLight.State == PinState.ON)
            {
                _poolLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_on_48);
            }
            else
            {
                _poolLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_off_48);
            }

            if (spaLight.State == PinState.ON)
            {
                _spaLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_on_48);
            }
            else
            {
                _spaLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_off_48);
            }

            if (groundLights.State == PinState.ON)
            {
                _groundLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_on_48);
            }
            else
            {
                _groundLightStatusBulb.SetImageResource(Resource.Drawable.icons8_light_off_48);
            }
        }

        async Task<PiPin> GetStatus(int pin)
        {
            return await PoolService.GetPinStatus(pin);
        }
    }
}