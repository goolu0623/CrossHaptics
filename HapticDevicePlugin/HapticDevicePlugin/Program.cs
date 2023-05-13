using System;
using System.Collections.Generic;
using System.Globalization;
using bHapticsLib;
using RedisEndpoint;
using HapticEventNS;
using System.IO;
using System.Threading;
using System.Linq;

namespace HapticDevicePlugin_HapticVest {
    class Program {
        public static Subscriber symmetrical_event_subscriber = new Subscriber("localhost", 6379);
        public static Subscriber nonsymmetrical_event_subscriber = new Subscriber("localhost", 6379);

        public static HapticVestPlugin hapticVestPlugin = new HapticVestPlugin();
        public static string symmetrical_event_channel;
        public static string nonsymmetrical_event_channel;
        static void Main(string[] args) {
            Console.WriteLine("HapticDevicePlugin start");
            //symmetrical_event_channel = args[0];  
            symmetrical_event_channel = "symmetrical_event";
            //nonsymmetrical_event_channel = args[1];
            nonsymmetrical_event_channel = "nonsymmetrical_event";

            symmetrical_event_subscriber.SubscribeTo(symmetrical_event_channel);
            nonsymmetrical_event_subscriber.SubscribeTo(nonsymmetrical_event_channel);

            symmetrical_event_subscriber.msgQueue.OnMessage(msg =>hapticVestPlugin.sym_msg_handler("sym",msg.Message));
            nonsymmetrical_event_subscriber.msgQueue.OnMessage(msg => hapticVestPlugin.non_sym_msg_handler("nonsym", msg.Message));

            _ = Console.ReadKey();
            return;
        }
    }
    class HapticVestPlugin {
        // bHapticLib連線
        private static bHapticsConnection Connection;
        string assembly_location = Path.GetDirectoryName(typeof(VestObject).Assembly.Location);

        // bHapticObject
        public static VestObject VestLeft;
        public static VestObject VestRight;
        public static VestObject VestBoth;
        private static ScaleOption scaleOption = new ScaleOption();  

        //private LinkedList<HapticEvent> sym_eventList = new LinkedList<HapticEvent>();
        //private LinkedList<HapticEvent> non_sym_eventList = new LinkedList<HapticEvent>();
        public HapticVestPlugin() {
            // init bhaptics vest;
            VestLeft = new VestObject("左浮動五顆正面");
            VestRight = new VestObject("右浮動五顆正面");
            VestBoth = new VestObject("對稱漸弱2");

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
        public void sym_msg_handler(string type, string msg) {
            // 解析msg
            Console.WriteLine(type+msg);
            string[] haptic_event_info_lists = msg.Split('|');
            string[] state_info_list = haptic_event_info_lists[4].Split(' ');
            // 做成HapticEvent
            HapticEvent temp = new HapticEvent(
                EventTime: haptic_event_info_lists[0],
                SourceTypeName: haptic_event_info_lists[1],
                EventType: haptic_event_info_lists[2],
                EventName: haptic_event_info_lists[3],
                Amp: state_info_list[1],
                Freq: state_info_list[3],
                Dur: state_info_list[5],
                EnListTime: DateTime.Now,
                msg: msg
            );
            if (temp.get_amplitude > 0.8f) {
                switch (temp.get_source_type_name) {
                    case "1":
                        ScaleOption scaleOption = new ScaleOption();
                        scaleOption.Duration = temp.get_duration;
                        scaleOption.Intensity = temp.get_amplitude;
                        VestBoth.haptic_pattern.Play(scaleOption);
                        break;
                    default:
                        Console.WriteLine("wrong non symmetrical event to the vest");
                        break;
                }
            }
        }
        public void non_sym_msg_handler(string type, string msg) {
            // 解析msg
            Console.WriteLine(type + " " + msg);
            string[] haptic_event_info_lists = msg.Split('|');
            string[] state_info_list = haptic_event_info_lists[4].Split(' ');
            // 做成HapticEvent
            HapticEvent temp = new HapticEvent(
                EventTime: haptic_event_info_lists[0],
                SourceTypeName: haptic_event_info_lists[1],
                EventType: haptic_event_info_lists[2],
                EventName: haptic_event_info_lists[3],
                Amp: state_info_list[1],
                Freq: state_info_list[3],
                Dur: state_info_list[5],
                EnListTime: DateTime.Now,
                msg: msg
            );
            if (temp.get_amplitude > 0.8f) {
                switch (temp.get_source_type_name) {
                    case "1":
                        VestLeft.haptic_pattern.Play();
                        break;
                    case "2":
                        VestRight.haptic_pattern.Play();
                        break;
                    default:
                        Console.WriteLine("wrong non symmetrical event to the vest");
                        break;
                }
            }
        }
        //public void plugin_thread() {
        //    HapticEvent nowEvent;
        //    while (true) {
        //        if(sym_eventList.Count > 0) {
        //            lock (sym_eventList) {
        //                nowEvent = sym_eventList.First();
        //            }
        //            ScaleOption sclaleoption = new ScaleOption();
        //            sclaleoption.Intensity = nowEvent.get_amplitude;
        //            sclaleoption.Duration = nowEvent.get_duration + 0.2f;
        //            VestBoth.haptic_pattern.Play(sclaleoption);
        //            Console.WriteLine("vibrate both");
        //            lock (sym_eventList) {
        //                sym_eventList.RemoveFirst();
        //            }
        //        }
        //        if (non_sym_eventList.Count > 0) {
        //            lock (non_sym_eventList) {
        //                nowEvent = non_sym_eventList.First();
        //            }
        //            ScaleOption sclaleoption = new ScaleOption();
        //            sclaleoption.Intensity = nowEvent.get_amplitude/3;
        //            sclaleoption.Duration = nowEvent.get_duration + 0.2f;
        //            VestLeft.haptic_pattern.Play(sclaleoption);
        //            Console.WriteLine("vibrate left");
        //            lock (non_sym_eventList) {
        //                non_sym_eventList.RemoveFirst();
        //            }

        //        }
        //    }
        //}

    }

    class VestObject {
        public string name;
        public string assembly_location;
        public string tactfile_path { get; set; }
        public HapticPattern haptic_pattern { get; set; }
        public VestObject(string name) {
            this.name = name;
            this.assembly_location = Path.GetDirectoryName(typeof(VestObject).Assembly.Location);
            this.tactfile_path = Path.Combine(this.assembly_location, this.name + ".tact");
            this.haptic_pattern = HapticPattern.LoadFromFile(this.name, this.tactfile_path);
        }
    }


}
