using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using eHub.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eHub.Android.Models
{
    public class PoolLightGridItem
    {
        public PoolLightMode SelectedLightMode { get; set; }
        public Func<PoolLightModel, TextView, Task<PoolLightModel>> PoolLightModeButtonTapped { get; set; }
        public Func<PoolLightModel, TextView, Task<PoolLightModel>> SpaLightModeButtonTapped { get; set; }
    }
}