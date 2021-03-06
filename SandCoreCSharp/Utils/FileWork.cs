using System.IO;
using System.Collections.Generic;

namespace SandCoreCSharp.Utils
{
    public class FileWork
    {
        static public void Write(string path, string message)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path);
                sw.Write(message + '\n');
                sw.Close();
            }
            catch (System.Exception)
            {
            }          
        }

        static public string[] Read(string path)
        {
            if (!File.Exists(path))
                return null;

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
