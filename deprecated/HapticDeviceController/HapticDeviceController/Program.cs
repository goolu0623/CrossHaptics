using System;
using RedisEndpoint;
using System.Threading;
using System.Globalization;



namespace HapticDeviceController {

    class Program {
        // normall
        public static Subscriber subscriber = new Subscriber("localhost", 6379);
        public static string outletChannelName;

        public static HapticEventsHandler eventsHandler = new HapticEventsHandler();
        public static Thread deviceThread = new Thread(eventsHandler.mainThread);
        public static Thread subThread = new Thread(FnSubscriber);



        static void Main(string[] args) {
            //testfunction();
            //Console.ReadLine();


            Console.WriteLine("HapticDeviceController start");
            outletChannelName = args[0];

            subThread.Start();
            deviceThread.Start();
            Console.WriteLine("normal start");
            Console.WriteLine("press C to switch mode");
            Console.WriteLine("press V to disable vibration");
            Console.WriteLine("press enter to stop");
#if !DEBUG
            while (true){
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key) {

                    case ConsoleKey.C:
                        eventsHandler.Switch_Solution_Mode();
                        goto default;
                    case ConsoleKey.V:
                        eventsHandler.Switch_Vibration_OnOff();
                        goto default;
                    case ConsoleKey.Enter:
                        subThread.Abort();
                        deviceThread.Abort();
                        return;
                    default:
                        break;
                }
            }   
#endif
#if DEBUG
            Console.WriteLine("Debug");
            while(true){
                if(eventsHandler.KeyCheck())
                    break;
            }
#endif

        }

        static void FnSubscriber() {
            Console.WriteLine("start sub " + outletChannelName);
            subscriber.SubscribeTo(outletChannelName);
            subscriber.msgQueue.OnMessage(msg => eventsHandler.Router(msg.Message));
        }

    }

}
