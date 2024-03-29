﻿using Android.Widget;
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
        public List<PoolLightModel> PoolLightModelList { get; set; }
        public List<PoolLightModel> SpaLightModelList { get; set; }
        public Func<PoolLightMode, Task<PoolLightModel>> PoolLightModeButtonTapped { get; set; }
        public Func<PoolLightMode, Task<PoolLightModel>> SpaLightModeButtonTapped { get; set; }
        public PoolLightMode SelectedPoolLightMode { get; set; }
        public PoolLightMode SelectedSpaLightMode { get; set; }

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