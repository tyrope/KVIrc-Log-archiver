using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Archiver {
    class Program {
        static void checkWD(string wd, bool verbose = false) {
            // This function checks if the directory exists, and if not creates it.
            if(!Directory.Exists(wd)) {
                if(verbose) { Console.WriteLine("Created Directory: " + wd); }
                Directory.CreateDirectory(wd);
            }
        }
        static void Main(string[] args) {
            /*
             * Testcase (for comments below)
             * Source file: D:\IRC_Logs\query_irma_weldon.freenode_2012.03.18.log
             * Output file: D:\IRC_Logs\freenode\2012\03\20\irma_weldon.log
            */

            //Initialize variables
            bool verbose = false, quiet = false, ignore = false;
            string logFolder = "", archiveFolder = "";
            int del = 0, move = 0, fail = 0, ignored = 0;

            //apply parameters to variables.
            for(int i = 0; i < args.Length; i++) {
                int j = i + 1;
                if(args[i] == "-a") {
                    archiveFolder = args[j];
                    i++;
                } else if(args[i] == "-l") {
                    logFolder = args[j];
                    i++;
                } else if(args[i] == "-v" || args[i] == "-verbose") {
                    verbose = true;
                } else if(args[i] == "-q" || args[i] == "-quiet") {
                    quiet = true;
                } else if(args[i] == "-i" || args[i] == "-ignore") {
                    ignore = true;
                }
            }

            //check for invalid and/or missing parameters
            if(archiveFolder.Length == 0) {
                Console.WriteLine("Archive folder parameter not given.");
                Console.ReadLine();
                return;
            } else if(!Directory.Exists(archiveFolder)) {
                Console.WriteLine("Archive folder doesn't exists, check the -a parameter.");
                if(verbose) {
                    Console.WriteLine("archiveFolder = " + archiveFolder);
                }
                Console.ReadLine();
                return;
            }

            if(logFolder.Length == 0) {
                Console.WriteLine("Logging folder parameter not given.");
                Console.ReadLine();
                return;
            } else if(!Directory.Exists(logFolder)) {
                Console.WriteLine("Logging folder doesn't exists, check the -l parameter");
                if(verbose) {
                    Console.WriteLine("logFolder = " + logFolder);
                }
                Console.ReadLine();
                return;
            }

            if(verbose && quiet) {
                Console.WriteLine("Should I speak loud or shut up? Make up your mind!");
                Console.ReadLine();
                return;
            }

            ///main loop
            foreach(string file in Directory.GetFiles(logFolder)) {
                string[] filePath = file.Split('\\');
                string fileName = filePath[filePath.Length - 1];
                //channel_irma_weldon.freenode_2012.03.20.log
                if(verbose) {
                    Console.WriteLine("Parsing: " + fileName);
                }
                if(fileName.StartsWith("deadchannel_")) {
                    File.Delete(file);
                    if(verbose) {
                        Console.WriteLine("Deleting deadchannel file: " + file);
                    }
                    del++;
                    continue;
                } else if(fileName.StartsWith("query_") || fileName.StartsWith("channel_")) {
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
                    string wd = archiveFolder+'\\'; //working directory
                    DateTime now = DateTime.Parse(DateTime.Now.Day.ToString()+'-'+DateTime.Now.Month.ToString()+'-'+DateTime.Now.Year.ToString());
                    DateTime logDate = DateTime.Parse(day+'-'+month+'-'+year);

                    //Is this today's file? if so, ignore it.
                    if(now == logDate) {
                        ignored++;
                        if(verbose) {
                            Console.WriteLine("Ignoring file: " + file + "to: " + wd + '\\' + year+ '\\' + month + '\\' + day + '\\' + channel + ".txt");
                        }
                        continue;
                    }

                    //replace characters to properly display their name
                    channel = channel.Replace("%2d", "-").Replace("%2a", "[BNC]").Replace("%7b", "{").Replace("%7d", "}").Replace("%5b", "[").Replace("%5d", "]").Replace("%26", "&");

                    //check if the working directory exists.
                    wd += network;
                    checkWD(wd, verbose);
                    wd += '\\' + year;
                    checkWD(wd, verbose);
                    wd += '\\' + month;
                    checkWD(wd, verbose);
                    wd += '\\' + day;
                    checkWD(wd, verbose);
                    //at this point the folder exists.

                    //does the file exist?
                    if(File.Exists(wd + '\\' + channel + ".txt")) {
                        if(!quiet) {
                            Console.WriteLine("File already exists: " + wd + '\\' + channel + ".txt!");
                        }
                        fail++;
                    } else {
                        try {
                            File.Move(file, wd + '\\' + channel + ".txt");
                            if(verbose) {
                                Console.WriteLine("Moved file: " + file + "to: " + wd + '\\' + channel + ".txt");
                            }
                        } catch (IOException) {
                            //file already exists, delete then move.
                            if(ignore) { continue; } //or not.
                            try {
                                File.Delete(wd + '\\' + channel + ".txt");
                                File.Move(file, wd + '\\' + channel + ".txt");
                                if(verbose) {
                                    Console.WriteLine("File overwritten: " + wd + '\\' + channel + ".txt");
                                }
                                move++;
                            } catch {
                                if(verbose) {
                                    Console.WriteLine("File couldn't be moved: " + file + "to: " + wd + '\\' + channel + ".txt");
                                }
                                fail++;
                            }
                        }
                    }
                }
            }
            if(!quiet) {
                if(verbose) {
                    Console.WriteLine(del + " files deleted (dead channel)");
                    Console.WriteLine(move + " files archived.");
                    Console.WriteLine(ignored + " files ignored.");
                    Console.WriteLine(fail + " files failed.");
                }
                Console.WriteLine("DONE! (press enter to exit.)");
                Console.ReadLine();
            }
        }
    }
}