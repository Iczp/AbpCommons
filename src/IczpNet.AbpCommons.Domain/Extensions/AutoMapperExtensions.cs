using AutoMapper;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.AutoMapper;

namespace IczpNet.AbpCommons.Extensions
{
    /// <summary>
    /// AutoMapperExtensions
    /// </summary>
    public static class AutoMapperExtensions
    {
        private static IServiceProvider ServiceProvider;
        private static IMapper _mapper;

        //private static IObjectMapper _objectMapper;
        //IMapperAccessor
        private static IMapper Mapper => _mapper ??= ServiceProvider.GetRequiredService<IMapperAccessor>().Mapper;
        //private static IMapper Mapper => _mapper ??= ServiceProvider.GetRequiredService<IMapper>();
        //private static IObjectMapper ObjectMapper => _objectMapper ??= ServiceProvider.GetRequiredService<IObjectMapper>();
        public static void UseStaticAutoMapper(this IApplicationBuilder applicationBuilder)
        {
            ServiceProvider = applicationBuilder.ApplicationServices;
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) => Mapper.Map<TSource, TDestination>(source, destination);

        public static TDestination MapTo<TDestination>(this object source) => Mapper.Map<TDestination>(source);
    }
}
