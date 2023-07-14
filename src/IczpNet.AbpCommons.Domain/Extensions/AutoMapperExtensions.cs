using AutoMapper;
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
        public static void UseStaticAutoMapper(this IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) => Mapper.Map(source, destination);

        public static TDestination MapTo<TDestination>(this object source) => Mapper.Map<TDestination>(source);

        public static object MapTo(this object source, Type sourceType, Type destinationType) => Mapper.Map(source, sourceType, destinationType);
    }
}
