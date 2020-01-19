using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Api;
using eHub.Common.Models;
using eHub.Common.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class PoolFragment : Fragment
    {
        Button _saveButton, _editBtnStart, _editBtnStop;
        TextView _startText, _stopText;
        Switch _toggleSwitch;

        PoolSchedule _ps = new PoolSchedule { StartHour = 8, StartMinute = 30, EndHour = 2, EndMinute = 30 };

        public override void OnCreate(Bundle savedInstanceState)
        {
            //var i = MainActivity.ResolveServiceForFragment<IPoolService, PoolService>(this);
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            

            return inflater.Inflate(Resource.Layout.fragment_pool, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Title = "Pool Pump Schedule";
            _saveButton = view.FindViewById<Button>(Resource.Id.pool_save_button);
            _editBtnStart = view.FindViewById<Button>(Resource.Id.pool_starttime_button);
            _editBtnStop = view.FindViewById<Button>(Resource.Id.pool_endtime_button);
            _startText = view.FindViewById<TextView>(Resource.Id.pool_starttime_text);
            _stopText = view.FindViewById<TextView>(Resource.Id.pool_endtime_text);
            _toggleSwitch = view.FindViewById<Switch>(Resource.Id.pool_onoff_switch);

            WebInterface webInterface = new WebInterface();
            PoolApi poolApi = new PoolApi(webInterface);
            PoolService poolService = new PoolService(poolApi);

            _toggleSwitch.SetOnClickListener(new OnClickListener(v =>
            {
                return;
            }));

            Task.Run(async () =>
            {
                _ps = await poolService.GetSchedule();
                var status = await poolService.GetPinStatus(EquipmentType.PoolPump);

                Activity.RunOnUiThread(() =>
                {
                    _startText.Text = $"{_ps.StartHour} : {_ps.StartMinute}";
                    _stopText.Text = $"{_ps.EndHour} : {_ps.EndMinute}";
                    _toggleSwitch.Selected = status.State == PinState.ON ? true : false;
                });
            });

            // Get current schedule

            _editBtnStart.SetOnClickListener(new OnClickListener(v =>
            {
                var d = new TimePickerDialog(Context, Resource.Style.timepicker_theme, (s, e) =>
                {
                    _ps.StartHour = e.HourOfDay;
                    _ps.StartMinute = e.Minute;
                    _startText.Text = $"{e.HourOfDay} : {e.Minute}";
                }, _ps.StartHour, _ps.StartMinute, true);
                d.SetTitle("Pick a Start Time");
                d.Show();
            }));

            _editBtnStop.SetOnClickListener(new OnClickListener(v =>
            {
                var d = new TimePickerDialog(Context, Resource.Style.timepicker_theme, (s, e) =>
                {
                    _ps.EndHour = e.HourOfDay;
                    _ps.EndMinute = e.Minute;
                    _stopText.Text = $"{e.HourOfDay} : {e.Minute}";
                }, _ps.StartHour, _ps.StartMinute, true);
                d.SetTitle("Pick a Stop Time");
                d.Show();

            }));

            _saveButton.SetOnClickListener(new OnClickListener(v =>
            {
                SaveSchedule(_ps);
            }));
        }

        void SaveSchedule(PoolSchedule ps)
        {
            Task.Run(async () =>
            {
                await SaveScheduleAsync(ps);
            });
        }

        async Task<PoolSchedule> GetSchedule(IPoolService poolService)
        {
            return await poolService.GetSchedule();
        }

        async Task SaveScheduleAsync(PoolSchedule ps)
        {
            var startDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ps.StartHour, ps.StartMinute, 0);
            var endDateTime = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ps.EndHour, ps.EndMinute, 0);

            //TODO - need to get this from container
            WebInterface webInterface = new WebInterface();
            PoolApi poolApi = new PoolApi(webInterface);
            PoolService poolService = new PoolService(poolApi);

            var result = await poolService.SetSchedule(startDateTime, endDateTime);

            Activity.RunOnUiThread(() =>
            {
                Dialogs.SimpleAlert(Context, "Success!", "Schedule saved successfully.").Show();
            });
        }
    }
}