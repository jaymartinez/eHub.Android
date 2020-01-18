using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class PoolFragment : Fragment
    {
        Button _saveButton, _editBtnStart, _editBtnStop;
        TextView _startText, _stopText;


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

            _editBtnStart.SetOnClickListener(new OnClickListener(v =>
            {
                //todo
            }));

            _editBtnStop.SetOnClickListener(new OnClickListener(v =>
            {
                //todo
            }));
        }
    }
}