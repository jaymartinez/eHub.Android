using Android.Widget;
using eHub.Common.Models;
using System;
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

        public Action<PoolLightModel> LightModeButtonTapped { get; set; }

        public PoolCellItem(PiPin poolPump, PiPin poolLight)
        {
            PoolPump = poolPump;
            PoolLight = poolLight;
        }
    }

    /// <summary>
    /// Used for any of the light theme button taps, passes the number of times to toggle the light switch.
    /// <code><list type="bullet">
    /// <item><see cref="Sam"/>: 1</item>
    /// <item><see cref="Party"/>: 2</item>
    /// <item><see cref="Romance"/>: 3</item>
    /// <item><see cref="Caribbean"/>: 4</item>
    /// <item><see cref="American"/>: 5</item>
    /// <item><see cref="CaliforniaSunset"/>: 6</item>
    /// <item><see cref="Royal"/>: 7</item>
    /// <item><see cref="Blue"/>: 8</item>
    /// <item><see cref="Green"/>: 9</item>
    /// <item><see cref="Red"/>: 10</item>
    /// <item><see cref="White"/>: 11</item>
    /// <item><see cref="Magenta"/>: 12</item>
    /// <item><see cref="Hold"/>: 13</item>
    /// <item><see cref="Recall"/>: 14</item>
    /// </list></code>
    /// </summary>
    public enum PoolLightMode
    {
        Sam = 1,
        Party = 2,
        Romance = 3,
        Caribbean = 4,
        American = 5,
        CaliforniaSunset = 6,
        Royal = 7,
        Blue = 8,
        Green = 9,
        Red = 10,
        White = 11,
        Magenta = 12,
        Hold = 13,
        Recall = 14
    }

    public class PoolLightModel
    {
        public PoolLightModel(PoolLightMode mode)
        {
            Mode = mode;
        }

        public PoolLightMode Mode { get; }
        public int PowerCycles => (int)Mode;

        public string ModeDisplay
        {
            get
            {
                switch (Mode)
                {
                    case PoolLightMode.Sam:
                        return "SAm Mode";
                    case PoolLightMode.Party:
                        return "Party Mode";
                    case PoolLightMode.Romance:
                        return "Romance Mode";
                    case PoolLightMode.Caribbean:
                        return "Caribbean Mode";
                    case PoolLightMode.American:
                        return "American Mode";
                    case PoolLightMode.CaliforniaSunset:
                        return "California Sunset Mode";
                    case PoolLightMode.Royal:
                        return "Royal Mode";
                    case PoolLightMode.Blue:
                        return "Blue Fixed Mode";
                    case PoolLightMode.Green:
                        return "Green Fixed Mode";
                    case PoolLightMode.Red:
                        return "Red Fixed Mode";
                    case PoolLightMode.White:
                        return "White Fixed Mode";
                    case PoolLightMode.Magenta:
                        return "Magenta Fixed Mode";
                    case PoolLightMode.Hold:
                        return "Save current color effect";
                    case PoolLightMode.Recall:
                        return "Recall the last saved color effect";
                    default:
                        throw new NotSupportedException("Color mode not supported");
                }
            }
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