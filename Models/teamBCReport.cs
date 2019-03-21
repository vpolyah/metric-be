using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Models
{
    public class Report
    {
        public string queryType { get; set; }
        public string aueryResultType { get; set; }
        public string asOf { get; set; }
        public IList<Columns> columns { get; set; }
        public IList<WorkItem> workItems { get; set; }
    }
}
