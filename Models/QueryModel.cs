using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tele2_Metrics.Models
{
    public class AnswerModel
    {
        public List<QueryModel> Queries { get; set; }
        public List<GroupModel> Groups { get; set; }
    }
    public class QueryModel
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Query { get; set; }
        public ObjectId GroupId { get; set; }
    }

    public class GroupModel
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
