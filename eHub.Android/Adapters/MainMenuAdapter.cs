using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using eHub.Android.Listeners;
using eHub.Android.Models;

namespace eHub.Android
{
    public class MainMenuAdapter : RecyclerView.Adapter
    {
        List<MenuItem> _items;

        public Action<MenuItem> MenuTapped { get; set; }

        public MainMenuAdapter(List<MenuItem> items)
        {
            _items = items;
        }

        public override int ItemCount => _items?.Count ?? 0;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var cell = holder as MenuCell;
            var menuType = _items[position].MenuType;

            cell.TextView.Text = _items[position].Label;
            cell.ImageView.SetImageResource(_items[position].ImageResource);

            cell.ItemView.SetOnClickListener(new OnClickListener(v =>
            {
                switch (menuType)
                {
                    case MenuType.Pool:
                        MenuTapped.Invoke(_items[position]);
                        break;
                    case MenuType.Spa:
                        break;
                }
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