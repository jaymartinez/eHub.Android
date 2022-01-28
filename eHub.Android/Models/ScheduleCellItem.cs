using Android.Widget;
using eHub.Common.Models;
using System;
using Switch = AndroidX.AppCompat.Widget.SwitchCompat;

namespace eHub.Android.Models
{
    public class ScheduleCellItem
    {
        public EquipmentSchedule Schedule { get; }
        public Action<Button> StartTapped { get; set; }
        public Action<Button> EndTapped { get; set; }
        public Action<CheckBox> IncludeBoosterTapped { get; set; }
        public Action<Switch> OnOffSwitchTapped { get; set; }

        public ScheduleCellItem(EquipmentSchedule schedule)
        {
            Schedule = schedule;
        }
    }
}