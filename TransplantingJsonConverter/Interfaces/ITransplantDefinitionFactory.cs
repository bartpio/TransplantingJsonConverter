using System;
using System.Collections.Generic;

namespace TransplantingJsonConverter.Interfaces
{
    /// <summary>
    /// interface to a factory that provides instances implementing <see cref="ITransplantDefinition"/>
    /// </summary>
    public interface ITransplantDefinitionFactory
    {
        /// <summary>
        /// build a transplant definition for a particular container type
        /// </summary>
        /// <param name="containerType">
        /// a particular container type
        /// </param>
        /// <returns>
        /// transplant definition for a particular container type
        /// </returns>
        ITransplantDefinition BuildTransplantDefinition(Type containerType);

        /// <summary>
        /// check if the specified type is a valid container type
        /// it should be marked with <see cref="JsonTransplantContainerAttribute"/>, and generally also [JsonConverter(typeof(JsonTransplantConverter))]
        /// </summary>
        /// <param name="potentialContainerType">
        /// a dot net type
        /// </param>
        /// <returns>
        /// true if the specified type is a container type valid for use with transplanting
        /// </returns>
        bool CanConvert(Type potentialContainerType);
    }
}