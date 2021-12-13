using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

using DialogFragment = AndroidX.AppCompat.App.AppCompatDialogFragment;
using static Android.App.TimePickerDialog;
using Newtonsoft.Json;
using eHub.Android.Models;

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
            var stringArgs = Arguments.GetString($"{nameof(TimePickerArgs)}");
            var args = JsonConvert.DeserializeObject<TimePickerArgs>(stringArgs);
            var dialog = new TimePickerDialog(Context, this, args.Hour, args.Minute, true);

            return dialog;
        }

        void IOnTimeSetListener.OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            OnTimeSelected.Invoke(new TimePickerArgs(hourOfDay, minute));
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            base.OnCancel(dialog);
        }
    }
}