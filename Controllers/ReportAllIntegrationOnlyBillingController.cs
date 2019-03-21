﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Newtonsoft.Json;
using Tele2_Metrics.Helpers;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Controllers
{
    public class ReportController : Controller
    {
        protected ConfigurationModel Configuration { get; set; }
        public ReportController(IOptions<ConfigurationModel> configuration)
        {
            Configuration = configuration.Value;
        }

        /// <summary>
        /// Требования с интеграцией только Биллинг ВСЕ Команды
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stepBacklogTime"></param>
        /// <param name="stepLeadTime"></param>
        /// <param name="queryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allIntegrationOnlyBilling")]
        public PlotSet allIntegrationOnlyBilling(DateTime startDate, int stepBacklogTime, int stepLeadTime, string queryId)
        {

            #region Запрос номеров таск из TFS, удовлетворяющих запросу
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://tfs.tele2.ru/tfs/Main/Tele2/_apis/wit/wiql?api-version=3.0");
            httpWebRequest.Method = "POST";
            httpWebRequest.Credentials = new NetworkCredential("service.crm.tfs", "$93aCZApFsW");
            httpWebRequest.ContentType = "application/json; charset=utf-8; api-version=3.0";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                QueryModel queryModel = new QueryHelper(Configuration).GetByIdHelper(queryId);
                string AllIntegration_OnlyBilling =
                    "{" + "\"query\":" + "\"" +
                    "select [System.Id], " +
                    "[System.CreatedDate], " +
                    "[Microsoft.VSTS.Common.ActivatedDate], " +
                    "[Microsoft.VSTS.Common.ClosedDate], " +
                    "[System.WorkItemType], " +
                    "[System.Title], " +
                    "[System.AssignedTo], " +
                    "[System.State], " +
                    "[System.AreaPath], " +
                    "[System.Tags] from WorkItems where " +
                    "[System.TeamProject] = @project and " +
                    "[System.WorkItemType] = 'Требование' and " +
                    @"[System.AreaPath] under 'Tele2\\CRM' and " +
                    @"[System.AreaPath] <> 'Tele2\\CRM\\CRM Team Bercut' and " +
                    @"[System.AreaPath] <> 'Tele2\\CRM\\CRM Team AutoTests' and " +
                    @"not [System.IterationPath] under 'Tele2\\Партнеры\\CRM\\АРМ Troubleshooting\\Отклоненные' and " +
                    @"not [System.IterationPath] under 'Tele2\\Партнеры\\CRM\\Отклоненные' and " +
                    "([System.Tags] contains 'Беркут' or " +
                    "[System.Tags] contains 'Биллинг') and not " +
                    "[System.Tags] contains 'Trash' and " +
                    "[System.State] <> 'New' and not " +
                    "[System.Tags] contains 'Без разработки' and " +
                    "not [System.Tags] contains 'Техническое' and " +
                    "not [System.Reason] contains 'Пропала необходимость' and " +
                    "[System.Reason] <> 'Отклонено' " +
                    "order by[Microsoft.VSTS.Common.ClosedDate] " +
                    "\"" +
                    "}";

                streamWriter.Write(queryModel.Query);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            dynamic resultOfQuery;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                resultOfQuery = streamReader.ReadToEnd();
            }
            Report result = JsonConvert.DeserializeObject<Report>(resultOfQuery);
            #endregion

            FinalResult FINAL = new FinalResult();
            FINAL.resultItems = new List<ResultItem>();
            ResultItem finalResult = new ResultItem();

            //ItemInfoHelper itemInfoHelper = new ItemInfoHelper();
            FINAL.resultItems = new ItemInfoHelper().getItemInfo(finalResult, result, FINAL);

            //Считаем LeadTime и BackLogTime для каждой таски
            foreach (ResultItem i in FINAL.resultItems)
            {
                if (i.value[0].fields.ActivatedDate != null && i.value[0].fields.CreatedDate != null)
                    i.value[0].backlogTime = (int)(DateTime.Parse(i.value[0].fields.ActivatedDate) - DateTime.Parse(i.value[0].fields.CreatedDate)).TotalDays;
                if (i.value[0].fields.ClosedDate != null && i.value[0].fields.ActivatedDate != null)
                    i.value[0].leadTime = (int)(DateTime.Parse(i.value[0].fields.ClosedDate) - DateTime.Parse(i.value[0].fields.ActivatedDate)).TotalDays;
            }

            //TaskReportHelper taskReportHelper = new TaskReportHelper();
            //BacklogReportHelper backlogReportHelper = new BacklogReportHelper();
            //LeadReportHelper leadReportHelper = new LeadReportHelper();

            PlotSet plotResult = new PlotSet();
            plotResult.PlotList = new List<PlotData>();
            plotResult.PlotList.Add(new TaskReportHelper().formTaskReport(FINAL, startDate));
            plotResult.PlotList.Add(new BacklogReportHelper().fromBacklogReport(FINAL, stepBacklogTime, startDate));
            plotResult.PlotList.Add(new LeadReportHelper().formLeadReport(FINAL, stepLeadTime, startDate));
            return plotResult;
        }
    }
}

