using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace MyLibs.Serialisation
{  
    public enum Mode
    {
        JSON,
        XML,
        BIN
    }
    public class Serializer<T> 
    {
        private Mode _mode;
        private Dictionary<Mode, Action<T, string>> serializers;
        private Dictionary<Mode, Func<string, T>> deserializers;
        public Serializer(Mode mode)
        {
            _mode = mode;
            serializers = new Dictionary<Mode, Action<T, string>>();
            serializers.Add(Mode.BIN, SerializeBinary);
            serializers.Add(Mode.XML, SerializeXml);
            serializers.Add(Mode.JSON, SerializeJson);

            deserializers = new Dictionary<Mode, Func<string, T>>();
            deserializers.Add(Mode.BIN, DeserializeBinary);
        }

        #region Serialize
        public void Serialize(T data, string path)
        {
            serializers[_mode](data, path);
        }

        private void SerializeJson(T data, string path)
        {
            using (StreamWriter file = new StreamWriter(path, true))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(file, data);
            }
        }

        private  void SerializeXml(T data, string path)
        {
            using (StreamWriter file = new StreamWriter(path, true))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(file.BaseStream, data);
            }
        }

        private void SerializeBinary(T data, string path)
        {
            using (StreamWriter file = new StreamWriter(path, true))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(file.BaseStream, data);
            }
        }
        #endregion

        #region Deserialize
        public T Deserialize(string path)
        {
            return deserializers[_mode](path);
        }

        private T DeserializeBinary(string path)
        {
            using (StreamReader file = new StreamReader(path))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(file.BaseStream);
            }
        }
        #endregion
    }
}
