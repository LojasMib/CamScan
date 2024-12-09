using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Atualizate.Class
{
    public class Logger
    {
        private static readonly string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string versionPath = Path.Combine(currentDirectory, "version");
        private static readonly string logFilePath = Path.Combine(versionPath, "logger", "log.txt");
        private int count = 0;
        
        public void Log(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                using (StreamWriter write = new StreamWriter(logFilePath, append: true))
                {
                    write.WriteLine($"[{DateTime.Now}] {message} (Etapa - {count})");
                }
                count++;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Erro na gravaçõa do Log: {ex.Message}");
            }
        }
    }
}
