using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using eHub.Android.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class LightModesGridViewFragment : Fragment
    {
        const string LightModes = "LightModes";

        public static LightModesGridViewFragment CreateInstance(List<PoolLightModel> items)
        {
            var args = new Bundle();
            args.PutString(LightModes, JsonConvert.SerializeObject(items));

            return new LightModesGridViewFragment()
            {
                Arguments = args
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_lightmodes_gridview, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var lightItems = JsonConvert.DeserializeObject<List<PoolLightModel>>(Arguments.GetString(LightModes));
            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.lightmodes_recycler);
            var adapter = new LightModesAdapter(lightItems);
            recyclerView.SetLayoutManager(new GridLayoutManager(Context, 4));
            recyclerView.SetAdapter(adapter);
        }
    }
}