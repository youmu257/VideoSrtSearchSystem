using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Diagnostics;
using VideoSrtSearchSystem.Models;
using VideoSrtSearchSystem.Models.LiveStraming;

namespace VideoSrtSearchSystem.Controllers
{
    //[EnableCors("AllOpen")]
    [ApiController]
    [Route("home")]
    public class HomeController(IConfiguration _configuration, ILogger<HomeController> _logger) : Controller
    {
        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            List<LiveStreamingModel> list = new List<LiveStreamingModel>();
            using (MySqlConnection connection = new MySqlConnection(_configuration["ConnectionStrings:localhost"]))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection successful!");

                    string query = $"SELECT * FROM {LiveStreamingModel.TableName}";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new LiveStreamingModel(reader));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            _logger.LogInformation(_configuration["aaa"]);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
