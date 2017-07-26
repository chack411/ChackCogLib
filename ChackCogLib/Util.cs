using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChackCogLib
{
    public class Util
    {
        public static async Task<string> CallSlackWebHook(string text, string webHookUrl, string userName)
        {
            using (var client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(new { text = text, username = userName });
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResponse = await client.PostAsync(webHookUrl, httpContent);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return "OK";
                }
            }
            return null;
        }
    }
}
