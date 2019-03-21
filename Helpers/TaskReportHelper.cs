using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Helpers
{
    public class TaskReportHelper
    {
        public PlotData formTaskReport(FinalResult FINAL, DateTime startDate)
        {
            //DateTime startDate = new DateTime(2018, 3, 1);

            int step = 1;
            var plot = new PlotData();
            plot.DataName = "Сколько берется в работу";
            plot.Periods = new List<Period>();

            for (DateTime date = startDate; date < DateTime.Today; date = date.AddMonths(step))
            {
                //создаём объект типа период, в который будем включать таски, попавшие в промежуток startDate/endDate
                var period = new Period();
                period.items = new List<ResultItem>();
                period.startDate = date;
                period.endDate = date.AddMonths(step);

                //Определяем сколько таск было создано/закрыто/взято в работу в текщий период
                foreach (ResultItem i in FINAL.resultItems)
                {
                    if (i.value[0].fields.ActivatedDate != null && DateTime.Parse(i.value[0].fields.ActivatedDate) < date.AddMonths(step) && DateTime.Parse(i.value[0].fields.ActivatedDate) > date)
                    {
                        period.items.Add(i);
                        period.activatedCount++;
                    }
                    if (i.value[0].fields.ClosedDate != null && DateTime.Parse(i.value[0].fields.ClosedDate) < date.AddMonths(step) && DateTime.Parse(i.value[0].fields.ClosedDate) > date)
                    {
                        period.items.Add(i);
                        period.closedCount++;
                    }
                    if (DateTime.Parse(i.value[0].fields.CreatedDate) < date.AddMonths(step) && DateTime.Parse(i.value[0].fields.CreatedDate) > date)
                    {
                        period.items.Add(i);
                        period.createdCount++;
                    }
                }
                plot.Periods.Add(period);
            }
            return plot;
        }
    }
}
