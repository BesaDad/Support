using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using Support.BusinessLayer;

namespace Support.App_Start
{
    public sealed class Configuration : DbMigrationsConfiguration<SupportContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}