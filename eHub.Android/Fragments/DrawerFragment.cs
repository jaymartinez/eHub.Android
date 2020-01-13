
using Android.OS;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Services;
using static Android.Views.View;

using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class DrawerFragment : Fragment
    {
        Button _poolButton;
        Button _spaButton;

        readonly IPoolService _poolService;

        public DrawerFragment() { }
        public DrawerFragment(IPoolService poolService)
        {
            _poolService = poolService;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //HasOptionsMenu = true;
            return inflater.Inflate(Resource.Layout.fragment_drawer, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            _poolButton = view.FindViewById<Button>(Resource.Id.pool_button);
            _spaButton = view.FindViewById<Button>(Resource.Id.spa_button);

            _poolButton.SetOnClickListener(new OnClickListener(v =>
            {
            }));
        }

        /*
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.main_menu, menu);
        }
        */
    }
}