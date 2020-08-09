using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StackExchange.Redis;

namespace WebAppTwo.Controllers
{
    public class HomeController : Controller
    {
        public static string CacheConnection = "Redis cache connection string";
        public static string connectionstring;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Display Users in the DB";

            connectionstring = "Server=tcp:se010server.database.windows.net,1433;Initial Catalog=se010db;Persist Security Info=False;User ID=se010serverlogin;Password=******;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var conn = new SqlConnection(connectionstring);
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Users", conn);

            conn.Open();
            Int32 rowCount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();

            ViewBag.MessageA = "Number of entries : " +rowCount;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Working with Redis";
            connectionstring = "Server=tcp:se010server.database.windows.net,1433;Initial Catalog=se010db;Persist Security Info=False;User ID=se010serverlogin;Password=******;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var conn = new SqlConnection(connectionstring);
            var cmd = new SqlCommand("SELECT COUNT(*) FROM Users", conn);

            conn.Open();
            Int32 rowCount = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            conn.Close();
            IDatabase cache = lazyConnection.Value.GetDatabase();
            ViewBag.MessageOne = "Reading Cache : " + cache.StringGet("UserCountS#").ToString();
            ViewBag.MessageTwo = "Writing Cache : " + cache.StringSet("UserCountS#","User Count is "+rowCount.ToString()+" @ "+ DateTime.Now.ToShortTimeString()).ToString();
            ViewBag.MessageThree = "Reading Cache : " + cache.StringGet("UserCountS#").ToString();
            cache.KeyExpire("UserCount", DateTime.Now.AddMinutes(1));
            lazyConnection.Value.Dispose();

            return View();
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
            return ConnectionMultiplexer.Connect(CacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

    }
}