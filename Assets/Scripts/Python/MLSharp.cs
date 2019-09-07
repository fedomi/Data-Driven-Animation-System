using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace RunPythonScript
{
    /// <summary>
    /// Machine Learning C# - Python 
    /// </summary>
    public class MLSharpPython : IMLSharpPython
    {
        private Process process;
        public readonly string filePythonExePath;

        int timeOut = 5000;

        public event EventHandler proc_finished = delegate { };
        /// <summary>
        /// ML Sharp Python class constructor
        /// </summary>
        /// <param name="exePythonPath">Python EXE file path</param>
        public MLSharpPython(string exePythonPath)
        {
            filePythonExePath = exePythonPath;
        }
        /// <summary>
        /// Execute Python script file
        /// </summary>
        /// <param name="filePythonScript">Python script file and input parameter(s)</param>
        /// <param name="standardError">Output standard error</param>
        /// <returns>Output text result</returns>
        public string ExecutePythonScript(string filePythonScript, out string standardError)
        {
            string outputText = string.Empty;
            standardError = string.Empty;
            try
            {
                UnityEngine.Debug.Log("Starting Python Process...");
                using (process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo(filePythonExePath)
                    {
                        Arguments = filePythonScript,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    process.Start();
                    outputText = process.StandardOutput.ReadToEnd();
                    outputText = outputText.Replace(Environment.NewLine, string.Empty);
                    standardError = process.StandardError.ReadToEnd();
                    //process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
            }
            return outputText;
        }


        public void SendToPython(string s) {
            //process.StandardInput.WriteLine(s);

            StreamWriter myStreamWriter = process.StandardInput;
            myStreamWriter.WriteLine(s);
            /*
            using (StreamWriter sw = process.StandardInput)
            {
                sw.WriteLine(s);
                //sw.Close();
            }*/
            /*
            string outputText = process.StandardOutput.ReadToEnd();
            outputText = outputText.Replace(Environment.NewLine, string.Empty);
            UnityEngine.Debug.Log(outputText);*/
        }


        public string ReadFromPython() {
            string outputText = string.Empty;

            //UnityEngine.Debug.Log("Read from python: " + outputText);
            outputText = process.StandardOutput.ReadLine();
            /*
            using (StreamReader sr = process.StandardOutput) {
                outputText = sr.ReadLine();
                //sr.Close();
            }*/

            return outputText;
        }

        public void ExecutePythonScriptInBackground(string filePythonScript, out string standardError)
        {

            var bw = new BackgroundWorker();
            string outputText = string.Empty;
            standardError = string.Empty;

            bw.DoWork += (sender, args) => {
                process = new Process();
                process.StartInfo = new ProcessStartInfo(filePythonExePath)
                {
                    Arguments = filePythonScript,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                };

                process.Start();

                //Thread.Sleep(1000);
                //process.StandardInput.WriteLine("EYYY");
                outputText = process.StandardOutput.ReadLine();
                outputText = outputText.Replace(Environment.NewLine, string.Empty);

                UnityEngine.Debug.Log(outputText);
                /*process.WaitForExit(timeOut);
                if (process.HasExited == false)
                    if (process.Responding)
                        process.CloseMainWindow();
                    else
                        process.Kill();*/
            };
            bw.RunWorkerCompleted += (sender, args) => {
                if (args.Error != null) { }
                this.proc_finished(this, new EventArgs());

            };
            
            bw.RunWorkerAsync();
            
        }


    }




    public interface IMLSharpPython
    {
        string ExecutePythonScript(string filePythonScript, out string standardError);

        void SendToPython(string s);

        string ReadFromPython();

        void ExecutePythonScriptInBackground(string filePythonScript, out string standardError);
    }
}

