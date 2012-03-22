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
            string logFolder = "";
            string archiveFolder = "";
            for (int i = 0; i < args.Length; i++)
            {
                if (verbose) { Console.WriteLine("checking argument " + i + " = " + args[i]); }
                int j = i+1;
                if (args[i] == "-a") {
                    archiveFolder = args[j];
                    i++;
                }else if (args[i] == "-l") {
                    logFolder = args[j];
                    i++;
                }else if (args[i] == "-v" || args[i] == "-verbose"){
                    verbose = true;
                }else if (args[i] == "-q" || args[i] == "-quiet"){
                    quiet = true;
                }
            }
            if (archiveFolder.Length == 0)
            {
                Console.WriteLine("Archive folder parameter not given.");
                Console.ReadLine();
                return;
            }else if (!Directory.Exists(archiveFolder))
            {
                Console.WriteLine("Archive folder doesn't exists, check the -a parameter.");
                if (verbose) { Console.WriteLine("archiveFolder = "+archiveFolder); }
                Console.ReadLine();
                return;
            }
            
            if (logFolder.Length == 0)
            {
                Console.WriteLine("Logging folder parameter not given.");
                Console.ReadLine();
                return;
            }else if (!Directory.Exists(logFolder))
            {
                Console.WriteLine("Logging folder doesn't exists, check the -l parameter");
                if (verbose) { Console.WriteLine("logFolder = " + logFolder); }
                Console.ReadLine();
                return;
            }
            if (verbose && quiet)
            {
                Console.WriteLine("Should I speak loud or shut up? Make up your mind!");
                Console.ReadLine();
                return;
            }
            int del = 0, move = 0, fail = 0;
            foreach (string file in Directory.GetFiles(logFolder))
            {
                if (verbose){ Console.WriteLine("Parsing: "+file); }
                string[] filePath = file.Split('\\');
                string fileName = filePath[filePath.Length - 1];
                //channel_irma_weldon.freenode_2012.03.20.log
                if (verbose) { Console.WriteLine(fileName); }
                if (fileName.StartsWith("deadchannel_"))
                {
                    File.Delete(file);
                    if (verbose) { Console.WriteLine("DEL: "+file); }
                    del++;
                    continue;
                }
                else if(fileName.StartsWith("query_") || fileName.StartsWith("channel_"))
                {
                    /*
                     * channel_irma_weldon.freenode 2012.03.20.log
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
                        if (verbose) { Console.WriteLine("Created Directory: " + archiveFolder + '\\' + network); }
                        Directory.CreateDirectory(archiveFolder + '\\' + network);
                    }
                    if (!Directory.Exists(archiveFolder + '\\' + network + '\\' + year))
                    {
                        if (verbose) { Console.WriteLine("Created Directory: " + archiveFolder + '\\' + network + '\\' + year); }
                        Directory.CreateDirectory(archiveFolder + '\\' + network + '\\' + year);
                    }
                    if (!Directory.Exists(archiveFolder + '\\' + network + '\\' + year + '\\' + month))
                    {
                        if (verbose) { Console.WriteLine("Created Directory: " + archiveFolder + '\\' + network + '\\' + year + '\\' + month); }
                        Directory.CreateDirectory(archiveFolder + '\\' + network + '\\' + year + '\\' + month);
                    }
                    if (!Directory.Exists(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day))
                    {
                        if (verbose) { Console.WriteLine("Created Directory: " + archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day); }
                        Directory.CreateDirectory(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day);
                    }
                    //at this point the folder exists.
                    if (verbose) { Console.WriteLine(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".txt"); }
                    if (File.Exists(archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".txt"))
                    {
                        if (!quiet) { Console.WriteLine("File already exists: " + archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".txt!"); }
                        fail++;
                    }else{
                        if (verbose) { Console.WriteLine("Moving file: " + file + "to: " + archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".log"); }
                        //todo: see if year/month/day != today, if it does, ignore. if not, move.
                        move++;
                        File.Copy(file,archiveFolder + '\\' + network + '\\' + year + '\\' + month + '\\' + day + '\\' + channel + ".log");
                    }
                }
            }
            if (!quiet)
            {
                if (verbose)
                {
                    Console.WriteLine(del + " files Deleted (dead channel)");
                    Console.WriteLine(move + " files Archived successfully.");
                    Console.WriteLine(fail + " files failed.");
                }
                Console.WriteLine("DONE! (press enter to exit.)");
                Console.ReadLine();
            }
        }
    }
}
