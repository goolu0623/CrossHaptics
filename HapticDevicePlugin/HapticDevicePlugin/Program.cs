using System;
using System.Collections.Generic;
using System.Globalization;
using RedisEndpoint;

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

        private LinkedList<HapticEvent> eventList = new LinkedList<HapticEvent>();
        public HapticVestPlugin() {

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
                EnListTime: DateTime.Now
            );
            // add into eventList
            lock (eventList) {
                eventList.AddLast(temp);
            }
        }
        public void plugin_thread() {

        }

    }

    class HapticEvent {
        // 性質
        public float Duration; // 這三個基本資訊合起來是 channel 裡的 stateinfo 
        public float Amplitude;
        public float Frequency;
        public DateTime EventDayTime;// new CultureInfo("en-US");

        public string EventType;
        public string EventTime;
        public string EventName;
        public string SourceTypeName;

        public DateTime EnListTime;


        public HapticEvent(string EventTime, string SourceTypeName, string EventType, string EventName, string Amp, string Freq, string Dur, DateTime EnListTime) {
            this.Duration = Convert.ToSingle(Dur);
            this.Amplitude = Convert.ToSingle(Amp);
            this.Frequency = Convert.ToSingle(Freq);

            this.EventDayTime = DateTime.Parse(EventTime, new CultureInfo("en-US"), DateTimeStyles.NoCurrentDateDefault);
            this.EventTime = EventTime;
            this.EventType = EventType;
            this.EventName = EventName;
            this.SourceTypeName = SourceTypeName;

            this.EnListTime = EnListTime;

        }
    };
}
