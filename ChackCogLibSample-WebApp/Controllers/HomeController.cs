using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChackCogLibSample.Models;
using ChackCogLib;

namespace ChackCogLibSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Sample Application for .NET Standard, .NET Core and Cognitive Services.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Home/Ocr
        public ActionResult Ocr()
        {
            return View();
        }

        // POST: Home/Ocr
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ocr(Image imageData)
        {
            try
            {
                Uri image = new Uri(imageData.ImageUrl);

                // Setup for OcrLib
                // 1) Go to https://www.microsoft.com/cognitive-services/en-us/computer-vision-api 
                //    Sign up for computer vision api
                // 2) Add environment variable "Vision_API_Subscription_Key" and set Computer vision key as value
                //    e.g. Vision_API_Subscription_Key=123456789abcdefghijklmnopqrstuvw

                Task<string> task = Task.Run(() => Vision.Ocr(image));
                task.Wait();

                imageData.Result = task.Result;

                return View("OcrView", imageData);
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Noodle
        public ActionResult Noodle()
        {
            return View();
        }

        // POST: Home/Noodle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Noodle(Image imageData)
        {
            try
            {
                Uri imageUrl = new Uri(imageData.ImageUrl);

                Task<Prediction> task = Task.Run(() => NoodleFinder.Noodle(imageUrl));
                task.Wait();

                imageData.Result = ConvertToText(task.Result);

                return View("NoodleView", imageData);
            }
            catch
            {
                return View();
            }
        }

        protected string ConvertToText(Prediction result)
        {
            string msg;

            if (result.Tag == "Udon")
                msg = string.Format("うどん です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.Tag == "OkinawaSoba")
                msg = string.Format("沖縄そば です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.Tag == "Soba")
                msg = string.Format("蕎麦 です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.Tag == "Ramen")
                msg = string.Format("ラーメン です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.Tag == "HiyashiChuka")
                msg = string.Format("冷やし中華 です (確度 {0})", result.Probability.ToString("P1"));
            else if (result.Tag == "Jiro")
                msg = string.Format("二郎 です (確度 {0})", result.Probability.ToString("P1"));
            else
                return "判別できませんでした";

            if (result.Probability < 0.6F)
                msg = "たぶん " + msg;

            return msg;
        }
    }
}
