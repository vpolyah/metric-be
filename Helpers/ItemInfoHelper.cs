using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tele2_Metrics.Models;

namespace Tele2_Metrics.Helpers
{
    public class ItemInfoHelper
    {
        public List<ResultItem> getItemInfo (ResultItem finalResult, Report result, FinalResult FINAL)
        {
            for (int i = 0; i < result.workItems.Count; i++)
            {
                string myParameters = "?ids=" + result.workItems[i].Id + "&" +
                                      "fields=" + "System.Id,System.CreatedDate,System.IterationPath,System.WorkItemType,System.Title,System.AssignedTo,System.State,System.AreaPath,System.Tags,Microsoft.VSTS.Common.ActivatedDate,Microsoft.VSTS.Common.ClosedDate&" +
                                      "api-version=" + "3.0";

                string strReturn = null;
                try
                {
                    HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create("https://tfs.tele2.ru/tfs/Main/_apis/wit/workitems" + myParameters);
                    WebReq.Credentials = new NetworkCredential("service.crm.tfs", "$93aCZApFsW");

                    //Set method to post, otherwise postvars will not be used
                    WebReq.Method = "GET";
                    WebReq.ContentType = "application/x-www-form-urlencoded";

                    //Get the response handle, we have no true response yet
                    HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                    string server = WebResp.Server;
                    //read the response
                    Stream WebResponse = WebResp.GetResponseStream();
                    StreamReader _response = new StreamReader(WebResponse);
                    strReturn = _response.ReadToEnd();
                    strReturn = strReturn.Replace("System.", "");
                    strReturn = strReturn.Replace("Microsoft.VSTS.Common.", "");
                    finalResult = JsonConvert.DeserializeObject<ResultItem>(strReturn);
                }
                catch (Exception ex)
                {
                    string MessageText = ex.Message;
                }
                FINAL.resultItems.Add(finalResult);
            }
            return FINAL.resultItems;
        }
    }
}
