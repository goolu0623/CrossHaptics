using System;
using System.Collections.Generic;
using System.Globalization;
using bHapticsLib;
using RedisEndpoint;
using HapticEventNS;
using System.IO;

namespace HapticDevicePlugin_HapticVest {
    class Program {
        public static Subscriber symmetrical_event_subscriber = new Subscriber("localhost", 6379);
        public static Subscriber nonsymmetrical_event_subscriber = new Subscriber("localhost", 6379);

        public static HapticVestPlugin hapticVestPlugin = new HapticVestPlugin();
        public static string symmetrical_event_channel;
        public static string nonsymmetrical_event_channel;
        static void Main(string[] args) {
            Console.WriteLine("HapticDevicePlugin start");
            symmetrical_event_channel = args[0];  
            nonsymmetrical_event_channel = args[1];
            symmetrical_event_subscriber.SubscribeTo(symmetrical_event_channel);
            nonsymmetrical_event_subscriber.SubscribeTo(nonsymmetrical_event_channel);
            symmetrical_event_subscriber.msgQueue.OnMessage(msg =>hapticVestPlugin.msg_handler("sym",msg.Message));
            nonsymmetrical_event_subscriber.msgQueue.OnMessage(msg => hapticVestPlugin.msg_handler("nonsym", msg.Message));
            while (true) {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.Enter:
                        break;
                    default:
                        continue;
                }

            }
        }
    }
    class HapticVestPlugin {
        // bHapticLib連線
        private static bHapticsConnection Connection;
        string assembly_location = Path.GetDirectoryName(typeof(VestController).Assembly.Location);

        // bHapticObject
        public static VestObject VestLeft;


        private LinkedList<HapticEvent> eventList = new LinkedList<HapticEvent>();
        public HapticVestPlugin() {
            // init bhaptics vest;
        }
        public void msg_handler(string type, string msg) {
            // 解析msg
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
            // add into eventList
            lock (eventList) {
                eventList.AddLast(temp);
            }
        }
        public void plugin_thread() {
            while (true) {
                if(eventList.Count > 0) { 

                }
            }
        }

    }

    class VestObject {
        public string name;
        public string assembly_location;
        public string tactfile_path { get; set; }
        public HapticPattern haptic_pattern { get; set; }
        public VestObject(string name) {
            this.name = name;
            this.assembly_location = Path.GetDirectoryName(typeof(VestController).Assembly.Location);
            this.tactfile_path = Path.Combine(this.assembly_location, this.name + ".tact");
            this.haptic_pattern = HapticPattern.LoadFromFile(this.name, this.tactfile_path);
        }
    }


}
