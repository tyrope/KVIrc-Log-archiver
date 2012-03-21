using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace Archiver
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Testcase (for comments below)
             * Source file: D:\IRC_Logs\query_irma_weldon.freenode_2012.03.18.log
             * Output file: D:\IRC_Logs\freenode\2012\03\20\irma_weldon.log
            */
            //check for launch parameters
            bool verbose = false;
            bool quiet = false;
            if (args.Length > 1)
            {
                verbose = (args[0] == "-v" || args[0] == "-verbose" || args[1] == "-v" || args[1] == "-verbose");
                quiet = (args[0] == "-q" || args[0] == "-quiet" || args[1] == "-q" || args[1] == "-quiet");
            }else if (args.Length > 0)
            {
                verbose = (args[0] == "-v" || args[0] == "-verbose");
                quiet = (args[0] == "-q" || args[0] == "-quiet");
            }
            string logFolder = @"D:\IRC_Logs";
            string archiveFolder = @"D:\IRC_Logs\Archive";
            string[] logs_ori = Directory.GetFiles(logFolder);
            foreach(string file in logs_ori){
                if (verbose){ Console.WriteLine(file); }
                string[] filePath = file.Split('\\');
                string fileName = filePath[filePath.Length - 1];
                //channel_irma_weldon.freenode_2012.03.20.log
                if (verbose) { Console.WriteLine(fileName); }
                if (fileName.StartsWith("deadchannel_"))
                {
                    continue;
                }
                if(fileName.StartsWith("query_") || fileName.StartsWith("channel_"))
                {
                    //channel irma_weldon.freenode 2012.03.20.log
                    /*
                     * remove prefix -> irma_weldon.freenode_2012.03.18.log
                     * split by . -> irma_weldon freenode_2012 03 18 log
                     * split [1] by _ -> irma_weldon freenode 2012 03 18 log
                    */
                    string[] nameParts = fileName.Replace("query_", "").Replace("channel_", "").Split('.');
                    string channel = nameParts[0];
                    string network = nameParts[1].Split('_')[0];
                    string year = nameParts[1].Split('_')[1];
                    string month = nameParts[2];
                    string day = nameParts[3];


                    channel = channel.Replace("%2d", "-").Replace("%2a", "[BNC]").Replace("%7b", "{").Replace("%7d", "}").Replace("%5b", "[").Replace("%5d", "]");
                    if (!Directory.Exists(archiveFolder + '\\' + network))
                    {
                        Directory.CreateDirectory(archiveFolder + '\\' + network);
                    }
                    if (!Directory.Exists(archiveFolder + '\\' + network + '\\' + year))
                    {
                        Directory.CreateDirectory(archiveFolder + '\\' + network + '\\' + year);
                    }
                    if (!Directory.Exists(archiveFolder + '\\' + network + '\\' + year + '\\' + month))
                    {
                        Directory.CreateDirectory(archiveFolder + '\\' + network + '\\' + year + '\\' + month);
                    }
                    if (!Directory.Exists(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day))
                    {
                        Directory.CreateDirectory(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day);
                    }
                    //at this point the folder exists.
                    if (verbose) { Console.WriteLine(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".txt"); }
                    if (File.Exists(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".txt"))
                    {
                        if (!quiet) { Console.WriteLine("File already exists: " + archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".txt!"); }
                    }else{
                        File.Copy(file,archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".log");
                    }
                }
            }
            if (!quiet)
            {
                Console.WriteLine("DONE!");
                Console.ReadLine();
            }
        }
    }
}
