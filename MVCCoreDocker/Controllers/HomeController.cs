using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MVCCoreDocker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MVCCoreDocker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            string connetionString = null;
            SqlConnection cnn;
#if DEBUG
            string connectionString1 = "Data Source=davelap;Initial Catalog=ContosoUniversity1XC;User ID=sa;Password=tvxs721#3";
            cnn = new SqlConnection(connectionString1);
#else
            string connectionString = "Data Source=davelapsqlserver;Initial Catalog=ContosoUniversity1XC;User ID=sa;Password=tvxs721#3";
            cnn = new SqlConnection(connectionString);
#endif
            
            List<Student153> li = new List<Student153>();

            try
            {
                SqlCommand command = new SqlCommand(
                     "SELECT * FROM Student;",
                     cnn);
                _logger.LogInformation("be4 the open call to 153");
                   cnn.Open();
                _logger.LogInformation("after the call to the open 153");

                SqlDataReader reader = command.ExecuteReader();
                var data = reader.Read();
                
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Student153 st = new Student153();
                        var ty = reader.GetInt32(0) + " " + reader.GetString(1) + reader.GetString(2);
                        st.ID = reader.GetInt32(0);
                        st.FirstMidName = reader.GetString(1);
                        st.LastName = reader.GetString(2);
                        li.Add(st);
                        _logger.LogInformation(ty);
                    }
                }
                else
                {
                   // Console.WriteLine("No rows found.");
                }
                reader.Close();

            } catch(Exception ex)
            { 
            
            }

            /*
            try
            {
          
                cnn.Open();
                 
                // MessageBox.Show("Connection Open ! ");
                cnn.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Can not open connection ! ");
            } */


            ViewData["data"] = li;
            return View();
        }


        public IActionResult About()
        {
            string connectionString = "";
#if DEBUG
            System.Net.IPAddress[] ipAddresses = Dns.GetHostAddresses("localhost");
#else
            System.Net.IPAddress[] ipAddresses = Dns.GetHostAddresses("headless-service.default.svc.cluster.local");
#endif

            if (ipAddresses.Length == 0)
                _logger.LogInformation("this addresses r 0");
            else
                _logger.LogInformation("the IP addresses are  " + ipAddresses.Length.ToString());

            foreach (IPAddress ip in ipAddresses)
            {
                _logger.LogInformation("this is one IP: " + ip.ToString());
                if (String.IsNullOrEmpty(connectionString))
                    connectionString = "mongodb://";
                else
                    connectionString += ",";
                connectionString += $"{ip.ToString()}:27017";
            }
            connectionString += "/database";
            //var client = new MongoClient(connectionString);
            
            

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



/*
 *   using (var client = new System.Net.Http.HttpClient())
            {
                // Call *mywebapi*, and display its response in the page
                var request = new System.Net.Http.HttpRequestMessage();
                //request.RequestUri = new Uri("http://webapi2/WeatherForecast"); // ASP.NET 3 (VS 2019 only)
                request.RequestUri = new Uri("http://webapi2/api/Student"); // ASP.NET 3 (VS 2019 only)
                //request.RequestUri = new Uri("http://mywebapi/api/values/1"); // ASP.NET 2.x
                var response = await client.SendAsync(request);
                ViewData["Message"] += " and " + await response.Content.ReadAsStringAsync();
            }
 */