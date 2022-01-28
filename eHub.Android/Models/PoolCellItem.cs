using Android.Widget;
using eHub.Common.Models;
using System;
using System.Threading.Tasks;
using Switch = AndroidX.AppCompat.Widget.SwitchCompat;

namespace eHub.Android.Models
{
    public class PoolCellItem
    {
        public PiPin PoolPin1 { get; }
        public PiPin PoolPin2 { get; }
        public PiPin PoolLight { get; }
        public Action<Switch> LightOnOffSwitchTapped { get; set; }
        public LightModeType SelectedLightMode { get; set; }
        public WaterTemp WaterTemp { get; set; }

        public Func<LightModel, TextView, Task<bool>> LightModeButtonTapped { get; set; }

        public PoolCellItem(PiPin poolPin1, PiPin poolPin2, PiPin poolLight)
        {
            PoolPin1 = poolPin1;
            PoolPin2 = poolPin2;
            PoolLight = poolLight;
        }
    }
}