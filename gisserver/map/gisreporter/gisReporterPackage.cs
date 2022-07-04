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
                if ((!p.Files[i].EndsWith("package.json")) && (!Path.GetFileName(p.Files[i]).StartsWith("gisreporter_updater")))
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
                string filename = Path.GetFileName(Files[i]);
                if ((filename == "Web.config") || (filename.StartsWith("gisreporter_updater")))
                {
                    Files[i] = "";
                }
                else
                {
                    Files[i] = filename;
                }
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}