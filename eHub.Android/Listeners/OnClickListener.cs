using System;
using Android.Views;

namespace eHub.Android.Listeners
{
    public class OnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        readonly Action<View> _action;

        public OnClickListener(Action<View> action)
        {
            _action = action;
        }

        public void OnClick(View v)
        {
            _action(v);
        }
    }
}