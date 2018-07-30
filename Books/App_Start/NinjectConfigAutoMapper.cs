using Books.AutoMapperTypeConfig;
using Books.Infrastructure.AutoMappingConfig;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Books.App_Start
{
    internal static class NinjectConfigAutoMapper
    {
        public static void ConfigureAutoMapper(IKernel kernel)
        {
            kernel.Bind<IAutoMapperTypeConfigurator>()
                    .To<AuthorToAuthorViewModelMapper>().InSingletonScope();

            kernel.Bind<IAutoMapperTypeConfigurator>()
                    .To<AuthorViewModelToAuthorMapper>().InSingletonScope();

            kernel.Bind<IAutoMapperTypeConfigurator>()
                    .To<BookToBookViewModelMapper>().InSingletonScope();

            kernel.Bind<IAutoMapperTypeConfigurator>()
                    .To<BookViewModelToBookMapper>().InSingletonScope();
        }
    }
}