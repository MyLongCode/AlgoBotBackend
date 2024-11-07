namespace AlgoBotBackend.Models.ViewModels
{
    public class AddPaymentViewModel
    {
        public int UserIdInAlgo { get; set; } 
        public int Amount { get; set; }
        public int CourseIdInAlgo { get; set; }
        public DateTime DateTime { get; set; }
    }
}
