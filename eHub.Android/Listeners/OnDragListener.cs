using System;
using Android.Views;

namespace eHub.Android.Listeners
{
    class OnDragListener : Java.Lang.Object, View.IOnDragListener
    {
        readonly Action<View, DragEvent> _action;

        public OnDragListener(Action<View, DragEvent> action)
        {
            _action = action;
        }

        public bool OnDrag(View v, DragEvent e)
        {
            _action(v, e);

            return e.Result;
        }
    }
}