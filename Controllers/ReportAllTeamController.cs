using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tele2_Metrics.Helpers;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Controllers
{
    public class ReportAllTeamController : ControllerBase
    {
        /// <summary>
        /// Поток поставки бизнес требований
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stepBacklogTime"></param>
        /// <param name="stepLeadTime"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allTeam")]
        public PlotSet allTeam(DateTime startDate, int stepBacklogTime, int stepLeadTime)
        {
            #region Запрос номеров таск из TFS, удовлетворяющих запросу
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://tfs.tele2.ru/tfs/Main/Tele2/_apis/wit/wiql?api-version=3.0");
            httpWebRequest.Method = "POST";
            httpWebRequest.Credentials = new NetworkCredential("victor.polyakh", "Zxcvbn123$");
            httpWebRequest.ContentType = "application/json; charset=utf-8; api-version=3.0";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string jsonAllTeam =
                    "{" + "\"query\":" + "\"" +
                    "select[System.Id], " +
                    "[System.AssignedTo], " +
                    "[System.Title], " +
                    "[System.State], " +
                    "[System.AreaPath], " +
                    "[System.IterationPath], " +
                    "[System.Tags], " +
                    "[System.CreatedDate], " +
                    "[Microsoft.VSTS.Common.ActivatedDate], " +
                    "[Microsoft.VSTS.Common.ClosedDate], " +
                    "[Microsoft.VSTS.Scheduling.Size] " +
                    "from WorkItems where " +
                    "[System.TeamProject] = @project and " +
                    "[System.WorkItemType] = 'Требование' and " +
                    @"([System.AreaPath] = 'Tele2\\CRM\\CRM Team DoC' or " +
                    @"[System.AreaPath] = 'Tele2\\CRM\\CRM Team A' or " +
                    @"[System.AreaPath] = 'Tele2\\CRM\\CRM Team B' or " +
                    @"[System.AreaPath] = 'Tele2\\CRM\\CRM Support') and " +
                    "[System.State] = 'Closed' and " +
                    "[System.Reason] <> 'Отклонено' and " +
                    "not [System.Tags] contains 'Техническое' and " +
                    "not [System.Tags] contains 'Орг вопросы' and " +
                    "not [System.Tags] contains 'Backend' and " +
                    "not [System.Reason] contains 'Пропала необходимость' " +
                    "order by [Microsoft.VSTS.Common.ClosedDate]" +
                    "\"" +
                    "}";

                streamWriter.Write(jsonAllTeam);
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

            ItemInfoHelper itemInfoHelper = new ItemInfoHelper();
            FINAL.resultItems = itemInfoHelper.getItemInfo(finalResult, result, FINAL);

            //Считаем LeadTime и BackLogTime для каждой таски
            foreach (ResultItem i in FINAL.resultItems)
            {
                if (i.value[0].fields.ActivatedDate != null && i.value[0].fields.CreatedDate != null)
                    i.value[0].backlogTime = (int)(DateTime.Parse(i.value[0].fields.ActivatedDate) - DateTime.Parse(i.value[0].fields.CreatedDate)).TotalDays;
                if (i.value[0].fields.ClosedDate != null && i.value[0].fields.ActivatedDate != null)
                    i.value[0].leadTime = (int)(DateTime.Parse(i.value[0].fields.ClosedDate) - DateTime.Parse(i.value[0].fields.ActivatedDate)).TotalDays;
            }

            TaskReportHelper taskReportHelper = new TaskReportHelper();
            BacklogReportHelper backlogReportHelper = new BacklogReportHelper();
            LeadReportHelper leadReportHelper = new LeadReportHelper();

            PlotSet plotResult = new PlotSet();
            plotResult.PlotList = new List<PlotData>();
            plotResult.PlotList.Add(taskReportHelper.formTaskReport(FINAL, startDate));
            plotResult.PlotList.Add(backlogReportHelper.fromBacklogReport(FINAL, stepBacklogTime, startDate));
            plotResult.PlotList.Add(leadReportHelper.formLeadReport(FINAL, stepLeadTime, startDate));
            return plotResult;
        }
    }
}
