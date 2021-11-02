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
        private string _path;
        private Dictionary<Mode, Action<T>> serializers;
        private Dictionary<Mode, Func<T>> deserializers;

        public Serializer(Mode mode, string path)
        {
            _mode = mode;
            _path = path;
            serializers = new Dictionary<Mode, Action<T>>();
            serializers.Add(Mode.BIN, SerializeBinary);
            serializers.Add(Mode.XML, SerializeXml);
            serializers.Add(Mode.JSON, SerializeJson);

            deserializers = new Dictionary<Mode, Func<T>>();
            deserializers.Add(Mode.BIN, DeserializeBinary);
            deserializers.Add(Mode.XML, DeserializeXml);
            deserializers.Add(Mode.JSON, DeserializeJson);
        }

        #region Serialize
        public void Serialize(T data)
        {
            serializers[_mode](data);
        }

        private void SerializeJson(T data)
        {
            using (StreamWriter file = new StreamWriter(_path, false))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(file, data);
            }
        }

        private  void SerializeXml(T data)
        {
            using (StreamWriter file = new StreamWriter(_path, false))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(file.BaseStream, data);
            }
        }

        private void SerializeBinary(T data)
        {
            using (StreamWriter file = new StreamWriter(_path, false))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(file.BaseStream, data);
            }
        }
        #endregion

        #region Deserialize
        public T Deserialize()
        {
            return deserializers[_mode]();
        }

        private T DeserializeBinary()
        {
            using (StreamReader file = new StreamReader(_path))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(file.BaseStream);
            }
        }

        private T DeserializeXml()
        {
            using (StreamReader file = new StreamReader(_path))
            {
                var xmlSerialize = new XmlSerializer(typeof(T));
                return (T)xmlSerialize.Deserialize(file.BaseStream);
            }
        }

        private T DeserializeJson()
        {
            using (StreamReader file = new StreamReader(_path))
            {
                return JsonConvert.DeserializeObject<T>(file.ReadToEnd());
            }
        }
        #endregion
    }
}
