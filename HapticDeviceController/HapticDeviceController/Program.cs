using System;
using RedisEndpoint;
using System.Threading;
using System.Globalization;



namespace HapticDeviceController {

    class Program {
        // normall
        public static Publisher publisher = new Publisher("localhost", 6379);
        public static Subscriber subscriber = new Subscriber("localhost", 6379);
        public static string outletChannelName;

        public static HapticEventsHandler eventsHandler = new HapticEventsHandler();
        public static Thread deviceThread = new Thread(eventsHandler.mainThread);
        public static Thread pubThread = new Thread(fn_publisher);
        public static Thread subThread = new Thread(fn_subscriber);



        static void Main(string[] args) {
            //testfunction();
            //Console.ReadLine();



            Console.WriteLine("HapticDeviceController start");
            outletChannelName = args[0];

            subThread.Start();
            //pubThread.Start();
            deviceThread.Start();
            Console.WriteLine("normal start");
            Console.ReadLine();
            subThread.Abort();
            //pubThread.Abort();

            //deviceThread.Abort();


            return;
        }
        static void testfunction() {
            CultureInfo ci = new CultureInfo("en-US");
            string startTime = DateTime.Now.ToString(ci);
            while (timediff(startTime) < 10000) {
                continue;
            }
        }
        static double timediff(string target_time) {
            CultureInfo ci = new CultureInfo("en-US");
            DateTime targetTime = DateTime.Parse(target_time, ci, DateTimeStyles.NoCurrentDateDefault);
            DateTime nowTime = DateTime.Now;
            double time_diff = (nowTime - targetTime).TotalMilliseconds;
            System.Console.WriteLine(time_diff);
            return time_diff;
        }

        static void fn_publisher() {
            Console.WriteLine("start pub");
            int cnt = 0;
            while (true) {
                if(cnt%10000000 == 0) {
                    publisher.Publish(outletChannelName, cnt.ToString());
                    System.Console.WriteLine("publish"+cnt.ToString());
                }
                cnt++;
            }
            //publisher.Publish(outletChannelName,"123");
            //Program.publisher.Publish(Program.outletChannelName, $"{DateTime.Now.ToString("MM/dd HH:mm:ss", ci)}|{SourceTypeName}|{EventType}|{EventName}|{StateInfo}");

            //return;
        }

        static void fn_subscriber() {
            Console.WriteLine("start sub " + outletChannelName);
            subscriber.SubscribeTo(outletChannelName);
            //subscriber.msgQueue.OnMessage(msg => TestPrinter(msg.Message));
            subscriber.msgQueue.OnMessage(msg => eventsHandler.Router(msg.Message));
        }
        static void TestPrinter(string msg) {
            Console.WriteLine(msg);
        }
        //static void Router(string msg) {
        //    System.Console.WriteLine(msg);
        //    return;
        //}




        
    }
    
}
