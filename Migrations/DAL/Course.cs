﻿namespace AlgoBotBackend.Migrations.DAL
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AdvertisingСampaign> Сampaigns { get; set; }
    }
}