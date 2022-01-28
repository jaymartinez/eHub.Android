using Android.Widget;
using eHub.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Switch = AndroidX.AppCompat.Widget.SwitchCompat;

namespace eHub.Android.Models
{
    public class LightModesCellItem
    {
        public EquipmentSchedule PoolLightSchedule { get; }
        public EquipmentSchedule SpaLightSchedule { get; }
        public List<LightModel> PoolLightModelList { get; set; }
        public List<LightModel> SpaLightModelList { get; set; }
        public Func<LightModeType, Task<LightModel>> PoolLightModeButtonTapped { get; set; }
        public Func<LightModeType, Task<LightModel>> SpaLightModeButtonTapped { get; set; }
        public LightModeType SelectedPoolLightMode { get; set; }
        public LightModeType SelectedSpaLightMode { get; set; }

        public Action LightLegendTapped { get; set; }
        public Action<Button> PoolLightScheduleStartTapped { get; set; }
        public Action<Button> PoolLightScheduleEndTapped { get; set; }
        public Action<Button> SpaLightScheduleStartTapped { get; set; }
        public Action<Button> SpaLightScheduleEndTapped { get; set; }
        public Action<Switch> PoolLightScheduleOnOffSwitchTapped { get; set; }
        public Action<Switch> SpaLightScheduleOnOffSwitchTapped { get; set; }

        public LightModesCellItem(
            EquipmentSchedule poolLightSchedule, 
            EquipmentSchedule spaLightSchedule)
        {
            PoolLightSchedule = poolLightSchedule;
            SpaLightSchedule = spaLightSchedule;
        }
    }
}