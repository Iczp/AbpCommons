using JetBrains.Annotations;
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
            [InvokerParameterName][NotNull] string message)
        {
            if (value)
            {
                throw new UserFriendlyException(message);
            }
        }

        [ContractAnnotation("value:true => halt")]
        public static void If(
           bool value,
           [InvokerParameterName][NotNull] string message,
           [InvokerParameterName][NotNull] string parameterName)
        {
            if (value)
            {
                throw new UserFriendlyException(message, parameterName);
            }
        }

        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(
            T value,
            [InvokerParameterName][NotNull] string message)
        {
            if (value == null)
            {
                throw new UserFriendlyException(message);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(
            T value,
            [InvokerParameterName][NotNull] string message,
            [InvokerParameterName][NotNull] string parameterName)
        {
            if (value == null)
            {
                throw new UserFriendlyException(message, parameterName);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotNull(
            string value,
            [InvokerParameterName][NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0)
        {
            if (value == null)
            {
                throw new UserFriendlyException($"{parameterName} 不能为空!", parameterName);
            }

            if (value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 长度必须小于或等于 {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new UserFriendlyException($"{parameterName} 长度必须大于或等于 {minLength}!", parameterName);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrWhiteSpace(
            string value,
            [InvokerParameterName][NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new UserFriendlyException($"{parameterName} 字符不能为空!", parameterName);
            }

            if (value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须小于或等于 {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须大于或等于 {minLength}!", parameterName);
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrEmpty(
            string value,
            [InvokerParameterName][NotNull] string parameterName,
            int maxLength = int.MaxValue,
            int minLength = 0)
        {
            if (value.IsNullOrEmpty())
            {
                throw new UserFriendlyException($"{parameterName} 字符不能为空!", parameterName);
            }

            if (value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须小于或等于 {maxLength}!", parameterName);
            }

            if (minLength > 0 && value.Length < minLength)
            {
                throw new UserFriendlyException($"{parameterName} 字符长度必须大于或等于 {minLength}!", parameterName);
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
            int minLength = 0)
        {
            if (minLength > 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new UserFriendlyException(parameterName + " 为能为null或为空!", parameterName);
                }

                if (value.Length < minLength)
                {
                    throw new UserFriendlyException($"{parameterName} 长度必须大于或等于 {minLength}!", parameterName);
                }
            }

            if (value != null && value.Length > maxLength)
            {
                throw new UserFriendlyException($"{parameterName} 长度必须小于或等于 {maxLength}!", parameterName);
            }

            return value;
        }
    }
}
