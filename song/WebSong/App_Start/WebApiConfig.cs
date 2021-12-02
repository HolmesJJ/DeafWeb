using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using comparison_audio;

namespace WebSong
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            try
            {
                CmpAudio cmpAudio = new CmpAudio();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
