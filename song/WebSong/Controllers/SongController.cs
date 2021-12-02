using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using comparison_audio;
using MathWorks.MATLAB.NET.Arrays;


namespace WebSong.Controllers
{
    public class SongController : ApiController
    {
        private Random _rand = new Random();

        // GET: api/Song
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Song/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Song
        public JsonResult<double[]> Post(IEnumerable<HttpPostedFileBase> files)
        { 
           
            return Json<double[]>(new double[]{});
        }

        // PUT: api/Song/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Song/5
        public void Delete(int id)
        {
        }
    }
}
