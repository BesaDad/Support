using AutoMapper;
using Books.Domain.Models;
using Books.Infrastructure.AutoMappingConfig;
using Books.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Books.AutoMapperTypeConfig
{
    public class AuthorToAuthorViewModelMapper : IAutoMapperTypeConfigurator
    {
        public void Configure()
        {
            Mapper.CreateMap<Author, AuthorViewModel>();            
        }
    }
}