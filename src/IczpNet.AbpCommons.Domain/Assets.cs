using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Volo.Abp;

namespace IczpNet.AbpCommons
{
    [DebuggerStepThrough]
    public static class Assert
    {
        [ContractAnnotation("value:true => halt")]
        public static void If(
            bool value,
            [InvokerParameterName][NotNull] string message,
            string code = null,
            string details = null,
            Exception innerException = null,
            LogLevel logLevel = LogLevel.Warning)
        {
            if (value)
            {
                throw new UserFriendlyException(message, code, details, innerException, logLevel);
            }
        }


        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(
            T value,
            [InvokerParameterName][NotNull] string message, string code = null,
            string details = null,
            Exception innerException = null,
            LogLevel logLevel = LogLevel.Warning)
        {
            if (value == null)
            {
                throw new UserFriendlyException(message, code, details, innerException, logLevel);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotNull(
            string value,
            [InvokerParameterName][NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0,
            string details = null,
            Exception innerException = null,
            LogLevel logLevel = LogLevel.Warning)
        {
            if (value == null)
            {
                throw new UserFriendlyException($"{parameterName} 不能为空!", parameterName, details, innerException, logLevel);
            }

            if (value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 长度必须小于或等于 {maxLength}!", parameterName, details, innerException, logLevel);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new UserFriendlyException($"{parameterName} 长度必须大于或等于 {minLength}!", parameterName, details, innerException, logLevel);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrWhiteSpace(
            string value,
            [InvokerParameterName][NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0, string details = null,
            Exception innerException = null,
            LogLevel logLevel = LogLevel.Warning)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new UserFriendlyException($"{parameterName} 字符不能为空!", parameterName, details, innerException, logLevel);
            }

            if (value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须小于或等于 {maxLength}!", parameterName, details, innerException, logLevel);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须大于或等于 {minLength}!", parameterName, details, innerException, logLevel);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrEmpty(string value, [InvokerParameterName][NotNull] string parameterName, int maxLength = int.MaxValue, int minLength = 0, string details = null, Exception innerException = null, LogLevel logLevel = LogLevel.Warning)
        {
            if (value.IsNullOrEmpty())
            {
                throw new UserFriendlyException($"{parameterName} 字符不能为空!", parameterName, details, innerException, logLevel);
            }

            if (value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须小于或等于 {maxLength}!", parameterName, details, innerException, logLevel);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须大于或等于 {minLength}!", parameterName, details, innerException, logLevel);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, [InvokerParameterName][NotNull] string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new UserFriendlyException(parameterName + " 集合不能为null或为空!", parameterName);
            }

            return value;
        }

        [ContractAnnotation("type:null => halt")]
        public static Type AssignableTo<TBaseType>(
            Type type,
            [InvokerParameterName][NotNull] string parameterName)
        {
            NotNull(type, parameterName);

            if (!type.IsAssignableTo<TBaseType>())
            {
                throw new UserFriendlyException($"{parameterName} (type of {type.AssemblyQualifiedName}) should be assignable to the {typeof(TBaseType).GetFullNameWithAssemblyName()}!");
            }

            return type;
        }

        public static string Length(
            [CanBeNull] string value,
            [InvokerParameterName][NotNull] string parameterName,
            int maxLength,
            int minLength = 0, string details = null,
        Exception innerException = null,
        LogLevel logLevel = LogLevel.Warning)
        {
            if (minLength > 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new UserFriendlyException(parameterName + " 为能为null或为空!", parameterName, details, innerException, logLevel);
                }

                if (value.Length < minLength)
                {
                    throw new UserFriendlyException($"{parameterName} 长度必须大于或等于 {minLength}!", parameterName, details, innerException, logLevel);
                }
            }

            if (value != null && value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 长度必须小于或等于 {maxLength}!", parameterName, details, innerException, logLevel);
            }

            return value;
        }
    }
}
