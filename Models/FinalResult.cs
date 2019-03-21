using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tele2_Metrics.Models
{
    public class FinalResult
    {
        public List<ResultItem> resultItems { get; set; }
        public  int averageLeadTime { get; set; }
        public int averageBackLogTime { get; set; }
    }
}
