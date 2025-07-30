namespace Envirovement.Models
{
    public class GetDataByDate
    {
        public Guid? AddGUID { get; set; }
        public Guid? BuildingGUID { get; set; }
        public Guid? CompanyGUID { get; set; }
        public string? Data { get; set; }
        public string? DataDateTime { get; set; }
        public long WsLogID { get; set; }
        public int StatusId { get; set; }
    }
}
