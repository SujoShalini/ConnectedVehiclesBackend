using ConnectedVehicles.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConnectedVehicles.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly VehiceStatusService _vehiceStatusService;
        public VehicleController(VehiceStatusService vehiceStatusService)
        {
            _vehiceStatusService = vehiceStatusService;

        }

        [HttpGet("data")]
        public IActionResult GetVehiclesData()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ConnectedVehicles.json");

            // Intergrate with Azure Storgar Container as the Vehicle Data will be having meta data. For test scenario I have read it from external source.

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("JSON file not found.");
            }

            var jsonData = System.IO.File.ReadAllText(filePath);
            return Ok(jsonData);
        }

        [HttpGet("streaming/VehicleStatus")]
        public async Task<IActionResult> GetStatus()
        {
            var message = await _vehiceStatusService.ReceiveMessagesAsync();
            if (message != null)
            {
                return Ok(message);
            }

            return NoContent();
        }


        [HttpPost("data")]
        public async Task<IActionResult> PostVehicleData([FromBody] VehicleStatus vehicleData)
        {
            if (vehicleData == null)
            {
                return BadRequest();
            }

            await _vehiceStatusService.SendMessageAsync(vehicleData);
            return Ok();
        }

        [HttpGet("data/company/{companyName}")]
        public IActionResult GetVehiclesByCompanyName(string companyName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ConnectedVehicles.json");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("JSON file not found.");
            }

            var jsonData = System.IO.File.ReadAllText(filePath);
            var companies = JsonConvert.DeserializeObject<List<VehicleData>>(jsonData);
            var company = companies.FirstOrDefault(c => c.CompanyName.Contains(companyName, StringComparison.OrdinalIgnoreCase));

            if (company == null)
            {
                return NotFound("Company not found.");
            }

            return Ok(company);
        }

        [HttpGet("data/status/{status}")]
        public IActionResult GetVehiclesByStatus(string status)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "ConnectedVehicles.json");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("JSON file not found.");
            }

            var jsonData = System.IO.File.ReadAllText(filePath);
            var companies = JsonConvert.DeserializeObject<List<VehicleData>>(jsonData);
            var statusFilePath = Path.Combine(Directory.GetCurrentDirectory(), "VehicleStatus.json");
            var statusData = System.IO.File.ReadAllText(statusFilePath);
            var vehicleStatuses = JsonConvert.DeserializeObject<List<VehicleStatus>>(statusData);

            var filteredVehicles = companies.SelectMany(c => c.Vehicles)
                                            .Where(v => vehicleStatuses.Any(vs => vs.VehicleId == v.VehicleId && vs.StatusMessage.Equals(status, StringComparison.OrdinalIgnoreCase)))
                                            .ToList();

            if (!filteredVehicles.Any())
            {
                return NotFound("No vehicles found with the given status.");
            }

            return Ok(filteredVehicles);
        }



    }
}
