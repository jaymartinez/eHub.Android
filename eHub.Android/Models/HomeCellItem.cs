using Android.Widget;
using eHub.Common.Models;
using System;
using System.Threading.Tasks;
using Switch = Android.Support.V7.Widget.SwitchCompat; 

namespace eHub.Android.Models
{
    public class HomeCellItem
    {
        public ScheduleCellItem ScheduleCellItem { get; set; }
        public PoolCellItem PoolItem { get; set; }
        public SpaCellItem SpaItem { get; set; }
        public CellType CellTypeObj { get; }
        public PiPin SingleSwitchItem { get; }

        public Action AboutTapped { get; set; }

        public HomeCellItem(CellType cellType)
        {
            CellTypeObj = cellType;
        }

        public HomeCellItem(ScheduleCellItem scheduleItem, CellType cellType)
        {
            ScheduleCellItem = scheduleItem;
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

    public class ScheduleCellItem
    {
        public PoolSchedule Schedule { get; }
        public Action<Button> StartTapped { get; set; }
        public Action<Button> EndTapped { get; set; }
        public Action<CheckBox> IncludeBoosterTapped { get; set; }
        public Action<Switch> OnOffSwitchTapped { get; set; }

        public ScheduleCellItem(PoolSchedule schedule)
        {
            Schedule = schedule;
        }
    }

    public class SpaCellItem
    {
        public PiPin SpaPump { get; }
        public PiPin SpaLight { get; }
        public Action<Switch> LightOnOffSwitchTapped { get; set; }
        public PoolLightMode SelectedLightMode { get; set; }

        public Func<PoolLightModel, TextView, Task<bool>> LightModeButtonTapped { get; set; }

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
        public Action<Switch> LightOnOffSwitchTapped { get; set; }
        public PoolLightMode SelectedLightMode { get; set; }

        public Func<PoolLightModel, TextView, Task<bool>> LightModeButtonTapped { get; set; }

        public PoolCellItem(PiPin poolPump, PiPin poolLight)
        {
            PoolPump = poolPump;
            PoolLight = poolLight;
        }
    }

    public class PoolLightModel
    {
        public PoolLightModel(PoolLightMode mode)
        {
            Mode = mode;
        }

        public PoolLightMode Mode { get; }
        public int PowerCycles => (int)Mode;
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