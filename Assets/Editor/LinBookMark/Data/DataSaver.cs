using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LinBookMark
{
    public static class DataSaver
    {
        public const string DataFileName = "LinBookMarkTreeHiLoJ";
        
         public static void WriteToDisk(string fileName, object serializeObject)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".dat";
#if UNITY_5_4_OR_NEWER
			string str = JsonUtility.ToJson(serializeObject);
            File.AppendAllText(path, str + Environment.NewLine);
#else
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
			bf.Serialize(fs, serializeObject);
			fs.Close();
#endif
		}
        public static T ReadFromDisk<T>(string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".dat";
            T returnObject = default(T);
            if (File.Exists(path))
            {
#if UNITY_5_4_OR_NEWER
				using (StreamReader streamReader = new StreamReader(path))
                {
                    string line;
                    while (!string.IsNullOrEmpty(line = streamReader.ReadLine()))
                    {
                        returnObject = Deserialize<T>(line);
                    }
                }
#else
				FileStream fs = new FileStream(path, FileMode.Open);
				BinaryFormatter bf = new BinaryFormatter();
				fs.Seek(0, SeekOrigin.Begin);
				returnObject = (T)bf.Deserialize(fs);
				fs.Close();
#endif
			}
			return returnObject;
        }
#if UNITY_5_4_OR_NEWER
		private static T Deserialize<T>(string text)
        {
            text = text.Trim();
            Type typeFromHandle = typeof(T);
            object obj = null;
            try
            {
                obj = JsonUtility.FromJson<T>(text);
            }
            catch (Exception ex)
            {
                Debug.LogError("Cannot deserialize to type " + typeFromHandle.ToString() + ": " + ex.Message + ", Json string: " + text);
            }
            if (obj != null && obj.GetType() == typeFromHandle)
            {
                return (T)obj;
            }
            return default(T);
        }
#endif
		public static void ClearData(string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName + ".dat";
            if (File.Exists(path))
            {
                using (FileStream fileStream = File.Open(path, FileMode.Open))
                {
                    fileStream.SetLength(0L);
                }
            }
        }
    }
}