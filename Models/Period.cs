using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tele2_Metrics.Models
{
    public class Period
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public List<ResultItem> items { get; set; }

        public int createdCount { get; set; }
        public int activatedCount { get; set; }
        public int closedCount { get; set; }

        public int percentilLeadTime { get; set; }
        public int percentilBacklogTime { get; set; }
    }
}
