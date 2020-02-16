using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment
    {
        TextView _poolStatusLbl;
        TextView _spaStatusLbl;
        TextView _boosterStatusLbl;
        TextView _heaterStatusLbl;

        ImageView _poolLightStatusBulb; 
        ImageView _spaLightStatusBulb; 
        ImageView _groundLightStatusBulb; 

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_home, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var act = Activity as AppCompatActivity;
            var ab = act.SupportActionBar;
            ab.Title = "Home";

            base.OnViewCreated(view, savedInstanceState);
        }
    }
}