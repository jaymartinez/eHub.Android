using Android.Widget;
using eHub.Common.Models;
using System;
using System.Threading.Tasks;

namespace eHub.Android.Models
{
    public class LightModel
    {
        public bool IsActive { get; }
        public LightModeType Mode { get; set; }
        public int PowerCycles => (int)Mode;
        public LightType LightType { get; }
        public int PinNumber => LightType == LightType.Pool ? Pin.PoolLight : Pin.SpaLight;
        public PinType PinType => LightType == LightType.Pool ? PinType.PoolLight : PinType.SpaLight;

        public LightModel(LightModeType mode, LightType lightType)
        {
            Mode = mode;
            LightType = lightType;
        }

    }
}