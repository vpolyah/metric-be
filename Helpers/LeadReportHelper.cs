using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Helpers
{
    public class LeadReportHelper
    {
        public PlotData formLeadReport(FinalResult FINAL, int stepLeadTime, DateTime startDate)
        {
            //int stepLeadTime = 3;
            var leadTimePlot = new PlotData();
            leadTimePlot.DataName = "Перцентиль по leadTime";
            leadTimePlot.Periods = new List<Period>();

            for (DateTime date = startDate; date < DateTime.Today; date = date.AddMonths(1))
            {
                if (date.AddMonths(stepLeadTime) > DateTime.Today)
                {
                    break;
                }
                else
                {
                    var period = new Period();
                    period.items = new List<ResultItem>();
                    period.startDate = date;
                    period.endDate = date.AddMonths(stepLeadTime);

                    List<ResultItem> leadTimeList = new List<ResultItem>();

                    foreach (ResultItem i in FINAL.resultItems)
                    {
                        if (i.value[0].fields.ClosedDate != null && DateTime.Parse(i.value[0].fields.ClosedDate) < date.AddMonths(stepLeadTime) && DateTime.Parse(i.value[0].fields.ClosedDate) >= date && i.value[0].fields.ActivatedDate != null)
                        {
                            //if (i.value[0].backlogTime != 0)
                            //{
                            period.items.Add(i);
                            // backlogTimeList.Add(i.value[0].backlogTime);
                            //}
                        }
                    }

                    period.items = period.items.OrderBy(o => o.value[0].leadTime).ToList();
                    var percentil = Math.Round(period.items.Count * 0.8, 1);
                    if (percentil != 0)
                    {
                        var date1 = date;
                        var date2 = date.AddMonths(3);
                        if (percentil > 1)
                        {
                            var percentilFraction = Math.Round(percentil - (int)percentil, 1);
                            var TaskDeltaForPercentil = period.items[(int)percentil].value[0].leadTime - period.items[(int)percentil - 1].value[0].leadTime;
                            var percentilResult = period.items[(int)percentil - 1].value[0].leadTime + (TaskDeltaForPercentil * percentilFraction); //значение по перцентилю, которое, помимо целой части, учитывает дробную часть перцентиля
                            period.percentilLeadTime = (int)Math.Round(percentilResult);
                        }
                        else
                        {
                            var percentilFraction = Math.Round(percentil - (int)percentil, 1);
                            var TaskDeltaForPercentil = period.items[0].value[0].leadTime;
                            var percentilResult = 0 + (TaskDeltaForPercentil * percentilFraction); //значение по перцентилю, которое, помимо целой части, учитывает дробную часть перцентиля
                            period.percentilLeadTime = (int)Math.Round(percentilResult);
                        }
                    }
                    leadTimePlot.Periods.Add(period);
                }
            }
            return leadTimePlot;
        }
    }
}
