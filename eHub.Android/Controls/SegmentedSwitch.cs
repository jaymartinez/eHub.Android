using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using eHub.Android.Listeners;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Android.Views.ViewGroup;

namespace eHub.Android.Controls
{
    internal class SegmentedSwitch : ViewGroup
    {
        public Button OnButton { get; } 
        public Button OffButton { get; }
        public int SelectedIndex { get; private set; }

        public SegmentedSwitch(Context context, IAttributeSet attributeSet)
            : base(context, attributeSet)
        {
            var parent = new LinearLayout(context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal
            };
           
            OnButton = new SegmentedButton(context, attributeSet)
            {
                Id = new Random(1).Next(),
                Text = "On",
                LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)
                {
                    MarginStart = 8
                }
            };
            OffButton = new SegmentedButton(context, attributeSet)
            {
                Id = new Random(1).Next(),
                Text = "Off",
                LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)
            };

            // Initialize to off state colors
            var offTextColor = ContextCompat.GetColor(context, Resource.Color.material_grey_300);
            OnButton.SetBackgroundResource(Resource.Color.blue_gray_400);
            OnButton.SetTextColor(new Color(offTextColor));

            OffButton.SetBackgroundResource(Resource.Color.redLabel);
            OffButton.SetTextColor(new Color(offTextColor));

            parent.AddView(OnButton);
            parent.AddView(OffButton);
            AddView(parent);
        }

        void ToggleButton(
           Button onButton,
           Button offButton,
           int state,
           Context context)
        {
            var onTextColor = ContextCompat.GetColor(
                context, Resource.Color.material_blue_grey_800);

            var offTextColor = ContextCompat.GetColor(
                context, Resource.Color.material_grey_300);

            if (state == 1)
            {
                onButton.SetBackgroundResource(Resource.Color.greenLabel);
                onButton.SetTextColor(new Color(onTextColor));

                offButton.SetBackgroundResource(Resource.Color.blue_gray_400);
                offButton.SetTextColor(new Color(offTextColor));
            }
            else
            {
                offButton.SetBackgroundResource(Resource.Color.redLabel);
                offButton.SetTextColor(new Color(offTextColor));

                onButton.SetBackgroundResource(Resource.Color.blue_gray_400);
                onButton.SetTextColor(new Color(offTextColor));
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            Console.WriteLine($"SEGMENTED SWITCH OnLayout(changed:{changed}, l:{l}, t:{t}, r:{r}, b:{b}");
        }
    }

    internal class SegmentedButton : Button
    {
        public bool IsSelected { get; set; }

        public SegmentedButton(Context context, IAttributeSet attrs) : 
            base(context, attrs)
        {
        }
    }
}