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
    public class PoolFragment : Fragment
    {
        Button _saveButton, _editBtnStart, _editBtnStop;

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
            _saveButton = view.FindViewById<Button>(Resource.Id.pool_save_button);
            _editBtnStart = view.FindViewById<Button>(Resource.Id.pool_starttime_button);
            _editBtnStop = view.FindViewById<Button>(Resource.Id.pool_endtime_button);
        }
    }
}