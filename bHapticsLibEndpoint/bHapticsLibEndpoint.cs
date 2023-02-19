using System;
using System.IO;
using bHapticsLib;
using System.Threading;

namespace bHapticsLibEndpoint {
    // bHaptic device part enum
    public enum BodyPart { vest = 1, head = 2 }
    public enum BodySide { left = 1, right = 2, both = 3 }
    public class VestController {
        string assembly_location = Path.GetDirectoryName(typeof(VestController).Assembly.Location);

        // bHapticObject
        public static VestObject VestLeft;
        public static VestObject VestRight;
        public static VestObject VestBoth;
        public static HeadObject HeadBoth;


        // bHapticLib連線
        private static bHapticsConnection Connection;


        public void Init() {
            // bHapticsLib Function
            VestLeft = new VestObject("left_vest", BodySide.left); // 現在裡面是1ms
            VestRight = new VestObject("right_vest", BodySide.right); // 現在裡面是1ms
            VestBoth = new VestObject("both_vest", BodySide.both); // 現在裡面是1ms
            HeadBoth = new HeadObject("both_head", BodySide.both); // 現在裡面是1ms


            //這個path.tact要放的資料夾
            Console.WriteLine(this.assembly_location);
            Console.WriteLine("Initializing...");

            //連這支扣
            bHapticsManager.Connect("bHapticsLib", "TestApplication", maxRetries: 0);
            Thread.Sleep(10);

            //確認連上才會繼續初始化
            Connection = new bHapticsConnection("bHapticsLib2", "AdditionalConnection", maxRetries: 0);
            Connection.BeginInit();
            Console.WriteLine(Connection.Status);

        }


        public void Play(float amp, float freq, float dur, BodySide body_side) {
            // 目前會傳進來的震動都是對稱的pattern
            // 所以只要先看amp然後決定要vibrate多少而已
            freq = 100f; // 未了清掉未使用parameter的error而已

            if (body_side == BodySide.both) {
                // 如果duration很大 就直接照正常給訊號
                // 如果duratino小到低於11ms controller對於震動強度的感受也會爆減, 這種時候要減一下amplitude
                // 減amplitude的依據暫時先以小於11ms的畫 等比例減少? 11ms->0ms 把amplitude呈線性的減少
                ScaleOption scale_option = new ScaleOption();
                scale_option.Duration = dur;
                if (dur >= 0.011f) {
                    scale_option.Intensity = amp;
                }
                else if (dur == 0f) {
                    scale_option.Intensity = amp;
                    scale_option.Duration = 0.011f;
                }
                else {
                    scale_option.Intensity = amp * (dur / 0.011f +0.2f);
                }
                VestBoth.haptic_pattern.Play(scale_option);
                HeadBoth.haptic_pattern.Play(scale_option);
            }
            // 左右手的話 只有amp>0.9的會call近來, 在這邊我們做一個/3的強度的震動 避免它影響到主要的controller體驗
            // 另外為了避免震動小到背心跑不出來 很小的dur我們會取一個lower bound 小於0.01f的都直接call 0.01f
            else if (body_side == BodySide.left) {
                ScaleOption scale_option = new ScaleOption(intensity: amp / 3, duration: System.Math.Max(dur,0.01f));
                VestLeft.haptic_pattern.Play(scale_option);
            }
            else if (body_side == BodySide.right) {
                ScaleOption scale_option = new ScaleOption(intensity: amp / 3, duration: System.Math.Max(dur, 0.01f));
                VestRight.haptic_pattern.Play(scale_option);
            }

        }

        static void Main() {

            return;
        }


    }

    public abstract class HapticObject {
        public string name;
        public string assembly_location;
        public string tactfile_path { get; set; }
        public HapticPattern haptic_pattern { get; set; }
        public BodyPart body_part;
        public BodySide body_side;
    }

    public class VestObject : HapticObject {
        public VestObject(string name, BodySide side) {
            this.name = name;
            this.assembly_location = Path.GetDirectoryName(typeof(VestController).Assembly.Location);
            this.tactfile_path = Path.Combine(this.assembly_location, this.name + ".tact");
            this.haptic_pattern = HapticPattern.LoadSwappedFromFile(this.name, this.tactfile_path);
            this.body_part = BodyPart.vest;
            this.body_side = side;
        }
    }

    public class HeadObject : HapticObject {
        public HeadObject(string name, BodySide side) {
            this.name = name;
            this.assembly_location = Path.GetDirectoryName(typeof(VestController).Assembly.Location);
            this.tactfile_path = Path.Combine(this.assembly_location, this.name + ".tact");
            this.haptic_pattern = HapticPattern.LoadFromFile(this.name, this.tactfile_path);
            this.body_part = BodyPart.head;
            this.body_side = side;
        }
    }




}
