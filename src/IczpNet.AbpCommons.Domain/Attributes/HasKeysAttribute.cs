using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Volo.Abp;

namespace IczpNet.AbpCommons.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HasKeysAttribute : Attribute

    {
        private bool? _isUnique;

        private string _name;

        //
        // 摘要:
        //     The properties which constitute the index, in order.
        public IReadOnlyList<string> PropertyNames { get; }

        //
        // 摘要:
        //     The name of the index.
        public string Name
        {
            get
            {
                return _name;
            }
            [param: DisallowNull]
            set
            {
                _name = Check.NotNull(value, "value");
            }
        }

        /// <summary>
        /// Whether the index is unique.
        /// </summary>
        public bool IsUnique
        {
            get
            {
                return _isUnique.GetValueOrDefault();
            }
            set
            {
                _isUnique = value;
            }
        }

        /// <summary>
        /// Checks whether IsUnique has been
        /// explicitly set to a value.
        /// </summary>
        public bool IsUniqueHasValue => _isUnique.HasValue;

        /// <summary>
        /// Initializes a new instance of the IndexAttribute
        /// </summary>
        /// <param name="propertyNames">The properties which constitute the index, in order(there must be at least one).</param>
        public HasKeysAttribute(params string[] propertyNames)
        {
            Assert.NotNull(propertyNames, null, "propertyNames");
            PropertyNames = propertyNames.ToList();
        }
    }
}
