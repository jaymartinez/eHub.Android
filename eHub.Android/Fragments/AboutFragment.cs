
using Android.OS;
using Android.Views;
using Android.Widget;
using eHub.Android.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class AboutFragment : Fragment
    {
        [Inject] AppVersion AppVersion { get; set; }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            EhubInjector.InjectProperties(this);

            return inflater.Inflate(Resource.Layout.fragment_about, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var versionNameLabel = view.FindViewById<TextView>(Resource.Id.about_version_name);
            var versionNumberLabel = view.FindViewById<TextView>(Resource.Id.about_version_number);

            versionNameLabel.Text = AppVersion.VersionName;
            versionNumberLabel.Text = AppVersion.VersionNumber.ToString();
        }
    }
}