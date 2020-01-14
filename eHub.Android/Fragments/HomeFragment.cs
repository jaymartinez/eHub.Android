using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class HomeFragment : Fragment
    {
        Button _poolButton;
        Button _spaButton;

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
            base.OnViewCreated(view, savedInstanceState);

            _poolButton = view.FindViewById<Button>(Resource.Id.pool_button);
            _spaButton = view.FindViewById<Button>(Resource.Id.spa_button);

            _poolButton.SetOnClickListener(new OnClickListener(v =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var frag = new PoolFragment();

                    Activity
                    .SupportFragmentManager
                    .BeginTransaction()
                    .Replace(Resource.Id.main_container, frag, "Pool")
                    .AddToBackStack("Pool")
                    .Commit();
                });
                    
            }));

        }
    }
}