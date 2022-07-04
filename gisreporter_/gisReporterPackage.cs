using System.Text.Json;
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

        public gisReporterPackage()
        {
            folder = "";
        }

        public static gisReporterPackage Create(string folder)
        {
            gisReporterPackage p = new gisReporterPackage();
            p.PublishDateUTC = DateTime.UtcNow.Subtract(new TimeSpan(36500, 0, 0, 0));
            p.Files = Directory.GetFiles(folder);
            p.folder = folder;

            for (int i = 0; i < p.Files.Length; i++)
            {
                DateTime lastModified = System.IO.File.GetLastWriteTimeUtc(p.Files[i]);
                if (lastModified > p.PublishDateUTC)
                {
                    p.PublishDateUTC = lastModified;
                }
            }
            return p;
        }

        public override string ToString()
        {
            for (int i = 0; i < Files.Length; i++)
            {
                Files[i] = "release" + Files[i].Substring(folder.Length);
            }
            return JsonSerializer.Serialize(this);
        }
    }
}