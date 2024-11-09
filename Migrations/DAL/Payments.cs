namespace AlgoBotBackend.Migrations.DAL
{
    public class Payment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Amount { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime DateTime { get; set; }
    }
}
