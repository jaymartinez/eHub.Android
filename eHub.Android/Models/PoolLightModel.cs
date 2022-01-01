using Android.Widget;
using eHub.Common.Models;
using System;
using System.Threading.Tasks;

namespace eHub.Android.Models
{
    public class PoolLightModel
    {
        public bool IsActive { get; }
        public PoolLightMode Mode { get; set; }
        public int PowerCycles => (int)Mode;
        public LightType LightType { get; }
        public int PinNumber => LightType == LightType.Pool ? Pin.PoolLight : Pin.SpaLight; 

        public PoolLightModel( PoolLightMode mode, LightType lightType)
        {
            Mode = mode;
            LightType = lightType;
        }

    }
}