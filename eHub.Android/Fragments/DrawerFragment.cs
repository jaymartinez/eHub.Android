
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Common.Services;
using System.Collections.Generic;
using static Android.Views.View;

using Fragment = Android.Support.V4.App.Fragment;

namespace eHub.Android.Fragments
{
    public class DrawerFragment : Fragment
    {
        RecyclerView _recyclerView;
        MainMenuAdapter _adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var act = Activity as AppCompatActivity;
            var actionBar = act.SupportActionBar;

            //HasOptionsMenu = true;
            return inflater.Inflate(Resource.Layout.fragment_drawer, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            var items = new List<MenuItem>
            {
                new MenuItem("Pool", Resource.Drawable.ic_pool_blue_dark_48dp, MenuType.Pool),
                new MenuItem("Spa", Resource.Drawable.ic_hot_tub_blue_dark_48dp, MenuType.Spa)
            };

            _adapter = new MainMenuAdapter(items);

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.main_menu_recycler_view);
            _recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            _recyclerView.AddItemDecoration(new DividerItemDecoration(Context, LinearLayoutManager.Vertical));
            _recyclerView.SetAdapter(_adapter);
        }

        /*
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.main_menu, menu);
        }
        */
    }

    public class MenuItem
    {
        public string Label { get; }
        public int ImageResource { get; }
        public MenuType MenuType { get; }

        public MenuItem(string label, int imageResource, MenuType menuType)
        {
            Label = label;
            ImageResource = imageResource;
            MenuType = menuType;
        }
    }

    public enum MenuType
    {
        Pool,
        Spa
    }
}