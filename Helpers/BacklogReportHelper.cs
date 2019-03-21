using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Helpers
{
    public class BacklogReportHelper
    {
        public PlotData fromBacklogReport(FinalResult FINAL, int stepBacklogTime, DateTime startDate)
        {
            var backlogTimePlot = new PlotData();
            backlogTimePlot.DataName = "Перцентиль по backlogTime";
            backlogTimePlot.Periods = new List<Period>();

            for (DateTime date = startDate; date < DateTime.Today; date = date.AddMonths(1))
            {
                if (date.AddMonths(stepBacklogTime) > DateTime.Today)
                {
                    break;
                }
                else
                {
                    var period = new Period();
                    period.items = new List<ResultItem>();
                    period.startDate = date;
                    period.endDate = date.AddMonths(stepBacklogTime);

                    List<ResultItem> backlogTimeList = new List<ResultItem>();

                    foreach (ResultItem i in FINAL.resultItems)
                    {
                        if (i.value[0].fields.ActivatedDate != null && DateTime.Parse(i.value[0].fields.ActivatedDate) < date.AddMonths(stepBacklogTime) && DateTime.Parse(i.value[0].fields.ActivatedDate) >= date)
                        {
                            //if (i.value[0].backlogTime != 0)
                            //{
                            period.items.Add(i);
                            // backlogTimeList.Add(i.value[0].backlogTime);
                            //}
                        }
                    }

                    period.items = period.items.OrderBy(o => o.value[0].backlogTime).ToList();

                    var percentil = Math.Round(period.items.Count * 0.8, 1);
                    if (percentil != 0 && percentil > 1)
                    {
                        var date1 = date;
                        var date2 = date.AddMonths(3);
                        var percentilFraction = Math.Round(percentil - (int)percentil, 1);
                        var TaskDeltaForPercentil = period.items[(int)percentil].value[0].backlogTime - period.items[(int)percentil - 1].value[0].backlogTime;
                        var percentilResult = period.items[(int)percentil - 1].value[0].backlogTime + (TaskDeltaForPercentil * percentilFraction); //значение по перцентилю, которое, помимо целой части, учитывает дробную часть перцентиля
                        period.percentilBacklogTime = (int)Math.Round(percentilResult);
                    }
                    backlogTimePlot.Periods.Add(period);
                }
            }
            return backlogTimePlot;
        }
    }
}
