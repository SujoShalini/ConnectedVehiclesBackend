namespace ConnectedVehicles.Models
{
    public class VehicleData
    {
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public List<Vehicle> Vehicles { get; set; }
    }

    public class Vehicle
    {
        public string VehicleId { get; set; }
        public string RegistrationNumber { get; set; }
    }
}
