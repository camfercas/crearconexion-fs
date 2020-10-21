using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrearConexion
{
    class GraboLog
    {
        public static void GrabarLog(string texto)
        {
            if (!Directory.Exists(@"C:\Log"))
            {
                Directory.CreateDirectory(@"C:\Log");
            }

            // Your program runs, you add log lines
            string[] start = { DateTime.Now + ": " + texto };
            string date = DateTime.Today.ToString("d");
            date = date.Replace("/", "");
            File.AppendAllLines(@"C:\Log\Log-Conector-" + date + ".txt", start);
        }

    }
}
