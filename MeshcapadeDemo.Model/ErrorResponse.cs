using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MeshcapadeDemo.Model
{
    public class ErrorResponse
    {
        /// <summary>
        /// string type message
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        /// <summary>
        /// Exception type innerException
        /// </summary>
        [JsonPropertyName("innerException")]
        public Exception? InnerException { get; set; }
    }
}
