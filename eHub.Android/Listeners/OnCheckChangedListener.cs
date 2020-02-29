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