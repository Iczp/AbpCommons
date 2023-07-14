
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.Json;

namespace IczpNet.AbpCommons.Extensions
{
    /// <summary>
    /// JsonExtensions
    /// </summary>
    public static class JsonExtensions
    {
        private static IServiceProvider ServiceProvider;
        private static IJsonSerializer _jsonSerializer;
        private static IJsonSerializer JsonSerializer => _jsonSerializer ??= ServiceProvider.GetRequiredService<IJsonSerializer>();
        public static void UseStaticJsonSerializer(this IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public static T ToObject<T>(this string source) where T : class
        {
            return JsonSerializer.Deserialize<T>(source);
        }
        public static string ToJson(this object source, bool indented = false)
        {
            return JsonSerializer.Serialize(source, indented);
        }
    }
}
