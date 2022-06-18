using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;

namespace gisserver.map.gisreporter
{
    public class gisReporterPackage
    {
        public DateTime PublishDateUTC { get; set; }
        public string[] Files { get; set; }
        private string folder;

        public gisReporterPackage() { }

        public static gisReporterPackage Create(string appFolder)
        {
            gisReporterPackage p = new gisReporterPackage();
            p.PublishDateUTC = DateTime.UtcNow.Subtract(new TimeSpan(36500, 0, 0, 0));
            p.folder = appFolder;
            p.Files = Directory.GetFiles(p.folder);

            for (int i = 0; i < p.Files.Length; i++)
            {
                if (!p.Files[i].EndsWith("package.json"))
                {
                    DateTime lastModified = System.IO.File.GetLastWriteTimeUtc(p.Files[i]);
                    if (lastModified > p.PublishDateUTC)
                    {
                        p.PublishDateUTC = lastModified;
                    }
                }
            }
            return p;
        }

        public override string ToString()
        {
            for (int i = 0; i < Files.Length; i++)
            {
                int lastSlash = Files[i].LastIndexOf(@"\");
                if (lastSlash < 0) { lastSlash = 0; }
                if (Files[i].EndsWith("\\Web.config"))
                {
                    Files[i] = "";
                }
                else
                {
                    Files[i] = Files[i].Substring(lastSlash);
                }
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}