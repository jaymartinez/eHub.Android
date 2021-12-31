using System;
using Android.Widget;

namespace eHub.Android.Listeners
{
    class OnCheckChangedListener : Java.Lang.Object, CompoundButton.IOnCheckedChangeListener
    {
        readonly Action<CompoundButton, bool> _action;

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            _action(buttonView, isChecked);
        }

        public OnCheckChangedListener(Action<CompoundButton, bool> action)
        {
            _action = action;
        }
    }
}