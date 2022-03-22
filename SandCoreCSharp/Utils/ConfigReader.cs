using System;
using System.IO;

namespace SandCoreCSharp.Utils
{
    static public class ConfigReader
    {
        static public string ReadParam(string fileName, string _param, char separator = '=')
        {
            FileInfo info = new FileInfo(fileName);
            if (!info.Exists)
                return "NONE";

            using (StreamReader sr = new StreamReader(fileName))
            {
                string line = ".";
                do
                {
                    line = sr.ReadLine();
                    string param = line.Split(separator)[0];
                    if (param == _param)
                        return line.Split(separator)[1];
                } while (line != "");  
            }

            return "NONE";
        }
    }
}
