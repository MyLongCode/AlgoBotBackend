using AlgoBotBackend.Migrations.DAL;

namespace AlgoBotBackend.Models.ViewModels
{
    public class BotUserViewModel
    {
        public string Username { get; set; }
        public string? ReferalUsername { get; set; }
        public string Firstname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ChildAge { get; set; } = null!;
        public string ChildName { get; set; } = null!;
        public int Score { get; set; }
        public int Сashback { get; set; }
        public List<User>? Referals1 { get; set; }
        public List<User>? Referals2 { get; set; }
        public List<User>? Referals3 { get; set; }
    }
}
