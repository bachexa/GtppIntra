namespace Envirovement.Models
{
    public class EnvirovementSentReport
    {
        public int ID { get; set; }
        public string? TurbinaName { get; set; }
        public double NoxValue { get; set; }
        public double CoValue { get; set; }
        public DateTime RecordDate { get; set; }
        public bool CheckConnection { get; set; }

        public bool CheckLanConnection { get; set; }

        public List<EnvrTHertyMinuteModel> thertyminutemodel;


        public EnvirovementSentReport() 
        {
            thertyminutemodel = new List<EnvrTHertyMinuteModel>();
        }
    }
}
