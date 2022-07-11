using Android.Widget;
using eHub.Common.Models;
using System;

namespace eHub.Android.Models
{
    public class ScheduleCellItem
    {
        public EquipmentSchedule PoolSchedule { get; }
        public EquipmentSchedule BoosterSchedule { get; }
        public Action<Button, EquipmentSchedule> StartTapped { get; set; }
        public Action<Button, EquipmentSchedule> EndTapped { get; set; }
        public Action<Button, Button, EquipmentSchedule> OnButtonTapped { get; set; }
        public Action<Button, Button, EquipmentSchedule> OffButtonTapped { get; set; }

        public ScheduleCellItem(EquipmentSchedule poolSchedule, EquipmentSchedule boosterSchedule)
        {
            PoolSchedule = poolSchedule;
            BoosterSchedule = boosterSchedule;
        }
    }
}