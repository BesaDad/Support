using AutoMapper;
using Tele.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tele.Web.Utility
{
    public class AutoMapperBootStrapper
    {
        public static void Configure(IEnumerable<Infrastructure.AutoMappingConfig.IAutoMapperTypeConfigurator> autoMapperTypeConfigurators)
        {
            autoMapperTypeConfigurators.ToList().ForEach(x => x.Configure());

            Mapper.AssertConfigurationIsValid();
        }

        public static void BootStrap()
        {

        }
    }
}