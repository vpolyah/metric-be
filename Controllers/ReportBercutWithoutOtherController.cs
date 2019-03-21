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
    public class ReportBercutWithoutOtherController : ControllerBase
    {
        /// <summary>
        /// Bercut - требования без тега Other
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="stepBacklogTime"></param>
        /// <param name="stepLeadTime"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("bercutWithoutOther")]
        public PlotSet bercutWithoutOther(DateTime startDate, int stepBacklogTime, int stepLeadTime)
        {
            #region Запрос номеров таск из TFS, удовлетворяющих запросу
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://tfs.tele2.ru/tfs/Main/Tele2/_apis/wit/wiql?api-version=3.0");
            httpWebRequest.Method = "POST";
            httpWebRequest.Credentials = new NetworkCredential("user", "password");
            httpWebRequest.ContentType = "application/json; charset=utf-8; api-version=3.0";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string BercutWithoutOther =
                    "{" + "\"query\":" + "\"" +
                    "select [System.Id], " +
                    "[System.CreatedDate], " +
                    "[Microsoft.VSTS.Common.ActivatedDate], " +
                    "[Microsoft.VSTS.Common.ClosedDate], " +
                    "[System.WorkItemType], " +
                    "[System.Title], [System.State], " +
                    "[System.AreaPath], " +
                    "[System.IterationPath], " +
                    "[System.Tags] from WorkItems where " +
                    "[System.TeamProject] = @project and " +
                    "([System.WorkItemType] = 'Требование' and " +
                    @"[System.CreatedBy] in ('Гордеев Александр Валериевич <T2RU\\a.gordeev>', " +
                    @"'Муратов Михаил Александрович <T2RU\\mikhail.muratov>', " +
                    @"'Юренков Станислав Сергеевич <T2RU\\supp.Dynamics21>', " +
                    @"'Евланов Сергей Леонидович <T2RU\\sergey.evlanov>', " +
                    @"'Семенов Антон Юрьевич <T2RU\\anton.y.semenov>', " +
                    @"'Кораблёва Екатерина Викторовна <T2RU\\supp.Dynamics11>', " +
                    @"'Иванов Арсений Дмитриевич <T2RU\\Supp.Dynamics02>', " +
                    @"'Анисимов Максим Александрович <T2RU\\supp.Dynamics12>', " +
                    @"'Фокин Андрей Геннадьевич <T2RU\\supp.Dynamics19>', " +
                    @"'Свидэрский Николай Сергеевич <T2RU\\nikolay.svidersky>', " +
                    @"'Панфилова Наталья Геннадьевна <T2RU\\natalya.panfilova>', " +
                    @"'Большакова Мария Владимировна <T2RU\\supp.Dynamics17>', " +
                    @"'Смирнов Фёдор Леонидович <T2RU\\supp.Dynamics20>', " +
                    @"'Недоступ Александр Николаевич <T2RU\\alexander.nedostup>', " +
                    @"'Нижников Виталий Сергеевич <T2RU\\vitaly.nizhnikov>', " +
                    @"'Чубукина Татьяна Сергеевна <T2RU\\Supp.Dynamics10>', " +
                    @"'Курилюк Антон Викторович <T2RU\\anton.kurilyuk>', " +
                    @"'Бондарчук Евгений Маркович <T2RU\\Supp.Dynamics08>', " +
                    @"'Чистяков Максим Сергеевич <T2RU\\supp.Dynamics18>', " +
                    @"'Манукян Ашот Каренович <T2RU\\ashot.manukyan>', " +
                    @"'Моисеева Ольга Алексеевна <T2RU\\supp.DSuite29>', " +
                    @"'Калашников Дмитрий Юрьевич <T2RU\\dmitry.y.kalashnikov>', " +
                    @"'Полях Виктор Васильевич <T2RU\\victor.polyakh>', " +
                    @"'Остапенко Сергей Сергеевич <T2RU\\sergey.ostapenko>', " +
                    @"'Глушков Сергей Иванович <T2RU\\Supp.Dynamics05>', " +
                    @"'Жиряков Дмитрий Владимирович <T2RU\\supp.Dynamics15>', " +
                    @"'Тришечкин Евгений Владимирович <T2RU\\Supp.Dynamics06>', " +
                    @"'Арсеньева Юлия Андреевна <T2RU\\supp.Dynamics16>', " +
                    @"'Хромина Ольга Анатольевна <T2RU\\olga.khromina>', " +
                    @"'Сибакова Евгения Александровна <T2RU\\evgenia.sibakova>', " +
                    @"'Артюков Иван Алексеевич <T2RU\\Supp.Dynamics03>', " +
                    @"'Хрупин Дмитрий Владимирович <T2RU\\dmitry.khrupin>', " +
                    @"'Сологуб Николай Сергеевич <T2RU\\Supp.Dynamics24>') and " +
                    @"[System.AreaPath] in ('Tele2\\Партнеры\\Беркут\\Product testing') and " +
                    @"[System.Reason] <> 'Пропала необходимость/Отклонено' and " +
                    "[System.Reason] <> 'Отклонено' and " +
                    "not [System.Tags] contains 'Trash' and " +
                    "not [System.Tags] contains 'Техническое' and " +
                    "not [System.Tags] contains 'Other' and  " +
                    @"not [System.IterationPath] under 'Tele2\\Партнеры\\CRM\\Отклоненные' or " +
                    "([System.WorkItemType] = 'Требование' and " +
                    @"[System.AreaPath] = 'Tele2\\CRM\\CRM Team Bercut' and " +
                    "[System.Reason] <> 'Пропала необходимость/Отклонено' and " +
                    "[System.Reason] <> 'Отклонено' and  " +
                    @"not [System.IterationPath] under 'Tele2\\Партнеры\\CRM\\Отклоненные' and " +
                    "not [System.Tags] contains 'Other' and " +
                    "not [System.Tags] contains 'Техническое' and " +
                    "not [System.Tags] contains 'Trash')) " +
                    "order by[Microsoft.VSTS.Common.ClosedDate] " +
                    "\"" +
                    "}";

                streamWriter.Write(BercutWithoutOther);
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



