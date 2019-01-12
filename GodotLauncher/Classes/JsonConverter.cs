using System;
using System.IO;
using Newtonsoft.Json;

namespace GodotLauncher.Classes
{
    public class JsonConverter<T> where T : class
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static T Deserialize(string fileName)
        {
            T res;

            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();

                res = (T)serializer.Deserialize(file, typeof(T));
            }

            return res;
        }

        public static void Serialize(T obj, string fileName)
        {
            using(var file = File.CreateText(fileName))
            {
                var serializer = new JsonSerializer();

                serializer.Formatting = Formatting.Indented;

                serializer.Serialize(file, obj);
            }
        }
    }
}
