using eHub.Common.Models;
using System.Collections.Generic;

namespace eHub.Android.Models
{
    public class DeviceCellItem
    {
        public PoolSpaModel PoolModel { get; }
        public PoolSpaModel SpaModel { get; }
        public BoosterPumpModel BoosterModel { get; }
        public HeaterModel HeaterModel { get; }

        public DeviceCellItem(PoolSpaModel poolModel,
            PoolSpaModel spaModel,
            BoosterPumpModel boosterModel, 
            HeaterModel heaterModel)
        {
            PoolModel = poolModel;
            SpaModel = spaModel;
            BoosterModel = boosterModel;
            HeaterModel = heaterModel;
        }
    }
}