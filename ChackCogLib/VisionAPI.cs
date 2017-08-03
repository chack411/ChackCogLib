using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChackCogLib
{
    public partial class Vision
    {
        protected const string VisionOcrUrl = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr";

        public enum ResultFormat { Text, Json };

        public static async Task<string> Ocr(Uri imageUrl, ResultFormat format = ResultFormat.Text, string apiKey = null)
        {
            var postData = "{\"url\": \"" + imageUrl.ToString() + "\"}";
            StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");

            return await VisionOcrRequest(content, format, apiKey);
        }

        public static async Task<string> Ocr(Stream image, ResultFormat format = ResultFormat.Text, string apiKey = null)
        {
            var content = new StreamContent(image);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return await VisionOcrRequest(content, format, apiKey);
        }

        private static async Task<string> VisionOcrRequest(HttpContent content, ResultFormat format, string apiKey)
        {
            using (var client = new HttpClient())
            {
                string key;
                if (String.IsNullOrEmpty(apiKey))
                    key = Environment.GetEnvironmentVariable("Vision_API_Subscription_Key");
                else
                    key = apiKey;

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                var httpResponse = await client.PostAsync(VisionOcrUrl, content);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    string result = await httpResponse.Content.ReadAsStringAsync();
                    return (format == ResultFormat.Text) ? ConvertToText(result) : result;
                }
            }
            return null;
        }

        protected static string ConvertToText(string jsonData)
        {
            var ocrResult = new StringBuilder();

            OcrData ocrData = JsonConvert.DeserializeObject<OcrData>(jsonData);
            foreach (Region region in ocrData.Regions)
            {
                foreach (Line line in region.Lines)
                {
                    foreach (Word word in line.Words)
                    {
                        ocrResult.Append(word.Text);
                        if (ocrData.Language != "ja")
                            ocrResult.Append(" ");
                    }
                    ocrResult.Append("\n");
                }
            }

            return ocrResult.ToString();
        }
    }

    public class OcrData
    {
        public string Language { get; set; }
        public string Orientation { get; set; }
        public List<Region> Regions { get; set; }
    }

    public class Region
    {
        public List<Line> Lines { get; set; }
    }

    public class Line
    {
        public List<Word> Words { get; set; }
    }

    public class Word
    {
        public string Text { get; set; }
    }
}
