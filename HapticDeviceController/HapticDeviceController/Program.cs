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
            Console.ReadLine();
            Console.WriteLine("abort");
            subThread.Abort();
            deviceThread.Abort();

            return;
        }

        static void FnSubscriber() {
            Console.WriteLine("start sub " + outletChannelName);
            subscriber.SubscribeTo(outletChannelName);
            subscriber.msgQueue.OnMessage(msg => eventsHandler.Router(msg.Message));
        }

    }

}
