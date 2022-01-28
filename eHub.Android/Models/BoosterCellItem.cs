using eHub.Common.Models;

namespace eHub.Android.Models
{
    public class BoosterCellItem
    {
        public PiPin BoosterPin { get; }

        public BoosterCellItem(PiPin boosterPin)
        {
            BoosterPin = boosterPin;
        }
    }
}