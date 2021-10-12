using Newtonsoft.Json;
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
    public class Serializer<T> where T : new()
    {

        private XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        private BinaryFormatter binaryFormatter = new BinaryFormatter();
        private Mode _mode;
        private bool _append;
        private bool _indent;
        public Serializer(Mode mode, bool append = false, bool indent = false)
        {
            _mode = mode;
            _append = append;
            _indent = indent;
        }
        public void Serialize(T data, string path)
        {
            switch (_mode)
            {
                case Mode.BIN:
                {
                    if (_append)
                    {
                        using (StreamWriter file = new StreamWriter(path, true))
                        {
                            binaryFormatter.Serialize(file.BaseStream, data);
                        }
                    }
                    else
                    {
                        using (StreamWriter file = new StreamWriter(path, false))
                        {
                            binaryFormatter.Serialize(file.BaseStream, data);
                        }
                    }
                    break;
                }
                case Mode.XML:
                {
                    if (_append)
                    {
                        using (StreamWriter file = new StreamWriter(path, true))
                        {
                            xmlSerializer.Serialize(file.BaseStream, data);
                        }
                    }
                    else
                    {
                        using (StreamWriter file = new StreamWriter(path, false))
                        {
                            xmlSerializer.Serialize(file.BaseStream, data);
                        }
                    }
                    break;
                }
                case Mode.JSON:
                {
                    if (!_append)
                    {
                        using (StreamWriter writer = new StreamWriter(path))
                        {
                            if (!_indent)
                                writer.WriteLine(JsonConvert.SerializeObject(data));
                            else
                                writer.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
                        }
                    }
                    else
                    {
                        using (StreamWriter writer = new StreamWriter(path, true))
                        {
                            if (!_indent)
                                JsonConvert.SerializeObject(data);
                            else
                                JsonConvert.SerializeObject(data, Formatting.Indented);
                        }
                    }
                    break;
                }
            }
        }
        public T Deserialize(string path)
        {
            var t = new T();
            switch(_mode)
            {
                case Mode.BIN:
                {
                    using (StreamReader file = new StreamReader(path))
                    {
                        return (T)binaryFormatter.Deserialize(file.BaseStream);
                    }
                }
                case Mode.XML:
                {
                    using (StreamReader file = new StreamReader(path))
                    {
                        return (T)xmlSerializer.Deserialize(file.BaseStream);
                    }
                }
                case Mode.JSON:
                {
                    using (StreamReader file = new StreamReader(path))
                    {
                        return JsonConvert.DeserializeObject<T>(file.ReadToEnd());
                    }
                }
            }
            return t;
        }
    }
}
