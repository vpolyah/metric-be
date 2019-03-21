using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tele2.Crm.Messaging;
using Tele2_Metrics.Helpers;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Controllers
{
    [Route("query")]
    public class QueryController : Controller
    {
        protected ConfigurationModel Configuration { get; set; }
        protected QueryHelper QueryHelper {  get;}
        public QueryController(IOptions<ConfigurationModel> configuration)
        {
            Configuration = configuration.Value;
            QueryHelper = new QueryHelper(configuration.Value);
        }

        [Route("save")]
        [HttpPost]
        public Message SaveQuery([FromBody]QueryRequestModel request)
        {
            QueryHelper.SaveQueryHelper(request.Name, request.Query, request.GroupId);
            return new SuccessMessage
            {
                Data = null,
                MessageText = "Query Saved"
            };
        }

        [Route("get-all")]
        [HttpGet]
        public Message GetAllGroups()
        {
            return new SuccessMessage
            {
                Data = QueryHelper.GetAllGroups(),
                MessageText = "Find all groups"
            };
        }

        [Route("get-queries")]
        [HttpGet]
        public Message GetQueries([FromQuery] string groupId)
        {
            return new SuccessMessage
            {
                Data = QueryHelper.GetQueriesByGroupHelper(groupId),
                MessageText = "Find group query"
            };
        }

        [Route("get-one")]
        [HttpGet]
        public Message GetById([FromQuery] string id)
        {
            return new SuccessMessage
            {
                Data = QueryHelper.GetByIdHelper(id),
                MessageText = "Find one query"
            };
        }

        [Route("delete")]
        [HttpGet]
        public Message Delete([FromQuery] string id)
        {
            QueryHelper.DeleteById(id);
            return new SuccessMessage
            {
                Data = null,
                MessageText = "Delete one query"
            };
        }
    }
}