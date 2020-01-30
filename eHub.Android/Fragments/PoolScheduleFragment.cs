using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Models;
using eHub.Common.Services;
using System;
using System.Threading.Tasks;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class PoolScheduleFragment : Fragment
    {
        Button _saveButton, _editBtnStart, _editBtnStop;
        TextView _startText, _stopText, _errorText;
        CheckBox _enabledCb;

        PoolSchedule _ps = new PoolSchedule();

        [Inject]
        public IPoolService PoolService { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_pool_schedule, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Title = "Pool Pump Schedule";
            _saveButton = view.FindViewById<Button>(Resource.Id.pool_save_button);
            _editBtnStart = view.FindViewById<Button>(Resource.Id.pool_starttime_button);
            _editBtnStop = view.FindViewById<Button>(Resource.Id.pool_endtime_button);
            _startText = view.FindViewById<TextView>(Resource.Id.pool_starttime_text);
            _stopText = view.FindViewById<TextView>(Resource.Id.pool_endtime_text);
            _errorText = view.FindViewById<TextView>(Resource.Id.pool_schedule_error_text);
            _enabledCb = view.FindViewById<CheckBox>(Resource.Id.pool_enable_cb);

            var loadingDialog = Dialogs.SimpleAlert(Context, "Loading...", "");

            loadingDialog.Show();
            var connected = await PoolService.Ping();

            if (connected)
            {
                var curSchedule = await PoolService.GetSchedule();
                loadingDialog.Hide();

                if (curSchedule != null)
                {
                    _ps = curSchedule;
                    _startText.Text = $"{curSchedule.StartHour} : {curSchedule.StartMinute}";
                    _stopText.Text = $"{curSchedule.EndHour} : {curSchedule.EndMinute}";
                    _enabledCb.Checked = curSchedule.IsActive;
                }

                _editBtnStart.SetOnClickListener(new OnClickListener(v =>
                {
                    var picker = TimePickerFragment.CreateInstance(_ps.StartHour, _ps.StartMinute);
                    picker.OnTimeSelected = (args) =>
                    {
                        _ps.StartHour = args.Hour;
                        _ps.StartMinute = args.Minute;
                        _startText.Text = GetTimeDisplay(args.Hour, args.Minute);
                    };

                    picker.Show(ChildFragmentManager, "starttime_picker");
                }));

                _editBtnStop.SetOnClickListener(new OnClickListener(v =>
                {
                    var picker = TimePickerFragment.CreateInstance(_ps.StartHour, _ps.StartMinute);

                    picker.OnTimeSelected = (args) =>
                    {
                        _ps.EndHour = args.Hour;
                        _ps.EndMinute = args.Minute;
                        _stopText.Text = GetTimeDisplay(args.Hour, args.Minute);
                    };

                    picker.Show(ChildFragmentManager, "endtime_picker");
                }));

                _saveButton.SetOnClickListener(new OnClickListener(v =>
                {
                    SaveSchedule(_ps);
                }));
            }
            else
            {
                loadingDialog.Hide();
                _editBtnStart.Enabled = false;
                _editBtnStop.Enabled = false;
                _saveButton.Enabled = false;
                _errorText.Visibility = ViewStates.Visible;
            }
        }

        string GetTimeDisplay(int hour, int minute)
        {
            var hourDisplay = hour.ToString();
            var minuteDisplay = minute.ToString();

            if (minute == 0)
            {
                minuteDisplay = "00";
            }
            else if (minuteDisplay.Length == 1)
            {
                minuteDisplay = "0" + minuteDisplay;
            }

            return $"{hourDisplay} : {minuteDisplay}";
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

            var result = await PoolService.SetSchedule(startDateTime, endDateTime, _enabledCb.Checked);

            if (result != null)
            {
                Activity.RunOnUiThread(() =>
                {
                    Dialogs.SimpleAlert(Context, "Success!", "Schedule saved successfully.").Show();
                });
            }
        }
    }
}