using eHub.Common.Models;
using System;

namespace eHub.Android.Models
{
    public class HomeCellItem
    {
        public ScheduleCellItem ScheduleCellItem { get; set; }
        public PoolCellItem PoolItem { get; set; }
        public SpaCellItem SpaItem { get; set; }
        public DeviceCellItem DevicesItem { get; set; }
        public BoosterCellItem BoosterItem { get; set; }
        public LightModesCellItem LightModesItem { get; set; }
        public CellType CellTypeObj { get; }
        public PiPin SingleSwitchItem { get; }

        public Action AboutTapped { get; set; }

        public HomeCellItem(CellType cellType)
        {
            CellTypeObj = cellType;
        }

        public HomeCellItem(DeviceCellItem deviceCellItem, CellType cellType)
        {
            DevicesItem = deviceCellItem;
            CellTypeObj = cellType;
        }

        public HomeCellItem(BoosterCellItem boosterCellItem, CellType cellType)
        {
            SingleSwitchItem = boosterCellItem.BoosterPin;
            BoosterItem = boosterCellItem;
            CellTypeObj = cellType;
        }

        public HomeCellItem(LightModesCellItem item, CellType cellType)
        {
            LightModesItem = item;
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
}