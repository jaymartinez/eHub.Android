using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class PoolFragment : Fragment, IDialogInterfaceOnCancelListener 
    {
        Button _saveButton, _editBtnStart, _editBtnStop;
        TextView _startText, _stopText;

        PoolSchedule _ps = new PoolSchedule { StartHour = 8, StartMinute = 30, EndHour = 2, EndMinute = 30 };

        public override void OnCreate(Bundle savedInstanceState)
        {
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

            //Todo get current schedule

            _editBtnStart.SetOnClickListener(new OnClickListener(v =>
            {
                var d = new TimePickerDialog(Context, Resource.Style.AlertDialog_AppCompat, (s, e) =>
                {
                    _ps.StartHour = e.HourOfDay;
                    _ps.StartMinute = e.Minute;
                    _startText.Text = $"{e.HourOfDay} : {e.Minute}";
                }, _ps.StartHour, _ps.StartMinute, true);
                d.SetOnCancelListener(this);
                d.Show();

                //var timeFrag = TimePickerFragment.CreateInstance();
                //timeFrag.OnTimeSelected = (args) =>
                //{
                //    _ps.StartHour = args.Hour;
                //    _ps.StartMinute = args.Minute;
                //};

                //timeFrag.Show(Activity.SupportFragmentManager, "time_picker");
            }));

            _editBtnStop.SetOnClickListener(new OnClickListener(v =>
            {
                var d = new TimePickerDialog(Context, Resource.Style.AlertDialog_AppCompat, (s, e) =>
                {
                    _ps.EndHour = e.HourOfDay;
                    _ps.EndMinute = e.Minute;
                    _stopText.Text = $"{e.HourOfDay} : {e.Minute}";
                }, _ps.StartHour, _ps.StartMinute, true);
                d.SetOnCancelListener(this);
                d.Show();

                /*
                var timeFrag = TimePickerFragment.CreateInstance();
                timeFrag.OnTimeSelected = (args) =>
                {
                    _ps.EndHour = args.Hour;
                    _ps.EndMinute = args.Minute;
                };

                timeFrag.Show(Activity.SupportFragmentManager, "time_picker");
                */
            }));

            _saveButton.SetOnClickListener(new OnClickListener(v =>
            {
                SaveSchedule(_ps);
            }));
        }

        void IDialogInterfaceOnCancelListener.OnCancel(IDialogInterface dialog)
        {
        }

        void SaveSchedule(PoolSchedule ps)
        {
            return;
        }
    }
}