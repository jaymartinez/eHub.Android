using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Fragments;
using eHub.Android.Listeners;

namespace eHub.Android
{
    public class MainMenuAdapter : RecyclerView.Adapter
    {
        List<MenuItem> _items;

        public MainMenuAdapter(List<MenuItem> items)
        {
            _items = items;
        }

        public override int ItemCount => _items?.Count ?? 0;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var cell = holder as MenuCell;

            cell.TextView.Text = _items[position].Label;
            cell.ImageView.SetImageResource(_items[position].ImageResource);

            cell.ItemView.SetOnClickListener(new OnClickListener(v =>
            {
                //Activity.RunOnUiThread(() =>
                //{
                //    var frag = new PoolFragment();

                //    Activity
                //    .SupportFragmentManager
                //    .BeginTransaction()
                //    .Replace(Resource.Id.main_container, frag, "Pool")
                //    .AddToBackStack("Pool")
                //    .Commit();
                //});

            }));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var cell = inflater.Inflate(Resource.Layout.fragment_main_menu_cell, parent, false);
            return new MenuCell(cell);
        }

        class MenuCell : RecyclerView.ViewHolder
        {
            public ImageView ImageView { get; }
            public TextView TextView { get; }

            public MenuCell(View view)
                : base(view)
            {
                ImageView = view.FindViewById<ImageView>(Resource.Id.menu_imageview);
                TextView = view.FindViewById<TextView>(Resource.Id.menu_label);
            }
        }
    }

}