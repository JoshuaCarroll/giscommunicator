using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using gisreporter_;
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
                    string strSql = "exec dbo.spCreateMapItem @DataSet, @UID, @Latitude, @Longitude, @LocationDescription, @Name, @Description, @Icon, @ReportedDateTime, @Recipient;";

                    foreach (MapItem mapItem in mapItems)
                    {
                        if (mapItem.LocationLatitude != null && mapItem.LocationLatitude != "" && mapItem.LocationLongitude != null && mapItem.LocationLongitude != "")
                        {
                            SqlCommand cmd = new SqlCommand(strSql, Connection);
                            CreateMapItemRecord(cmd, "1", mapItem.UniqueID, mapItem.LocationLatitude, mapItem.LocationLongitude, mapItem.LocationDescription, mapItem.Name, mapItem.Description, mapItem.Icon, mapItem.ReportedDateTime, mapItem.Recipient);
                        }
                    }
                    Connection.Close();
                }

                log.LogInformation(string.Format("Added {0} records.", mapItems.Length));

                return (ActionResult)new OkObjectResult("Ok");
            }
            else if (req.Query["recipient"] != "")
            {
                try
                {
                    string recipient = req.Query["recipient"];
                    log.LogInformation("recipient: " + recipient);
                    string kml = "";
                    using (SqlConnection Connection = new SqlConnection(connectionString))
                    {
                        string strSql = "exec dbo.spGetMapItems @recipient;";
                        SqlCommand cmd = new SqlCommand(strSql, Connection);
                        cmd.Parameters.AddWithValue("@recipient", recipient);
                        Connection.Open();
                        kml = cmd.ExecuteScalar().ToString();
                        Connection.Close();
                    }
                    return (ActionResult)new OkObjectResult(kml);
                }
                catch (Exception ex)
                {
                    return (ActionResult)new OkObjectResult(ex.Message);
                }
            }
            else
            {
                return (ActionResult)new OkObjectResult("No recipient specified");
            }
        }

        public static void CreateMapItemRecord(SqlCommand cmd, string DataSet, string UID, string Latitude, string Longitude, string LocationDescription, string Name, string Description, string Icon, string ReportedDateTime, string Recipient)
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
            cmd.Parameters.AddWithValue("@Recipient", Recipient);
            cmd.ExecuteNonQuery();
        }
    }
}

