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