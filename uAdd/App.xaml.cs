using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using System.Resources;

namespace uAdd
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainWindow pAddWindow;
        public ServerWindow pServerWindow;
        public List<string> CmdParams;
        public bool ServerMode = false;

        public static string SerializeToString(object obj)
        {
            var xmlSerializer = new XmlSerializer(obj.GetType());
            var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, obj);
            return stringWriter.ToString();
        }

        public static T DeserializeString<T>(string sourceString)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(sourceString);
            return (T)xmlSerializer.Deserialize(stringReader);
        }

        public static string GetSizeReadable(long i)
        {
            string sign = (i < 0 ? "-" : "");
            double readable = (i < 0 ? -i : i);

            string[] FileSizeSuffix = uAdd.Properties.Resources.FileSizeSuffix.Split(new Char[] { ',' });

            string suffix;

            if (FileSizeSuffix.Length > 6)
            {
                if (i >= 0x1000000000000000)
                {
                    suffix = FileSizeSuffix[6];  // Exabyte
                    readable = (double)(i >> 50);
                }
                else if (i >= 0x4000000000000)
                {
                    suffix = FileSizeSuffix[5];  // Petabyte
                    readable = (double)(i >> 40);
                }
                else if (i >= 0x10000000000)
                {
                    suffix = FileSizeSuffix[4];  // Terabyte
                    readable = (double)(i >> 30);
                }
                else if (i >= 0x40000000)
                {
                    suffix = FileSizeSuffix[3];  // Gigabyte
                    readable = (double)(i >> 20);
                }
                else if (i >= 0x100000)
                {
                    suffix = FileSizeSuffix[2];  // Megabyte
                    readable = (double)(i >> 10);
                }
                else if (i >= 0x400)
                {
                    suffix = FileSizeSuffix[1];  // Kilobyte
                    readable = (double)i;
                }
                else
                {
                    return i.ToString(sign + "0 " + FileSizeSuffix[0]);   // byte
                }
            }
            else
            {
                suffix = "";
            }

            readable = readable / 1024;

            return sign + readable.ToString("0.## ") + suffix;


        }

        public static string GetCulturePresentationEnum(object obj)
        {
            string result = "";

            if ((obj.GetType().BaseType).Name == "Enum")
            {                
                string StringId = "Enum" + obj.GetType().Name + obj.ToString();

                ResourceManager rm = new ResourceManager(typeof(uAdd.Properties.Resources));
                
                object EnumObject = rm.GetObject(StringId);

                if (EnumObject !=null)
                {
                    result = EnumObject.ToString();
                }                

            }

            return result;
        }



        void App_Startup(object sender, StartupEventArgs e)
        {   
            CmdParams = new List<string>();
            
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "-s" || e.Args[i] == "/s")
                {
                    ServerMode = true;
                }
                else
                {
                    CmdParams.Add(e.Args[i]);
                }
            }
                        
            //ServerMode = true;  // [debug code]

            if (CmdParams.Count() == 0)
            {
                ServerMode = true;
            }

            if (ServerMode == true)
            {
                ServerWindow MainWindow = new ServerWindow((CmdParams.Count() == 0) ? "" : CmdParams[0]);
                pServerWindow = MainWindow;
                pAddWindow = null;                
                MainWindow.Show();
                MainWindow.ShowStartupAdd();
            }
            else
            {
                MainWindow MainWindow = new MainWindow((CmdParams.Count() == 0) ? "" : CmdParams[0]);
                pAddWindow = MainWindow;
                pServerWindow = null;
                MainWindow.Show();
            }                       
            
        }

    }
}
