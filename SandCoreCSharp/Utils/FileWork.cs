using System.IO;
using System.Collections.Generic;

namespace SandCoreCSharp.Utils
{
    public class FileWork
    {
        static public void Write(string path, string message)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(message);
            sw.Close();
        }

        static public string[] Read(string path)
        {
            if (!File.Exists(path))
                return new string[0];

            StreamReader sr = new StreamReader(path);
            List<string> lines = new List<string>();

            string line = "";

            while (true)
            {
                line = sr.ReadLine();

                if (line == "" || line == null)
                    break;

                lines.Add(line);
            }

            sr.Close();
            return lines.ToArray();
        }
    }
}
