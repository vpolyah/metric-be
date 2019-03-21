using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tele2_Metrics.Models
{
    public class ResultItem
    {
        public List<Item> value { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public int rev { get; set; }
        public Field fields { get; set; }
        public string url { get; set; }
        public int leadTime { get; set; }
        public int backlogTime { get; set; }
    }

    public class Field
    {
        public string Id { get; set; }
        public string AreaPath { get; set; }
        public string IterationPath { get; set; }
        public string WorkItemType { get; set; }
        public string State { set; get; }
        public string AssignedTo { get; set; }
        public string CreatedDate { get; set; }
        public string ActivatedDate { get; set; }
        public string ClosedDate { get; set; }
        public string Title { get; set; }
    }
}
