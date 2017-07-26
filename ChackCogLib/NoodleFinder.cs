using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChackCogLib
{
    public class NoodleFinder
    {
        protected static string predictionURL = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/5487c67b-7cee-4f68-8cf2-3027c939eb66/url?iterationId=3798ed34-4f42-43f0-b7c9-d506b0630410";

        public static async Task<Prediction> Noodle(Uri imageUrl)
        {
            using (var client = new HttpClient())
            {
                var postData = "{\"Url\": \"" + imageUrl.ToString() + "\"}";
                StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Prediction-Key", Environment.GetEnvironmentVariable("Noodle_Prediction_Key"));
                var httpResponse = await client.PostAsync(predictionURL, content);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    return GetTopPredictionResult(await httpResponse.Content.ReadAsStringAsync());
                }
            }
            return null;
        }

        public static Prediction GetTopPredictionResult(string jsonData)
        {
            NoodlePrediction NoodleData = JsonConvert.DeserializeObject<NoodlePrediction>(jsonData);

            Prediction result = new Prediction
            {
                Tag = "",
                Probability = 0.0F
            };

            foreach (Prediction prediction in NoodleData.Predictions)
            {
                if (prediction.Probability > result.Probability)
                    result = prediction;
            }

            return result;
        }
    }

    public class NoodlePrediction
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Iteration { get; set; }
        public DateTime Created { get; set; }
        public Prediction[] Predictions { get; set; }
    }

    public class Prediction
    {
        public string TagId { get; set; }
        public string Tag { get; set; }
        public float Probability { get; set; }
    }
}
