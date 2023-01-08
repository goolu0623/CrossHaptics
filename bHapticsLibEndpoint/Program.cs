using System;
using System.IO;
using bHapticsLib;
using System.Threading;

namespace bHapticsLibEndpoint
{
    public class VestController
    {
        // bHapticLib
        private static HapticPattern testFeedbackSwapped0;
        private static HapticPattern testFeedbackSwapped1;
        private static HapticPattern testFeedbackSwapped2;
        private static bHapticsConnection Connection;
        
        static void Main()
        {

        }
        
        public void Init()
        {
            // bHapticsLib Function
            string testFeedbackPath0 = Path.Combine(Path.GetDirectoryName(typeof(VestController).Assembly.Location), "CrossHaptics_Intensity_0.tact");
            string testFeedbackPath1 = Path.Combine(Path.GetDirectoryName(typeof(VestController).Assembly.Location), "CrossHaptics_Intensity_1.tact");
            string testFeedbackPath2 = Path.Combine(Path.GetDirectoryName(typeof(VestController).Assembly.Location), "CrossHaptics_Intensity_2.tact");
            
            testFeedbackSwapped0 = HapticPattern.LoadSwappedFromFile("CrossHaptics_Intensity_0", testFeedbackPath0);
            testFeedbackSwapped1 = HapticPattern.LoadSwappedFromFile("CrossHaptics_Intensity_1", testFeedbackPath1); 
            testFeedbackSwapped2 = HapticPattern.LoadSwappedFromFile("CrossHaptics_Intensity_2", testFeedbackPath2); 
            //這個path.tactu要放的資料夾
            Console.WriteLine(testFeedbackPath0);
            Console.WriteLine("Initializing...");

            //連這支扣
            bHapticsManager.Connect("bHapticsLib", "TestApplication", maxRetries: 0);
            Thread.Sleep(10);

            //確認連上才會繼續初始化
            Connection = new bHapticsConnection("bHapticsLib2", "AdditionalConnection", maxRetries: 0);
            Connection.BeginInit();
            Console.WriteLine(Connection.Status);
        }


        public void Play(int type)
        {
            switch (type)
            { 
                case 0:
                    testFeedbackSwapped0.Play();
                    goto default;
                case 1:
                    testFeedbackSwapped1.Play();
                    goto default;
                case 2:
                    testFeedbackSwapped2.Play();
                    goto default;
                default:
                    return;
            }
        }


        //Console Key(bHapticsLib)
        private static bool KeyCheck()
        {
            if (!Console.KeyAvailable)
                return false;

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    return true;

                case ConsoleKey.D0:
                    Console.WriteLine($"{nameof(bHapticsManager.Status)}: {bHapticsManager.Status}");
                    goto default;

                case ConsoleKey.D1:
                    Console.WriteLine($"{nameof(bHapticsManager.GetConnectedDeviceCount)}(): {bHapticsManager.GetConnectedDeviceCount()}");
                    goto default;

                case ConsoleKey.D2:
                    Console.WriteLine($"{nameof(bHapticsManager.IsAnyDevicesConnected)}(): {bHapticsManager.IsAnyDevicesConnected()}");
                    goto default;

                case ConsoleKey.I:
                    Console.WriteLine($"{nameof(bHapticsManager.GetDeviceStatus)}({nameof(PositionID)}.{nameof(PositionID.HandLeft)}): {bHapticsManager.GetDeviceStatus(PositionID.Vest).ToArrayString()}");
                    goto default;

                case ConsoleKey.P:
                    //testFeedbackSwapped.Play();
                    Console.WriteLine($"{nameof(bHapticsManager.Play)}()");
                    goto default;

                /*case ConsoleKey.C:
                    testFeedbackSwapped.ChangeMode();
                    Console.WriteLine($"{nameof(testFeedbackSwapped)}.{nameof(testFeedbackSwapped.ChangeMode)}(): Is Changed to CrossHaptics Mode: {testFeedbackSwapped.ChangeMode()}");
                    goto default;*/
                default:
                    return false;
            }
        }

    }
    internal static class Extensions
    {
        internal static string ToArrayString<T>(this T[] arr) where T : IComparable<T>
        {
            if (arr == null)
                return "{ }";
            int count = arr.Length;
            if (count <= 0)
                return "{ }";
            string returnval = "{";
            for (int i = 0; i < count; i++)
            {
                returnval += $" {arr[i]}";
                if (i < count - 1)
                    returnval += ",";
            }
            returnval += " }";
            return returnval;
        }
    }
}
