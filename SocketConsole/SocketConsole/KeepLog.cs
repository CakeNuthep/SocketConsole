using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketConsole
{
    class KeepLog
    {
        public KeepLog()
        {

        }
        public bool write(string name, string message, string path, bool append = true)
        {
            //string PATH = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\LOG.txt";
            //using (TextWriter write = new StreamWriter(PATH, true))
            //{
            //    if (permission)
            //        write.WriteLine("{0} - {1}({2}) : Allowed access", DateTime.Now, IP, Code);
            //    else
            //        write.WriteLine("{0} - {1}({2}) : Access denied", DateTime.Now, IP, Code);
            //}
            try
            {

                if (Directory.Exists(path))
                {
                    int year = DateTime.Now.Year;
                    int month = DateTime.Now.Month;
                    int day = DateTime.Now.Day;
                    string pathYear = Path.Combine(path, year.ToString());
                    if (!Directory.Exists(pathYear))
                    {
                        Directory.CreateDirectory(pathYear);
                    }
                    string pathMonth = Path.Combine(pathYear, month.ToString());
                    if (!Directory.Exists(pathMonth))
                    {
                        Directory.CreateDirectory(pathMonth);
                    }
                    string fileName = day.ToString("D2") + ".txt";
                    string pathFile = Path.Combine(pathMonth, fileName);
                    if (!File.Exists(pathFile))
                    {
                        using (TextWriter write = new StreamWriter(pathFile, true))
                        {
                            write.WriteLine("Name;Message;TimeStamp");
                        }
                    }
                    using (TextWriter write = new StreamWriter(pathFile, true))
                    {
                        string log = message;
                        write.WriteLine("{0};{1};{2}", name, message, DateTime.Now);
                        return true;
                    }
                }
            }
            catch
            {

            }
            return false;
        }
    }
}
