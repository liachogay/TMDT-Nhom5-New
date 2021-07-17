using System;
using System.IO;

namespace MyProject.Models
{
    public class PaypalLogger
    {
        public static string LogDirectoryPath = Environment.CurrentDirectory;
        public static void Log(String message)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(LogDirectoryPath + "\\PaypalError.log", true);
                streamWriter.WriteLine("{0}--->{1}", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), message);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}