using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RunPythonScript
{
    public class Program : MonoBehaviour
    {
        private float currentTime = 0.0f;
        private float inputTimer = 5.0f;

        // Get config settings
        //private static string filePythonExePath = Properties.Settings.Default.FilePythonExePath;
        //private static string folderImagePath = Properties.Settings.Default.FolderImagePath;
        //private static string filePythonNamePath = Properties.Settings.Default.FilePythonNamePath;
        //private static string filePythonParameterName = Properties.Settings.Default.FilePythonParameterName;

        private static string filePythonExePath = "C:/Users/fdomi/AppData/Local/Programs/Python/Python35/python.exe";
        private static string filePythonNamePath = "F:/ProyectosUnity/VanillaMotionMatching/VanillaMotionMatching/Assets/Scripts/Python/TrainedNN.py";
        private IMLSharpPython mlSharpPython;
        void Start()
        {
            string outputText, standardError;

            // Instantiate Machine Learning C# - Python class object            
            mlSharpPython = new MLSharpPython(filePythonExePath);
            // Test image
            //string imagePathName = folderImagePath + "Image_Test_Name.png";
            // Define Python script file and input parameter name
            //string fileNameParameter = $"{filePythonNamePath} {filePythonParameterName} {imagePathName}";
            string fileNameParameter = filePythonNamePath;
            // Execute the python script file 
            //outputText = mlSharpPython.ExecutePythonScriptInBackground(fileNameParameter, out standardError);
            mlSharpPython.ExecutePythonScriptInBackground(fileNameParameter, out standardError);
            if (string.IsNullOrEmpty(standardError))
            {
                //UnityEngine.Debug.Log(outputText);
                //Console.WriteLine(outputText);

            }
            else
            {
                Console.WriteLine(standardError);
            }
            //Console.ReadKey();
        }

        void Update()
        {
            if(currentTime > inputTimer)
            {
                mlSharpPython.SendToPython("HOLA.");
                UnityEngine.Debug.Log(mlSharpPython.ReadFromPython());
                currentTime = 0.0f;
            }

            currentTime += Time.deltaTime;
        }


    }
}