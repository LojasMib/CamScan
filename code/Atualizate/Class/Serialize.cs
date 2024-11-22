using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atualizate.Class
{
    internal class Serialize
    {
        public T DeserializerJson<T>(string fileJson)
        {
            return JsonSerializer.Deserialize<T>(fileJson);
        }

        public string SerializerJson<T>(T version)
        {
            string jsonString = JsonSerializer.Serialize(version, new JsonSerializerOptions { WriteIndented = true });
            return jsonString;
        }
    }
}
