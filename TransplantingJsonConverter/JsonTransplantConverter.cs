using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TransplantingJsonConverter.Interfaces;
using TransplantingJsonConverter.Internal;

namespace TransplantingJsonConverter
{
    /// <summary>
    /// transplanting JSON converter
    /// </summary>
    public sealed class JsonTransplantConverter : JsonConverter
    {
        private readonly ITransplantDefinitionFactory _tdf;

        /// <summary>
        /// construct, with default transplant definiton factory
        /// </summary>
        public JsonTransplantConverter()
        {
            _tdf = new TransplantDefinitionFactory(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// construct, with specified transplant definition factory
        /// </summary>
        /// <param name="tdf">
        /// transplant definition factory
        /// </param>
        public JsonTransplantConverter(ITransplantDefinitionFactory tdf)
        {
            _tdf = tdf ?? throw new ArgumentNullException(nameof(tdf));
        }

        /// <summary>
        /// construct, with specified transplant definition factory type
        /// </summary>
        /// <param name="transplantDefinitionFactoryType">
        /// a type (it must implement <see cref="ITransplantDefinitionFactory"/>)
        /// </param>
        public JsonTransplantConverter(Type transplantDefinitionFactoryType)
        {
            if (transplantDefinitionFactoryType is null)
            {
                throw new ArgumentNullException(nameof(transplantDefinitionFactoryType));
            }

            _tdf = (ITransplantDefinitionFactory)Activator.CreateInstance(transplantDefinitionFactoryType);
        }

        /// <summary>
        /// construct, with specified transplant definition factory type
        /// </summary>
        /// <param name="transplantDefinitionFactoryType">
        /// a type (it must implement <see cref="ITransplantDefinitionFactory"/>)
        /// </param>
        /// <param name="constructorArgs">
        /// constructor arguments for the factory
        /// </param>
        public JsonTransplantConverter(Type transplantDefinitionFactoryType, object[] constructorArgs)
        {
            if (transplantDefinitionFactoryType is null)
            {
                throw new ArgumentNullException(nameof(transplantDefinitionFactoryType));
            }

            _tdf = (ITransplantDefinitionFactory)Activator.CreateInstance(transplantDefinitionFactoryType, constructorArgs);
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => _tdf.CanConvert(objectType ?? throw new ArgumentNullException(nameof(objectType)));

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var td = _tdf.BuildTransplantDefinition(objectType);
            var remapper = new ObjectRemapper(td);

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    return null;
                case JsonToken.StartObject:
                    Dictionary<string, JToken> mapform;
                    JObject? rootjob;
                    {
                        if (!reader.Read())
                        {
                            throw new JsonTransplantException("could not read first property");
                        }

                        mapform = new Dictionary<string, JToken>(td.StringComparer);
                        var tempjob = new JObject();
                        while (reader.TokenType == JsonToken.PropertyName)
                        {
                            var jprop = (JProperty)JToken.ReadFrom(reader);
                            tempjob.Add(jprop);
                            mapform.Add(jprop.Name, jprop.Value);
                        }
                        rootjob = remapper.PeelRootForDeserialize(tempjob);
                    }

                    var rootform = rootjob.ToObject(td.RootPropertyInfo.PropertyType);
                    var othermap = td.OtherMap;

                    if (td.MarkedConstructor is null)
                    {
                        var result = Activator.CreateInstance(objectType);

                        void Transferrer(PropertyInfo otherpi, object obj)
                        {
                            otherpi.SetValue(result, obj);
                        }

                        td.RootPropertyInfo.SetValue(result, rootform);
                        TransferOthers(Transferrer, mapform, othermap);
                        return result;
                    }
                    else
                    {
                        var consparams = td.MarkedConstructor.GetParameters();
                        var parammap = consparams.ToDictionary(x => x.Name, x => x, td.StringComparer);
                        var ordinalmap = Enumerable.Range(0, consparams.Length).ToDictionary(x => consparams[x], x => x);
                        var cargs = new object[consparams.Length];

                        void Transferrer(PropertyInfo otherpi, object obj)
                        {
                            if (parammap.TryGetValue(otherpi.Name, out var parameterinfo))
                            {
                                cargs[ordinalmap[parameterinfo]] = obj;
                            }
                        }

                        cargs[ordinalmap[parammap[td.RootPropertyInfo.Name]]] = rootform;
                        TransferOthers(Transferrer, mapform, othermap);
                        return Activator.CreateInstance(objectType, cargs);
                    }

                default:
                    throw new JsonTransplantException($"did not expect token {reader.TokenType}");
            }
        }

        private static void TransferOthers(Action<PropertyInfo, object> transferrer, Dictionary<string, JToken> mapform, ReadOnlyDictionary<string, PropertyInfo> othermap)
        {
            foreach (var kvp in mapform)
            {
                if (othermap.TryGetValue(kvp.Key, out var otherpi))
                {
                    transferrer(otherpi, kvp.Value.ToObject(otherpi.PropertyType));
                }
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not null)
            {
                var valueType = value.GetType();
                var td = _tdf.BuildTransplantDefinition(valueType);
                var remapper = new ObjectRemapper(td);

                var pis = valueType.GetProperties();
                var decomposed = pis.Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(value))).ToOrderedDictionary(td.StringComparer);
                var job = JObject.FromObject(decomposed, serializer);
                var rootvalue = remapper.PeelRootForSerialize(job);
                var tokentowrite = remapper.ComingleForSerialize(job, rootvalue, valueType);
                tokentowrite.WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}
