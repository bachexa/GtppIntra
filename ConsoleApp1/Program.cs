// See https://aka.ms/new-console-template for more information
using ConsoleApp1;
using System.Globalization;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;


DateTime originalUtcDateTime = DateTime.UtcNow;
TimeZoneInfo getTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Georgian Standard Time");
DateTime getDateTime = TimeZoneInfo.ConvertTimeFromUtc(originalUtcDateTime, getTimeZone);
string dateInput = getDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
var parsedDate = DateTime.Parse(dateInput);

//Console.OutputEncoding = Encoding.UTF8;
//DateTime originalUtcDateTime = DateTime.UtcNow; // Get current UTC time directly
//string formattedDateTime = originalUtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
//var parsedDate = DateTime.Parse(formattedDateTime);



swaggerClient swagger = new swaggerClient(new HttpClient());

Guid CompanyGUID = new Guid("00471ea0-3f5e-44c8-b998-c82260971a9b");
Guid BuildingGUID = new Guid("353e6580-a1ba-4ad5-ab3e-2c5ec47203e9");
Guid MonitoringPointGUID = new Guid("fb940c8e-7649-41a3-9205-0f42d1cc5981");
Guid SensorGUID = new Guid("60953271-5307-4179-b798-b74806707120");



var a =await swagger.GetDataByDateAsync(CompanyGUID, BuildingGUID, MonitoringPointGUID, SensorGUID, parsedDate);




var requestData = new List<Request>();
requestData.Add(new Request
{
    CompanyGUID = new Guid("00471ea0-3f5e-44c8-b998-c82260971a9b"),
    BuildingGUID = new Guid("353e6580-a1ba-4ad5-ab3e-2c5ec47203e9"),
    MonitoringPointGUID = new Guid("fb940c8e-7649-41a3-9205-0f42d1cc5981"),
    SensorGUID = new Guid("60953271-5307-4179-b798-b74806707120"),
    Data = 14.5,
    DataDateTime = DateTime.Now // Assuming parsedDate is defined elsewhere
});


var b = await swagger.AddBulkDataAsync(requestData);


//var c = await swagger.GetDataByDateAsync(CompanyGUID, BuildingGUID, MonitoringPointGUID, SensorGUID, parsedDate);

//var a = await swagger.AddDataAsync(CompanyGUID, BuildingGUID, MonitoringPointGUID, SensorGUID, 0.2, parsedDate);


// var a = await c.AddDataAsync(CompanyGUID, BuildingGUID, MonitoringPointGUID, SensorGUID, 2, parsedDate);

//var data = 2;
//List<Request> AddData = new List<Request>();

//string Date = "03/18/24";
//DateTimeOffset? dataDateTime = DateTimeOffset.ParseExact(Date, "MM/dd/yy", CultureInfo.InvariantCulture);



//Request newRequest = new Request();
//newRequest.DataDateTime = dataDateTime;
//newRequest.Data = data;
//newRequest.BuildingGUID = BuildingGUID;
//newRequest.MonitoringPointGUID = MonitoringPointGUID;
//newRequest.SensorGUID = SensorGUID;
//newRequest.CompanyGUID = CompanyGUID;

//AddData.Add(newRequest);
//AddData.Add(newRequest);
//AddData.Add(newRequest);

//var a = await swagger.AddBulkDataAsync(AddData);




//foreach (var item in a)
//{
//    Console.WriteLine(item.Status);
//}

Console.WriteLine("მუშაობს");




public class MyCustomRequest
{
    public string? companyGUID { get; set; }
    public string? buildingGUID { get; set; }
    public string? monitoringPointGUID { get; set; }
    public string? sensorGUID { get; set; }
    public int data { get; set; }
    public DateTime dataDateTime { get; set; }
}