using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//Используется: https://www.newtonsoft.com/json

namespace StopHandler.Infrastructure.Files
{
    static class JSONConverter
    {       
        public static T OpenJSONFile<T>(string path)
        {
            try
            {
                if (!File.Exists(path)) return default(T);
                string fileText = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(fileText);
            }
            catch (Exception)
            {
                return default(T);
            }

        }
        public static bool SaveJSONFile<T>(T data, string path)
        {
            try
            {
                var jsonFile = JsonConvert.SerializeObject(data);
                File.WriteAllText(path, jsonFile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
