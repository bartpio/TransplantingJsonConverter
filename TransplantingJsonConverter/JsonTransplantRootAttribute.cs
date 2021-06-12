using System;
using System.Collections.Generic;
using System.Text;

namespace TransplantingJsonConverter
{
    /// <summary>
    /// marks the property that will become the root of the serialized JSON form
    /// a container class (marked with <see cref="JsonTransplantContainerAttribute"/>) must have exactly one public property marked with this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonTransplantRootAttribute : Attribute
    {
    }
}
