﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CrossHapticsLauncher {
    class CrossHapticsLauncher {
        static void Main(string[] args) {
            Process capturerProcess = new Process();
            Process classifierProcess = new Process();
            string filePath=Directory.GetCurrentDirectory();
                for(int i = 0; i < 4; i++) {
                    filePath = Directory.GetParent(filePath).FullName;
                }
            string capturerPath = Path.Combine(filePath + "\\OpenVRInputTest\\OpenVRInputTest\\bin\\Release\\OpenVRInputTest.exe");
            string classifierPath = Path.Combine(filePath + "\\VibrationSignalClassifier\\VibrationSignalClassifier\\bin\\Release\\VibrationSignalClassifier.exe");
            //string capturerPath = "D:\\DW\\HCI\\crosshaptics_gitfolder\\OpenVRInputTest\\OpenVRInputTest\\bin\\Release\\OpenVRInputTest.exe";
            capturerProcess.StartInfo.FileName= capturerPath;
            capturerProcess.StartInfo.Arguments = "test";
            //capturerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            bool result = capturerProcess.Start();
            Console.WriteLine(result);

            //string classifierPath = "D:\\DW\\HCI\\crosshaptics_gitfolder\\VibrationSignalClassifier\\VibrationSignalClassifier\\bin\\Debug\\VibrationSignalClassifier.exe";
            classifierProcess.StartInfo.FileName = classifierPath;
            classifierProcess.StartInfo.Arguments = "test";
            //classifierProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            bool result2 = classifierProcess.Start();
            Console.WriteLine(result2);
            Console.WriteLine("Press Any Key To Stop All The Scripts");
            Console.ReadLine();
            //classifierProcess.CloseMainWindow();
            //capturerProcess.CloseMainWindow();
            classifierProcess.Kill();
            capturerProcess.Kill();
            classifierProcess.WaitForExit();
            capturerProcess.WaitForExit();
            classifierProcess.Dispose();
            capturerProcess.Dispose();
            //classifierProcess.Close();
            //capturerProcess.Close();
            
            //Console.ReadLine();
            return;
        }
    }
}
