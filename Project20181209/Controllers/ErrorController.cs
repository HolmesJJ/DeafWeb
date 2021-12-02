using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project20181209.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Project20181209.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /<controller>/
        // 强制在调用Error控制器时调用Index操作方法
        [Route("/api/Error")]
        public IActionResult Index()
        {
            ViewBag.Title = "Error";
            return View(new ErrorViewModel { StatusCode = -1, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: /<controller>/HttpError
        [Route("/api/Error/HttpError/{id}")]
        public IActionResult HttpError(int id)
        // id是Status Codes, "{controller=Home}/{action=Index}/{id?}"中被定义
        {
            ViewBag.Title = "Http Error";
            return View(new ErrorViewModel { StatusCode = id, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
