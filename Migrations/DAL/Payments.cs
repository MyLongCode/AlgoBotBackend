namespace AlgoBotBackend.Migrations.DAL
{
    public class Payments
    {
        public int Id { get; set; }
        public int UserIdInAlgo { get; set; }
        public int Amount { get; set; }
        public int CourseIdInAlgo { get; set; }
        public DateTime DateTime { get; set; }
    }
}
