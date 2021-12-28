using Android.Widget;
using eHub.Common.Models;
using System;
using System.Threading.Tasks;

namespace eHub.Android.Models
{
    public class PoolLightModel
    {
        public bool IsActive { get; }
        public Func<PoolLightModel, PoolLightServerModel, Task<PoolLightModel>> LightTapped { get; }
        public PoolLightMode Mode { get; set; }
        public int PowerCycles => (int)Mode;
        public LightType LightType { get; }
        public int PinNumber => LightType == LightType.Pool ? Pin.PoolLight : Pin.SpaLight; 

        public PoolLightModel(
            PoolLightMode mode, 
            bool isActive, 
            LightType lightType,
            Func<PoolLightModel, PoolLightServerModel, Task<PoolLightModel>> lightTapped)
        {
            Mode = mode;
            IsActive = isActive;
            LightType = lightType;
            LightTapped = lightTapped;
        }

    }
}