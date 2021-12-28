using eHub.Common.Models;
using System.Collections.Generic;

namespace eHub.Android.Models
{
    public class DeviceCellItem
    {
        public List<PiPin> DevicePins { get; }

        public DeviceCellItem(List<PiPin> devicePins)
        {
            DevicePins = devicePins;
        }
    }
}