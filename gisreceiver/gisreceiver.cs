using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using gisreporter;
using System.Data.SqlClient;

namespace gisreceiver
{
    public static class gisreceiver
    {
        [FunctionName("gisreceiver")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("sqldb_connstr");

            var json = await new StreamReader(req.Body).ReadToEndAsync();

            if (json != String.Empty)
            {

                MapItem[] mapItems = JsonConvert.DeserializeObject<MapItem[]>(json);

                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                    Connection.Open();
                    string strSql = "exec dbo.spCreateMapItem @DataSet, @UID, @Latitude, @Longitude, @LocationDescription, @Name, @Description, @Icon, @ReportedDateTime;";

                    foreach (gisreporter.MapItem mapItem in mapItems)
                    {
                        if (mapItem.LocationLatitude != null && mapItem.LocationLatitude != "" && mapItem.LocationLongitude != null && mapItem.LocationLongitude != "")
                        {
                            SqlCommand cmd = new SqlCommand(strSql, Connection);
                            CreateMapItemRecord(cmd, "1", mapItem.UniqueID, mapItem.LocationLatitude, mapItem.LocationLongitude, mapItem.LocationDescription, mapItem.Name, mapItem.Description, mapItem.Icon, mapItem.ReportedDateTime);
                        }
                    }
                    Connection.Close();
                }

                log.LogInformation(string.Format("Added {0} records.", mapItems.Length));

                return (ActionResult)new OkObjectResult("Ok");
            }
            else
            {
                string kml = "";
                using (SqlConnection Connection = new SqlConnection(connectionString))
                {
                    Connection.Open();
                    string strSql = "exec dbo.genBasicKML;";

                    SqlCommand cmd = new SqlCommand(strSql, Connection);
                    kml = cmd.ExecuteScalar().ToString();

                    Connection.Close();

                }
                return (ActionResult)new OkObjectResult(kml);
            }
        }

        public static void CreateMapItemRecord(SqlCommand cmd, string DataSet, string UID, string Latitude, string Longitude, string LocationDescription, string Name, string Description, string Icon, string ReportedDateTime)
        {
            cmd.Parameters.AddWithValue("@DataSet", DataSet);
            cmd.Parameters.AddWithValue("@UID", UID);
            cmd.Parameters.AddWithValue("@Latitude", Latitude);
            cmd.Parameters.AddWithValue("@Longitude", Longitude);
            cmd.Parameters.AddWithValue("@LocationDescription", LocationDescription);
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Description", Description);
            cmd.Parameters.AddWithValue("@Icon", Icon);
            cmd.Parameters.AddWithValue("@ReportedDateTime", ReportedDateTime);
            cmd.ExecuteNonQuery();
        }
    }
}

