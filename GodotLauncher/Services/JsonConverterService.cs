using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GodotLauncher.Services
{
    /// <summary>
    /// Helper class to serialize and deserialize JSON object
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize/deserialize</typeparam>
    public class JsonConverterService<T> where T : class
    {
        /// <summary>
        /// Logger instance for the class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Deserializes an object from the given JSON file
        /// </summary>
        /// <param name="fileName">The name of the file to be deserialized</param>
        /// <returns>A deserialized object of the given type</returns>
        public static T Deserialize(string fileName)
        {
            T res;

            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();

                res = (T)serializer.Deserialize(file, typeof(T));
            }

            return res;
        }

        /// <summary>
        /// Serializes an object to the given file
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="fileName">The filename for the output</param>
        public static void Serialize(T obj, string fileName)
        {
            if (!fileName.EndsWith(".json"))
                fileName += ".json";

            using(var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();

#if DEBUG
                serializer.Formatting = Formatting.Indented;
#endif

                serializer.Serialize(file, obj);
            }
        }
    }
}
