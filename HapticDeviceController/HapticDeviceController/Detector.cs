﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using bHapticsLibEndpoint;
using bHapticsLib;

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
            foreach (var element in eventList) {
                if (element.Duration == nowEvent.Duration && element.Amplitude == nowEvent.Amplitude && element.Frequency == nowEvent.Frequency && element.SourceTypeName != nowEvent.SourceTypeName) {
                    // 拔掉對稱
                    return true;
                }
            }
            return false;
        }

        public void Router(string msg) {
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
            try {
                eventList.AddLast(temp);

            }
            catch (Exception e) {
                Console.WriteLine($"AddLast error: {e.Message}");
            }


        }
        public void mainThread() {
            //System.Threading.Thread.CurrentThread.IsBackground = true;
            //bHapticsLib Vest Init
            vestController.Init();
#if DEBUG
            Console.WriteLine("debug mode");
#endif

            while (true) {

#if DEBUG
                //Console.WriteLine("123");
                KeyCheck(); //按鍵可以主動觸發vibration
#endif
                // 看list裡面的時間過期沒
                while (eventList.Count() != 0) {
                    DateTime nowTime = DateTime.Now;
                    DateTime eventTime = nowTime;
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
                        vestController.Play(
                            amp: nowEvent.Amplitude,
                            freq: nowEvent.Frequency,
                            dur: nowEvent.Duration,
                            body_side: BodySide.both
                        );
                    }
                    else {
                        //Console.WriteLine(eventTime + ": " + "None Amp = " + nowEvent.Amplitude + " Dur = " + nowEvent.Duration + " Freq = " + nowEvent.Frequency);
                        // send 原本controller震動 (這邊我們不disable的話可以do nothing, 就不用remap回去了)
                        Console.WriteLine($"NONE {nowEvent.EventDayTime.ToString("MM/dd HH:mm:ss.fff", new CultureInfo("en-US")) + "  :"}|{nowEvent.SourceTypeName}|{nowEvent.Amplitude}|{nowEvent.Frequency}|{nowEvent.Duration}|{nowEvent.EventName}");
                        if (nowEvent.Amplitude > 0.9f && nowEvent.SourceTypeName == "LeftController") {
                            vestController.Play(
                                amp: nowEvent.Amplitude,
                                freq: nowEvent.Frequency,
                                dur: nowEvent.Duration,
                                body_side: BodySide.left
                            );
                        }
                        else if (nowEvent.Amplitude > 0.9f && nowEvent.SourceTypeName == "RightController") {
                            vestController.Play(
                                amp: nowEvent.Amplitude,
                                freq: nowEvent.Frequency,
                                dur: nowEvent.Duration,
                                body_side: BodySide.right
                            );
                        }
                    }
                }
                // 睡個1ms再來一次 免得他一直跑 block掉其他人
                // System.Threading.Thread.Sleep(1);
            }
        }

        public bool KeyCheck() {
            if (!Console.KeyAvailable)
                return false;
            ScaleOption temp = new ScaleOption();
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key) {
                case ConsoleKey.Enter:
                    return true;

                case ConsoleKey.P:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.1f, 100f, 0.04f, BodySide.both);
                    Console.WriteLine("get P");
                    goto default;


                case ConsoleKey.N:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(1f, 100f, 0.04f, BodySide.both);
                    Console.WriteLine("get N");

                    goto default;

                case ConsoleKey.M:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.5f, 100f, 0.04f, BodySide.both);
                    Console.WriteLine("get M");
                    goto default;

                case ConsoleKey.J:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(1f, 100f, 1f, BodySide.both);
                    Console.WriteLine("get J");
                    goto default;
                case ConsoleKey.Z:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.1f, 100f, 0.04f, BodySide.both);
                    Console.WriteLine("get Z");
                    goto default;
                case ConsoleKey.X:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.2f, 100f, 0.03f, BodySide.both);
                    Console.WriteLine("get X");
                    goto default;
                case ConsoleKey.C:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.5f, 100f, 0.02f, BodySide.both);
                    Console.WriteLine("get C");
                    goto default;
                case ConsoleKey.V:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(1f, 100f, 0.001f, BodySide.both);
                    Console.WriteLine("get V");
                    goto default;
                case ConsoleKey.A:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.1f, 200f, 0.01f, BodySide.both);
                    Console.WriteLine("get A");
                    goto default;
                case ConsoleKey.S:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.2f, 200f, 0.001f, BodySide.both);
                    Console.WriteLine("get S");
                    goto default;
                case ConsoleKey.D:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(0.5f, 200f, 0.0005f, BodySide.both);
                    Console.WriteLine("get D");
                    goto default;
                case ConsoleKey.F:
                    temp.Intensity = 0.1f;
                    temp.Duration = 1f;
                    vestController.Play(1f, 200f, 0.0001f, BodySide.both);
                    Console.WriteLine("get F");
                    goto default;


                default:
                    return false;
            }
        }
    }

}