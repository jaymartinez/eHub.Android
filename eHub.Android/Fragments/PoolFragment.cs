using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Services;
using System;
using static Android.Widget.TimePicker;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class PoolFragment : Fragment, IOnTimeChangedListener
    {
        TimePicker _startPicker;
        TimePicker _endPicker;
        Button _saveButton;

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
            //_startPicker = view.FindViewById<TimePicker>(Resource.Id.picker_start_time);
            //_endPicker = view.FindViewById<TimePicker>(Resource.Id.picker_end_time);
            //_saveButton = view.FindViewById<Button>(Resource.Id.pool_save_button);

            _startPicker.SetOnTimeChangedListener(this);
            _endPicker.SetOnTimeChangedListener(this);

            _saveButton.SetOnClickListener(new OnClickListener(v =>
            {
                var startHour = _startPicker.Hour;
                var startMinute = _startPicker.Minute;

                var endHour = _endPicker.Hour;
                var endMinute = _endPicker.Minute;
            }));

        }

        void IOnTimeChangedListener.OnTimeChanged(TimePicker view, int hourOfDay, int minute)
        {
            /*
            if (view.Id == Resource.Id.picker_start_time)
            {
                Console.WriteLine($">>>>> Start Time: {hourOfDay} : {minute}");
            }
            else if (view.Id == Resource.Id.picker_end_time)
            {
                Console.WriteLine($">>>>> End Time: {hourOfDay} : {minute}");
            }
            */
        }
    }
}