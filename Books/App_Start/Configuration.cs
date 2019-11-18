using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using Tele.BusinessLayer;

namespace Books.App_Start
{
    public sealed class Configuration : DbMigrationsConfiguration<TeleContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
    }
}