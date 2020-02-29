using eHub.Common.Models;
using System;

namespace eHub.Android.Models
{
    public class HomeCellItem
    {
        public PoolSchedule PoolScheduleObj { get; }
        public PoolCellItem PoolItem { get; set; }
        public SpaCellItem SpaItem { get; set; }
        public CellType CellTypeObj { get; }
        public PiPin SingleSwitchItem { get; }

        public Action AboutTapped { get; set; }

        public Action StartButtonTapped { get; set; }
        public Action EndButtonTapped { get; set; }
        public Action EnabledCheckboxTapped { get; set; }

        public HomeCellItem(CellType cellType)
        {
            CellTypeObj = cellType;
        }

        public HomeCellItem(PoolSchedule poolSchedule, CellType cellType)
        {
            PoolScheduleObj = poolSchedule;
            CellTypeObj = cellType;
        }

        public HomeCellItem(PoolCellItem poolItem, CellType cellType)
        {
            PoolItem = poolItem;
            CellTypeObj = cellType;
        }

        public HomeCellItem(SpaCellItem spaItem, CellType cellType)
        {
            SpaItem = spaItem;
            CellTypeObj = cellType;
        }

        public HomeCellItem(PiPin piPin, CellType cellType)
        {
            SingleSwitchItem = piPin;
            CellTypeObj = cellType;
        }
    }

    public class SpaCellItem
    {
        public PiPin SpaPump { get; }
        public PiPin SpaLight { get; }
        public Action SpaLightTapped { get; set; }
        public Action SpaSwitchTapped { get; set; }

        public SpaCellItem(PiPin spaPump, PiPin spaLight)
        {
            SpaPump = spaPump;
            SpaLight = spaLight;
        }
    }

    public class PoolCellItem
    {
        public PiPin PoolPump { get; }
        public PiPin PoolLight { get; }
        public Action PoolLightTapped { get; set; }
        public Action PoolSwitchTapped { get; set; }

        public PoolCellItem(PiPin poolPump, PiPin poolLight)
        {
            PoolPump = poolPump;
            PoolLight = poolLight;
        }
    }

    public enum CellType
    {
        Schedule,
        Pool,
        Spa,
        Booster,
        Heater,
        GroundLights,
        About
    }
}