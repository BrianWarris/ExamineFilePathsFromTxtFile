using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamineFilePathsFromTxtFile
{
    delegate  bool FileDetectMethod(string filePath);
    class Program
    {
        static bool DetectFileExists(string filePath)
        {
            return File.Exists(filePath);
        }
        static bool DetectFileInfoExists(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            return ((fi != null) && (fi.Exists));
        }
        static bool DetermineFileExistence(string nextPath, FileDetectMethod fn, string methodName)
        {
            bool foundIt = false;
            try
            {
                foundIt = fn(nextPath);
            }
            catch (System.ArgumentException)
            {
                Debug.WriteLine($"Detected an ArgumentException on {nextPath.Length} characters when calling {methodName}");
            }
            catch (PathTooLongException)
            {
                Debug.WriteLine($"Detected a PathTooLongException on {nextPath.Length} characters when calling {methodName}");
            }
            return foundIt;
        }

        static void Main(string[] args)
        {
            // overall goal: if path to txt file was passed, then open it and iterate through the contents, commenting on file existence,
            //               else echo the syntax of the app
            bool showSyntax = false;
            List<string> lstChars = new List<string>();
            string nonBlockSpace = new string(new char[] { (char)0xfffd });

            if (((args == null) || (args.Length == 0)) || (args[0] == @"\?"))
            {
                showSyntax = true;
            }
            else
            {
                foreach (string fp in args)
                {
                    try
                    {
                        if (File.Exists(fp))
                        {
                            FileInfo fi = new FileInfo(fp);
                            Console.Out.WriteLine($"Looking at {fp} which is {fi.Length} bytes.");
                            if (fi.Length > 0L)
                            {
                                using (StreamReader sr = File.OpenText(fp))
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        string nextPath = sr.ReadLine();
                                        if (nextPath.Contains("\""))
                                        {
                                            nextPath = nextPath.Replace("\"", "");
                                        }
                                        if (nextPath.Contains(nonBlockSpace))
                                        {
                                            nextPath = nextPath.Replace(nonBlockSpace, "");
                                        }
                                        if (nextPath.Trim().Length > 0)
                                        {
                                            FileDetectMethod fdm = DetectFileExists;
                                            bool foundIt = DetermineFileExistence(nextPath, fdm, "File.Exists");
                                            fdm = DetectFileInfoExists;
                                            foundIt = DetermineFileExistence(nextPath, fdm, "new Filenfo") && foundIt;
                                            string comment = (foundIt) ? " " : " NOT ";
                                            Console.Out.WriteLine($"\tFile {nextPath} was{comment}found.");
                                        }
                                    }
                                    sr.Close();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.ToString());
                    }
                }
            }
            if (showSyntax)
            {
                Console.Out.WriteLine("ExamineFilePathsFromTxtFile  [any number of paths to txt files.]");
                Console.Out.WriteLine("For each file, iterate through the contents, commenting on file existence.");
            }
        }
    }
}
