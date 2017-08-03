using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChackCogLib
{
    public partial class Vision
    {
        protected const string VisionAnalyzeUrl = "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze";

        public Vision()
        {

        }

        public AnalyzeResult AnalyzeImage(Uri imageUrl, string apiKey = null)
        {
            Task<string> task = Task.Run(() => Vision.Analyze(imageUrl, ResultFormat.Json, apiKey));
            return JsonConvert.DeserializeObject<AnalyzeResult>(task.Result);
        }

        public static async Task<string> Analyze(Uri imageUrl, ResultFormat format = ResultFormat.Text, string apiKey = null)
        {
            var postData = "{\"url\": \"" + imageUrl.ToString() + "\"}";
            StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");

            return await VisionAnalyzeRequest(content, format, apiKey);
        }

        public static async Task<string> Analyze(Stream image, ResultFormat format = ResultFormat.Text, string apiKey = null)
        {
            var content = new StreamContent(image);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return await VisionAnalyzeRequest(content, format, apiKey);
        }

        private static async Task<string> VisionAnalyzeRequest(HttpContent content, ResultFormat format, string apiKey)
        {
            using (var client = new HttpClient())
            {
                string key;
                if (String.IsNullOrEmpty(apiKey))
                    key = Environment.GetEnvironmentVariable("Vision_API_Subscription_Key");
                else
                    key = apiKey;

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                string requestParameters = "visualFeatures=Categories,Tags,Description,Faces,ImageType,Color,Adult";

                // Assemble the URI for the REST API Call.
                string uri = VisionAnalyzeUrl + "?" + requestParameters;


                var httpResponse = await client.PostAsync(uri, content);
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return await httpResponse.Content.ReadAsStringAsync();
                    //return (format == ResultFormat.Text) ? ConvertToText(result) : result;
                }
            }
            return null;
        }
    }

    public class AnalyzeResult
    {
        public Category[] categories { get; set; }
        public Adult adult { get; set; }
        public Tag[] tags { get; set; }
        public Description description { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
        public Face[] faces { get; set; }
        public Color color { get; set; }
        public Imagetype imageType { get; set; }
    }

    public class Adult
    {
        public bool isAdultContent { get; set; }
        public bool isRacyContent { get; set; }
        public float adultScore { get; set; }
        public float racyScore { get; set; }
    }

    public class Description
    {
        public string[] tags { get; set; }
        public Caption[] captions { get; set; }
    }

    public class Caption
    {
        public string text { get; set; }
        public float confidence { get; set; }
    }

    public class Metadata
    {
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
    }

    public class Color
    {
        public string dominantColorForeground { get; set; }
        public string dominantColorBackground { get; set; }
        public string[] dominantColors { get; set; }
        public string accentColor { get; set; }
        public bool isBWImg { get; set; }
    }

    public class Imagetype
    {
        public int clipArtType { get; set; }
        public int lineDrawingType { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public float score { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public float confidence { get; set; }
    }

    public class Face
    {
        public int age { get; set; }
        public string gender { get; set; }
        public Facerectangle faceRectangle { get; set; }
    }

    public class Facerectangle
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

}
