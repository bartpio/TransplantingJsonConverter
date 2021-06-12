using System;

namespace TransplantingJsonConverter
{
    /// <summary>
    /// marks a class type as being the outer container that participates in JSON transplanting
    /// a container class (marked with this attribute) must have exactly one public property marked with <see cref="JsonTransplantRootAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class JsonTransplantContainerAttribute : Attribute
    {
    }
}
