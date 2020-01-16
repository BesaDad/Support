using System.Data.Entity.Migrations;
using Support.Domain;

namespace Support
{
    public sealed class Configuration : DbMigrationsConfiguration<SupportContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}