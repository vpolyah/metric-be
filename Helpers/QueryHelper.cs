using Microsoft.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Helpers
{
    public class QueryHelper
    {
        private static MongoClient Client { get; set; }
        private static IMongoDatabase MetricDatabase { get; set; }
        private static IMongoCollection<QueryModel> QueriesCollection { get; set; }
        private static IMongoCollection<GroupModel> GroupsCollection { get; set; }

        private static object syncRoot = new object();

        public QueryHelper(ConfigurationModel Configuration)
        {
            if (MetricDatabase == null && QueriesCollection == null)
            {
                lock (syncRoot)
                {
                    if (Client == null)
                    {
                        var connectionString = Configuration.DbAddress;
                        Client = new MongoClient(connectionString);
                        MetricDatabase = Client.GetDatabase(Configuration.DbName);
                        QueriesCollection = MetricDatabase.GetCollection<QueryModel>(Configuration.CollectionName);
                        GroupsCollection = MetricDatabase.GetCollection<GroupModel>(Configuration.GroupCollectionName);
                    }
                }
            }
        }

        public void SaveQueryHelper(string name, string query, string groupId)
        {
            var savedGroupIdString = ObjectId.GenerateNewId();
            try
            {
                savedGroupIdString = GroupsCollection.Find(q => q.Name == "Saved").FirstOrDefault().Id;
            }
            catch (Exception)
            {
                savedGroupIdString = ObjectId.Parse("5c7416681c9d4400004a9cb0");
            }

            var outGroupId = ObjectId.GenerateNewId();
            var isParseSuccess = ObjectId.TryParse(groupId, out outGroupId);

            if (!string.IsNullOrEmpty(query))
            {
                query = query.Replace("/", @"\\");
                query = "{\"query\":\"" + query + "\"}";
            }

            QueriesCollection.InsertOne(new QueryModel
            {
                Name = name,
                Query = query,
                GroupId = isParseSuccess ? outGroupId : savedGroupIdString
            });
        }

        public List<QueryModel> GetQueriesByGroupHelper(string id) => QueriesCollection.Find(q => q.GroupId == ObjectId.Parse(id)).ToList();

        public List<GroupModel> GetAllGroups() => GroupsCollection.AsQueryable().ToList();

        public QueryModel GetByIdHelper(string id) => QueriesCollection.Find(q => q.Id == ObjectId.Parse(id)).FirstOrDefault();

        public void DeleteById(string id) => QueriesCollection.FindOneAndDelete(q => q.Id == ObjectId.Parse(id));
    }
}
