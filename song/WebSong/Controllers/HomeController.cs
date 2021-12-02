using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using comparison_audio;
using MathWorks.MATLAB.NET.Arrays;

namespace WebSong.Controllers
{
    public class CmpResult
    {
        public double[] Diff { get; set; }
        public double[] D1 { get; set; }
        public double[] D2 { get; set; }
        public Int32[] Step1 { get; set; }
        public Int32[] Step2 { get; set; }
        public Int32[] StepDiff { get; set; }

    }

    public class HomeController : Controller
    {
        private Random _rand = new Random();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            var contentRootPath = HostingEnvironment.MapPath("~/Upload/");
            Directory.CreateDirectory(contentRootPath);
            string uploadPath = contentRootPath;


            var httpPostedFileBases = files as HttpPostedFileBase[] ?? files.ToArray();
            if (httpPostedFileBases.Count() < 2)
            {
                return Json(new Double[] { });
            }

            var name0 = _rand.Next().ToString() + Path.GetFileName(httpPostedFileBases[0].FileName);
            httpPostedFileBases[0].SaveAs(Path.Combine(uploadPath,name0));
            var name1 = _rand.Next().ToString() + Path.GetFileName(httpPostedFileBases[1].FileName);
            httpPostedFileBases[1].SaveAs(Path.Combine(uploadPath,name1));

            CmpAudio cmpAudio = new CmpAudio();
            MWCharArray d1_path = Path.Combine(uploadPath, name0);
            MWCharArray d2_path = Path.Combine(uploadPath, name1);
            var r = cmpAudio.comparison_audio(6,d1_path, d2_path);
            CmpResult cmpResult = new CmpResult();
            cmpResult.Diff = r[0].ToArray().Cast<double>().ToArray();
            cmpResult.D1 = r[1].ToArray().Cast<double>().ToArray();
            cmpResult.D2 = r[2].ToArray().Cast<double>().ToArray();
            cmpResult.Step1 = r[3].ToArray().Cast<Int32>().ToArray();
            cmpResult.Step2 = r[4].ToArray().Cast<Int32>().ToArray();
            cmpResult.StepDiff = r[5].ToArray().Cast<Int32>().ToArray();
            
            return Json(cmpResult);
        }
    }
}