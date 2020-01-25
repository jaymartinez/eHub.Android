using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using DialogFragment = Android.Support.V4.App.DialogFragment;
using static Android.App.TimePickerDialog;
using eHub.Common.Models;
using Newtonsoft.Json;

namespace eHub.Android.Fragments
{
    public class TimePickerFragment : 
        DialogFragment, 
        IDialogInterfaceOnCancelListener, 
        IOnTimeSetListener,
        IDialogInterfaceOnDismissListener
    {
        public Action<TimePickerArgs> OnTimeSelected { get; set; }

        public static TimePickerFragment CreateInstance(int hour, int minute)
        {
            var args = new Bundle();
            var pickerArgs = new TimePickerArgs(hour, minute);

            args.PutString($"{nameof(TimePickerArgs)}", 
                JsonConvert.SerializeObject(pickerArgs));

            return new TimePickerFragment()
            {
                Arguments = args
            };
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var stringArgs = savedInstanceState.GetString($"{nameof(TimePickerArgs)}");
            var args = JsonConvert.DeserializeObject<TimePickerArgs>(stringArgs);
            return new TimePickerDialog(Context, this, args.Hour, args.Minute, true);
        }

        void IOnTimeSetListener.OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {

        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
        }


        /*
        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            var args = new TimePickerArgs(hourOfDay, minute);
            OnTimeSelected.Invoke(args);
        }
        */
    }

    public class TimePickerArgs
    {
        public int Hour { get; }
        
        public int Minute { get; }

        public TimePickerArgs(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }
    }
}