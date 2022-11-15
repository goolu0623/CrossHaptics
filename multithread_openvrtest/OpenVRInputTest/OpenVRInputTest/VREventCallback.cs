using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using System.Globalization;

namespace OpenVRInputTest
{
    public class VREventCallback
    {
        public enum DeviceType
        {
            HMD,
            LeftController,
            RightController
        }
        public static void SendEventOut(string SourceTypeName, string EventType, string EventName, string StateInfo,ref Queue<Tuple<string,string>> output_data)
        {
#if DEBUG
            /*
             Example:
             From RightController Get Digital Item: Trigger Data: Pressed
             From RightController Get Digital Item: TriggerVector1 Data: {0.4627, 0.0000, 0.0000 }
             From RightController Get Output Item: Vibration Data: Amp 0.2000 Freq 20.0000 Duration 0.0000
             From HMD Get Pose Item: ObjectAttitude Data: Position {-1.0498, 0.8372, 1.1911 } Quaternion {0.2779, -0.0208, 0.9600, 0.0272 }
             */
            if (EventType != "Pose")
            {
                CultureInfo ci = new CultureInfo("en-US");
                //Utils.PrintInfo($"{DateTime.Now.ToString("MM/dd HH:mm:ss.fff", ci)} | From {SourceTypeName} Get {EventType} Item: {EventName} Data: {StateInfo}");
                //Utils.PrintInfo($"{}")

            }
            //Utils.PrintInfo($"From {SourceTypeName} Get {EventType} Item: {EventName} Data: {StateInfo}");
#endif
            if (Program.outletChannelName != null)
            {
                //CultureInfo ci = new CultureInfo("en-US");
                //Program.publisher.Publish(Program.outletChannelName, $"{DateTime.Now.ToString("MM/dd HH:mm:ss", ci)}|{SourceTypeName}|{EventType}|{EventName}|{StateInfo}");
                if(SourceTypeName == "HMD")
                {
                    //LogWriterTest.LogWriter.LogWrite("\n", "console.txt");
                }
                //LogWriterTest.LogWriter.LogWrite($"{SourceTypeName}|{EventType}|{EventName}|{StateInfo}", "console.txt");
                CultureInfo ci = new CultureInfo("en-US");
                string EventTime = DateTime.Now.ToString("MM/dd HH:mm:ss.fff", ci)+"  :";
                lock (output_data) {
                    output_data.Enqueue(Tuple.Create($"{EventTime}|{SourceTypeName}|{EventType}|{EventName}|{StateInfo}",EventType));
                }
            }
            
        }
        public static void NewDigitalEvent(DeviceType SourceType, DigitalControllerEvent EventClass, bool State, ref Queue<Tuple<string,string>> output_data) {
            string SourceTypeName = Enum.GetName(typeof(DeviceType), SourceType);
            string EventType = "Digital";
            string EventName = EventClass.EventName();
            string StateInfo = (State ? "Pressed" : "Released");
            SendEventOut(SourceTypeName, EventType, EventName, StateInfo, ref output_data);
        }
        public static void NewAnalogEvent(DeviceType SourceType, AnalogControllerEvent EventClass, in InputAnalogActionData_t AnalogData, ref Queue<Tuple<string,string>> output_data) {
            string SourceTypeName = Enum.GetName(typeof(DeviceType), SourceType);
            string EventType = "Digital";
            string EventName = EventClass.EventName();
            string StateInfo = $"{{{AnalogData.x:F4}, {AnalogData.y:F4}, {AnalogData.z:F4} }}";
            SendEventOut(SourceTypeName, EventType, EventName, StateInfo, ref output_data);
        }
        public static void NewPoseEvent(DeviceType SourceType, PoseControllerEvent EventClass, in HmdVector3_t PoseData, in HmdQuaternion_t QuaternionData, ref Queue<Tuple<string,string>> output_data) {
            string SourceTypeName = Enum.GetName(typeof(DeviceType), SourceType);
            string EventType = "Pose";
            string EventName = EventClass.EventName();
            string StateInfo = $"Position {{{PoseData.v0:F4}, {PoseData.v1:F4}, {PoseData.v2:F4} }} Quaternion {{{QuaternionData.w:F4}, {QuaternionData.x:F4}, {QuaternionData.y:F4}, {QuaternionData.z:F4} }}";
            SendEventOut(SourceTypeName, EventType, EventName, StateInfo, ref output_data);
        }
        public static void NewPoseEvent(DeviceType SourceType, in HmdVector3_t PoseData, in HmdQuaternion_t QuaternionData,ref Queue<Tuple<string,string>> output_data) {
            string SourceTypeName = Enum.GetName(typeof(DeviceType), SourceType);
            string EventType = "Pose";
            string EventName = "ObjectAttitude";
            string StateInfo = $"Position {{{PoseData.v0:F4}, {PoseData.v1:F4}, {PoseData.v2:F4} }} Quaternion {{{QuaternionData.w:F4}, {QuaternionData.x:F4}, {QuaternionData.y:F4}, {QuaternionData.z:F4} }}";
            SendEventOut(SourceTypeName, EventType, EventName, StateInfo, ref output_data);
            //lock (output_data) {
            //    output_data.Enqueue($"{SourceTypeName}|{EventType}|{EventName}|{StateInfo}");
            //}
        }
        public static void NewVibrationEvent(ETrackedControllerRole DeviceType, VREvent_HapticVibration_t HapticData, ref Queue<Tuple<string,string>> output_data)
        {
            string SourceTypeName;
            switch (DeviceType)
            {
                case ETrackedControllerRole.LeftHand:
                    SourceTypeName = "LeftController";
                    break;
                case ETrackedControllerRole.RightHand:
                    SourceTypeName = "RightController";
                    break;
                default:
                    return;
            }
            string EventType = "Output";
            string EventName = "Vibration";
            string StateInfo = $"Amp {HapticData.fAmplitude:F4} Freq {HapticData.fFrequency:F4} Duration {HapticData.fDurationSeconds:F4}";
            SendEventOut(SourceTypeName, EventType, EventName, StateInfo, ref output_data);
            //lock (output_data) {
            //    output_data.Enqueue($"{SourceTypeName}|{EventType}|{EventName}|{StateInfo}");
            //}
            //LogWriterTest.LogWriter.LogWrite($"{SourceTypeName}|{EventType}|{EventName}|{StateInfo}", "console.txt");
        }
    }
}
