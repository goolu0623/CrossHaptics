using System;
using System.IO;
using bHapticsLib;
using System.Threading;

namespace bHapticsLibEndpoint {
    // bHaptic device part enum
    public enum BodyPart {
        vest = 1,
        head = 2
    }
    public enum VibrateIntensity {
        low = 1,
        medium = 2,
        high = 3
    }
    public enum BodySide {
        left = 1,
        right = 2
    }


    public class VestController {
        string assembly_location = Path.GetDirectoryName(typeof(VestController).Assembly.Location);
        

        // bHapticLib暫時分類
        private static HapticPattern vestFeedbackright;
        private static HapticPattern vestFeedbackLeft;
        private static HapticPattern vestFeedbackAllLow;
        private static HapticPattern vestFeedbackAllMed;
        private static HapticPattern vestFeedbackAllHigh;
        private static HapticPattern headFeedbackAllLow;
        private static HapticPattern headFeedbackAllMed;
        private static HapticPattern headFeedbackAllHigh;
        // bHapticLib舊的
        private static HapticPattern testFeedbackSwapped0;
        private static HapticPattern testFeedbackSwapped1;
        private static HapticPattern testFeedbackSwapped2;
        private static bHapticsConnection Connection;


        public void Init() {
            // bHapticsLib Function
            string testFeedbackPath0 = Path.Combine(this.assembly_location, "CrossHaptics_Intensity_0.tact");
            string testFeedbackPath1 = Path.Combine(this.assembly_location, "CrossHaptics_Intensity_1.tact");
            string testFeedbackPath2 = Path.Combine(this.assembly_location, "CrossHaptics_Intensity_2.tact");

            testFeedbackSwapped0 = HapticPattern.LoadSwappedFromFile("CrossHaptics_Intensity_0", testFeedbackPath0);
            testFeedbackSwapped1 = HapticPattern.LoadSwappedFromFile("CrossHaptics_Intensity_1", testFeedbackPath1);
            testFeedbackSwapped2 = HapticPattern.LoadSwappedFromFile("CrossHaptics_Intensity_2", testFeedbackPath2);
            //這個path.tact要放的資料夾
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


        public void Play(int type) {
            switch (type) {
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

        static void Main(string[] args) {

            return;
        }


    }
    
    public abstract class HapticObject {
        public string name;
        public string assembly_location;
        public string tactfile_path { get; set; }
        HapticPattern haptic_pattern { get; set; }
    }

    public class VestObject: HapticObject{
        BodyPart body_part;
        BodySide body_side;
        VibrateIntensity intensity;

        VestObject(string name) {
            this.name = name;
            this.assembly_location = Path.GetDirectoryName(typeof(VestController).Assembly.Location);
            this.tactfile_path = Path.Combine(this.assembly_location, this.name + ".tact");
        }

    }




}
