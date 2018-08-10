using AutoMapper;
using Books.Domain.Models;
using Books.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Books.Web.Utility
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
            Mapper.CreateMap<Book, BookViewModel>()
                .ForMember(x => x.Image, opt => opt.Ignore()); 
        }
    }
}