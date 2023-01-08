using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using bHapticsLibEndpoint;

namespace HapticDeviceController {
    class HapticEventsHandler {
        
        // 原本的??
        private int DELAY_TIME_WINDOW = 10; // Delay time to detect symmetric signals
        

        //bhapticslibs
        private VestController vestController = new VestController();

        private struct HapticEvent {
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
        private LinkedList<HapticEvent> eventList = new LinkedList<HapticEvent>();
        private bool IsSymmetric(ref HapticEvent nowEvent) {
            // 檢查第一個有沒有跟人對稱
            //Console.WriteLine("=============");
            foreach (var element in eventList) {
                //Console.WriteLine(element);
                if (element.Duration == nowEvent.Duration && element.Amplitude == nowEvent.Amplitude && element.Frequency == nowEvent.Frequency && element.SourceTypeName != nowEvent.SourceTypeName) {
                    // 拔掉對稱
                    //Console.WriteLine("====TRUE END=========");
                    return true;
                }
            }
            //Console.WriteLine("=====FALSE END========");
            return false;
        }

        public void Router(string msg) {
            //Console.WriteLine(msg);
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
                EnListTime : DateTime.Now
            );

            //temp.EventDayTime = DateTime.Parse(temp.EventTime, new CultureInfo("en-US"), DateTimeStyles.NoCurrentDateDefault);


            // add into eventList
            try { 
                eventList.AddLast(temp);

            }
            catch(Exception e)
            {
                Console.WriteLine($"AddLast error: {e.Message}");
            }
            

        }
        public void mainThread() {

            //bHapticsLib Vest Init
            vestController.Init();


            while (true) {
                // 看list裡面的時間過期沒
                while (eventList.Count() != 0) {
                    //CultureInfo ci = new CultureInfo("en-US");
                    DateTime nowTime = DateTime.Now;
                    DateTime eventTime = DateTime.Now;
                    try { 
                        eventTime = eventList.First().EnListTime;
                    } 
                    catch (Exception e) {
                        Console.WriteLine($"eventTime error: {e.Message}, eventTime = {eventList.First().EventDayTime}");
                    }
                    

                    // 給個time window XX ms 看這個區間有沒有產生對稱震動
                    if ((nowTime - eventTime).TotalMilliseconds < DELAY_TIME_WINDOW)
                        break;
                    HapticEvent nowEvent = eventList.First(); // error
                    eventList.RemoveFirst();
                    if (IsSymmetric(ref nowEvent)) {
                        Console.WriteLine($"SYM  {nowEvent.EventDayTime.ToString("MM/dd HH:mm:ss.fff", new CultureInfo("en-US")) + "  :"}|{nowEvent.SourceTypeName}|{nowEvent.Amplitude}|{nowEvent.Frequency}|{nowEvent.Duration}|{nowEvent.EventName}");

                        //Console.WriteLine(eventTime + ": " + "Symmetric Amp = " + nowEvent.Amplitude + " Dur = " + nowEvent.Duration + " Freq = " + nowEvent.Frequency);

                        // send 對稱event
                        // Console.WriteLine("============================================symmetric=====================================");

                        //bhapticsvest play


                        if (nowEvent.Amplitude <= 0.2)
                        {
                            vestController.Play(0);
                        }
                        else if (nowEvent.Amplitude <= 0.5 )
                        {
                            vestController.Play(1);
                        }
                        else
                        {
                            vestController.Play(2);
                        }
                        

                    }
                    else {
                        //Console.WriteLine(eventTime + ": " + "None Amp = " + nowEvent.Amplitude + " Dur = " + nowEvent.Duration + " Freq = " + nowEvent.Frequency);
                        // send 原本controller震動 (這邊我們不disable的話可以do nothing, 就不用remap回去了)
                    Console.WriteLine($"NONE {nowEvent.EventDayTime.ToString("MM/dd HH:mm:ss.fff", new CultureInfo("en-US")) + "  :"}|{nowEvent.SourceTypeName}|{nowEvent.Amplitude}|{nowEvent.Frequency}|{nowEvent.Duration}|{nowEvent.EventName}");

                    }
                }
                // 睡個1ms再來一次 免得他一直跑 block掉其他人
                // System.Threading.Thread.Sleep(1);
            }
        }
    }
}
