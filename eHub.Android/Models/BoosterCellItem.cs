using eHub.Common.Models;

namespace eHub.Android.Models
{
    public class BoosterCellItem
    {
        public PiPin BoosterPin1 { get; }
        public PiPin BoosterPin2 { get; }

        public BoosterCellItem(PiPin pin1, PiPin pin2)
        {
            BoosterPin1 = pin1;
            BoosterPin2 = pin2;
        }
    }
}