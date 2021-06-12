using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TransplantingJsonConverter.Interfaces;

namespace TransplantingJsonConverter.Internal
{
    /// <inheritdoc/>
    public class TransplantDefinitionFactory : ITransplantDefinitionFactory
    {
        private readonly IEqualityComparer<string> _stringComparer;

        /// <summary>
        /// construct an instance of transplant definition factory
        /// </summary>
        /// <param name="stringComparer">
        /// comparer to use in various key comparison operations
        /// </param>
        public TransplantDefinitionFactory(IEqualityComparer<string> stringComparer)
        {
            _stringComparer = stringComparer ?? throw new ArgumentNullException(nameof(stringComparer));
        }

        /// <summary>
        /// construct an instance of transplant definition factory
        /// </summary>
        /// <param name="stringComparerName">
        /// name of a string comparer that exists as a static property on StringComparer
        /// </param>
        public TransplantDefinitionFactory(string stringComparerName)
        {
            if (string.IsNullOrWhiteSpace(stringComparerName))
            {
                throw new ArgumentException($"'{nameof(stringComparerName)}' cannot be null or whitespace.", nameof(stringComparerName));
            }

            var prop = typeof(StringComparer).GetProperty(stringComparerName, BindingFlags.Static | BindingFlags.Public);
            _stringComparer = (prop.GetValue(null) as IEqualityComparer<string>)!;
            if (_stringComparer is null)
            {
                throw new ArgumentOutOfRangeException(nameof(stringComparerName));
            }
        }

        /// <inheritdoc/>
        public bool CanConvert(Type potentialContainerType)
        {
            if (potentialContainerType is null)
            {
                throw new ArgumentNullException(nameof(potentialContainerType));
            }

            return potentialContainerType.GetCustomAttribute<JsonTransplantContainerAttribute>() is not null;
        }

        /// <inheritdoc/>
        public ITransplantDefinition BuildTransplantDefinition(Type containerType)
        {
            if (containerType is null)
            {
                throw new ArgumentNullException(nameof(containerType));
            }
            if (!CanConvert(containerType))
            {
                throw new ArgumentOutOfRangeException(nameof(containerType));
            }

            var qpis = from pi in containerType.GetProperties()
                       let attr = pi.GetCustomAttribute<JsonTransplantRootAttribute>()
                       select new { isroot = attr is not null, pi };

            var rootpis = qpis.Where(x => x.isroot).Select(x => x.pi).Take(2).ToList();
            if (rootpis.Count > 1)
            {
                throw new JsonTransplantException($"container type must not have more than one JsonTransplantRoot property: {containerType.FullName}");
            }
            else if (rootpis.Count < 1)
            {
                throw new JsonTransplantException($"container type must have exactly one JsonTransplantRoot property: {containerType.FullName}");
            }

            var rootPropertyInfo = rootpis[0];
            var otherPropertyInfos = qpis.Where(x => !x.isroot).Select(x => x.pi).ToList();

            var markedConstructors = containerType.GetConstructors().Where(x => x.GetCustomAttribute<JsonConstructorAttribute>() is not null).Take(2).ToList();
            if (markedConstructors.Count > 1)
            {
                throw new JsonTransplantException($"container type must not have more than one JsonConstructor: {containerType.FullName}");
            }

            return new TransplantDefinition(containerType, rootPropertyInfo, otherPropertyInfos.AsReadOnly(), markedConstructors.FirstOrDefault(), _stringComparer);
        }
    }
}
