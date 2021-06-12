using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using TransplantingJsonConverter.Interfaces;

namespace TransplantingJsonConverter.Internal
{
    /// <summary>
    /// object remapper
    /// deals with shifting around properties on intermediate <see cref="JObject"/> forms during serialization or deserialization with transplantation engaged
    /// </summary>
    internal sealed class ObjectRemapper
    {
        private readonly ITransplantDefinition _td;

        /// <summary>
        /// construct, given definition
        /// </summary>
        /// <param name="td">a transplant definition</param>
        public ObjectRemapper(ITransplantDefinition td)
        {
            _td = td ?? throw new ArgumentNullException(nameof(td));
        }

        /// <summary>
        /// remove a property from its parent, and return that same property for chaining convenience
        /// </summary>
        /// <param name="jprop">
        /// a JSON property
        /// SIDE EFFECT: property is removed from its parent
        /// </param>
        /// <returns>
        /// the same JSON property reference that was passed in
        /// </returns>
        private static JProperty Peel(JProperty jprop)
        {
            jprop.Remove();
            return jprop;
        }

        /// <summary>
        /// peel root for deserialize operation
        /// </summary>
        /// <param name="rawjob">
        /// the raw JSON object that was just read
        /// SIDE EFFECT: properties that belong on root-object will be removed from rawjob
        /// </param>
        /// <returns>
        /// root-object, with properties peeled from rawjob as appropriate
        /// </returns>
        public JObject PeelRootForDeserialize(JObject rawjob)
        {
            if (rawjob is null)
            {
                throw new ArgumentNullException(nameof(rawjob));
            }

            var notothers = rawjob.Properties().Where(x => !_td.OtherMap.ContainsKey(x.Name)).ToList().Select(x => Peel(x)).ToArray();
            return new JObject(notothers);
        }

        /// <summary>
        /// peel root for serialize operation
        /// </summary>
        /// <param name="rawjob">
        /// container represented as a plainly serialized JSON object, without any transformations applied yet
        /// SIDE EFFECT: root property will be removed from rawjob
        /// </param>
        /// <returns>
        /// root-value
        /// </returns>
        public JToken PeelRootForSerialize(JObject rawjob)
        {
            var rootName = _td.RootPropertyInfo.Name;
            var stringComparer = _td.StringComparer;
            var matchingprops = rawjob.Properties().Where(x => stringComparer.Equals(rootName, x.Name)).Take(2).ToList();
            if (matchingprops.Count > 1)
            {
                throw new JsonTransplantException($"duplicate occurence of rootnamed property '{rootName}'");
            }
            else if (matchingprops.Count < 1)
            {
                throw new JsonTransplantException($"rootnamed property '{rootName}' required");
            }

            return Peel(matchingprops[0]).Value;

        }

        

        /// <summary>
        /// comingle for serialize
        /// take properties from the root value, and add them to the peeled JSON object
        /// </summary>
        /// <param name="peeledjob">
        /// peeled JSON object
        /// SIDE EFFECT: will be augmented with properties from the root value
        /// </param>
        /// <param name="rootvalue">
        /// root-value
        /// </param>
        /// <param name="containerType">
        /// type of container object we're dealing with
        /// </param>
        /// <returns>
        /// final transformed and serialized form of the container object
        /// </returns>
        public JToken ComingleForSerialize(JObject peeledjob, JToken rootvalue, Type containerType)
        {
            if (TryComingleForSerialize(peeledjob, rootvalue, containerType))
            {
                return peeledjob;
            }
            else
            {
                return JToken.Parse("null");
            }
        }

        /// <summary>
        /// try-comingle for serialize
        /// see <see cref="ComingleForSerialize(JObject, JToken, Type)"/>
        /// </summary>
        /// <param name="peeledjob"></param>
        /// <param name="rootvalue"></param>
        /// <param name="containerType"></param>
        /// <returns>
        /// true if comingling was effect and resulted in not-null
        /// </returns>
        private bool TryComingleForSerialize(JObject peeledjob, JToken rootvalue, Type containerType)
        {
            switch (rootvalue.Type)
            {
                case JTokenType.Null:
                    return false;
                case JTokenType.Object:
                    var rootjob = (JObject)rootvalue;

                    foreach (var prop in rootjob.Properties().ToList().Select(x => Peel(x)).Reverse().ToList())
                    {
                        try
                        {
                            peeledjob.AddFirst(prop);
                        }
                        catch (ArgumentException arge) //when (arge.Message.EndsWith("Property with the same name already exists on object.", StringComparison.InvariantCulture))
                        {
                            throw new JsonTransplantException($"property '{prop.Name}' can't exist on both container type '{containerType.FullName}' and root type", arge);
                        }
                    }

                    return true;
                default:
                    throw new JsonTransplantException($"not expecting token type {rootvalue.Type} (while serializing container type '{containerType.FullName}')");
            }
        }
    }
}
