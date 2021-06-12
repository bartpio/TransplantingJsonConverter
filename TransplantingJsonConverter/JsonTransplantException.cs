using Newtonsoft.Json;
using System;

namespace TransplantingJsonConverter
{
    /// <summary>
    /// The exception thrown when an error occurs during JSON transplantation
    /// </summary>
    public sealed class JsonTransplantException : JsonSerializationException
    {
        /// <summary>
        /// Initializes a new instance of JsonTransplantException
        /// </summary>
        public JsonTransplantException()
        {
        }

        /// <summary>
        /// Initializes a new instance of JsonTransplantException, given a message
        /// </summary>
        /// <param name="message">
        /// message describing the problem
        /// </param>
        public JsonTransplantException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of JsonTransplantException, given a message, and inner exception
        /// </summary>
        /// <param name="message">
        /// message describing the problem
        /// </param>
        /// <param name="inner">
        /// inner exception
        /// </param>
        public JsonTransplantException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
