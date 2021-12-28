using Android.Widget;
using eHub.Common.Models;
using System;
using System.Threading.Tasks;

namespace eHub.Android.Models
{
    public class SpaCellItem
    {
        public PiPin SpaPin1 { get; }
        public PiPin SpaPin2 { get; }
        public PiPin SpaLight { get; }
        public Action<Switch> LightOnOffSwitchTapped { get; set; }
        public PoolLightMode SelectedLightMode { get; set; }

        public Func<PoolLightModel, TextView, Task<bool>> LightModeButtonTapped { get; set; }

        public SpaCellItem(PiPin spaPin1, PiPin spaPin2, PiPin spaLight)
        {
            SpaPin1 = spaPin1;
            SpaPin2 = spaPin2;
            SpaLight = spaLight;
        }
    }
}