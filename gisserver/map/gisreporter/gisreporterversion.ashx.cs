using System.IO;
using System.Web;

namespace gisserver.map
{
    /// <summary>
    /// Summary description for gisreporterversion
    /// </summary>
    public class gisreporterversion : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string folder = HttpContext.Current.Server.MapPath("release");
            gisreporter.gisReporterPackage package = gisreporter.gisReporterPackage.Create(folder);
            File.WriteAllText(folder + @"\package.json", package.ToString());

            context.Response.ContentType = "text/plain";
            context.Response.Write(package.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}