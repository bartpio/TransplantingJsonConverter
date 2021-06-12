using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TransplantingJsonConverter.Interfaces;

namespace TransplantingJsonConverter.Internal
{
    /// <inheritdoc/>
    public sealed class TransplantDefinition : ITransplantDefinition
    {
        private readonly Type _containerType;
        private readonly PropertyInfo _rootPropertyInfo;
        private readonly IList<PropertyInfo> _otherPropertyInfos;
        private readonly ConstructorInfo? _markedConstructor;
        private readonly IEqualityComparer<string> _stringComparer;
        private readonly ReadOnlyDictionary<string, PropertyInfo> _otherMap;

        /// <summary>
        /// construct an instance of a Transplant Definition for a particular container type
        /// </summary>
        /// <param name="containerType">a particular container type</param>
        /// <param name="rootPropertyInfo">root property info</param>
        /// <param name="otherPropertyInfos">nonroot property infos</param>
        /// <param name="markedConstructor">optional "marked constructor" (marked with newtonsoft's JsonConstructor attribute), useful for immutable objects</param>
        /// <param name="stringComparer">comparer to use for key comparisons</param>
        public TransplantDefinition(Type containerType, PropertyInfo rootPropertyInfo, IList<PropertyInfo> otherPropertyInfos, ConstructorInfo? markedConstructor, IEqualityComparer<string> stringComparer)
        {
            _containerType = containerType ?? throw new ArgumentNullException(nameof(containerType));
            _rootPropertyInfo = rootPropertyInfo ?? throw new ArgumentNullException(nameof(rootPropertyInfo));
            _otherPropertyInfos = otherPropertyInfos ?? throw new ArgumentNullException(nameof(otherPropertyInfos));
            _markedConstructor = markedConstructor; // this one is optional.
            _stringComparer = stringComparer ?? throw new ArgumentNullException(nameof(stringComparer));
            _otherMap = new ReadOnlyDictionary<string, PropertyInfo>(_otherPropertyInfos.ToDictionary(x => x.Name, x => x, _stringComparer));
        }

        /// <inheritdoc/>
        public Type ContainerType => _containerType;

        /// <inheritdoc/>
        public PropertyInfo RootPropertyInfo => _rootPropertyInfo;

        /// <inheritdoc/>
        public ReadOnlyDictionary<string, PropertyInfo> OtherMap => _otherMap;

        /// <inheritdoc/>
        public ConstructorInfo? MarkedConstructor => _markedConstructor;

        /// <inheritdoc/>
        public IEqualityComparer<string> StringComparer => _stringComparer;
    }
}
