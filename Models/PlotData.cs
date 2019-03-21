using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tele2_Metrics.Models
{
    public class PlotData
    {
        public string DataName { get; set; }
        public List<Period> Periods { get; set; }
    }

    public class PlotSet
    {
        public List<PlotData> PlotList { get; set; }
    }
}
