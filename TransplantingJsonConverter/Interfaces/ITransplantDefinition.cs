using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace TransplantingJsonConverter.Interfaces
{
    /// <summary>
    /// transplant definition interface
    /// contains information needed for the transplant converter to work, for a particular container type
    /// </summary>
    public interface ITransplantDefinition
    {
        /// <summary>
        /// container type (the outermost object)
        /// </summary>
        Type ContainerType { get; }

        /// <summary>
        /// root property info
        /// </summary>
        PropertyInfo RootPropertyInfo { get; }

        /// <summary>
        /// mapping of nonroot property names to their infos
        /// </summary>
        ReadOnlyDictionary<string, PropertyInfo> OtherMap { get; }

        /// <summary>
        /// optional "marked constructor" (marked with newtonsoft's JsonConstructor attribute), useful for immutable objects
        /// </summary>
        ConstructorInfo? MarkedConstructor { get; }

        /// <summary>
        /// comparer to use for key comparisons
        /// </summary>
        IEqualityComparer<string> StringComparer { get; }
    }
}