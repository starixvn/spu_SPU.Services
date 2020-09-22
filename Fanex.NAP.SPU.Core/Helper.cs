namespace Fanex.NAP.SPU.Core
{
    using System;
    using System.IO;
    using System.Text;

    public static class Helper
    {
        public static void WriteXML<T>(string fileName, T data)
        {
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var path = AppDomain.CurrentDomain.BaseDirectory + fileName;

            if (!File.Exists(path) || !IsFileLocked(new FileInfo(path)))
            {
                var file = new StreamWriter(path,false,Encoding.UTF8);
                writer.Serialize(file, data);
                file.Close();
            }
        }

        public static T ReadXML<T>(string fileName)
        {
           var reader = new System.Xml.Serialization.XmlSerializer(typeof(T));
            var path = AppDomain.CurrentDomain.BaseDirectory + fileName;
            var output = default(T);
            try
            {
                if(!CheckFileExist(path))
                {
                    return output;
                }

                var file = new StreamReader(path);
                output = (T)reader.Deserialize(file);
                file.Close();

                return output;
            }
            catch
            {
                return output;
            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    //file is not locked
                    return false;
                }
            }
            catch
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
        }

        public static bool CheckFileExist(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }

            return true;
        }
    }
}
