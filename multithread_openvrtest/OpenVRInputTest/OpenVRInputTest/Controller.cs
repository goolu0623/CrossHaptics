﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;
using static OpenVRInputTest.VREventCallback;

namespace OpenVRInputTest
{
    public class Controller
    {
        public string ActionHandleBasePath = "/actions/default/in/left_";
        public string ControllerName = "LeftController";
        public ulong ControllerHandle = 0;
        public List<ControllerEvent> EventList = new List<ControllerEvent>();
        DeviceType _ControllerType;
        public DeviceType ControllerType
        {
            get
            {
                return _ControllerType;
            }
        }
        public Controller(DeviceType ControllerType, string ControllerName, string ControllerHandlePath, string ActionHandleBasePath)
        {
            this._ControllerType = ControllerType;
            this.ControllerName = ControllerName;
            this.ActionHandleBasePath = ActionHandleBasePath;
            OpenVR.Input.GetInputSourceHandle(ControllerHandlePath, ref ControllerHandle);
        }
        public Controller AttachNewEvent(ControllerEvent controllerEvent)
        {
            if (controllerEvent == null)
                throw new Exception("In Controller.AttachNewEvent: controllerEvent is Null!");
            EventList.Add(controllerEvent);
            controllerEvent.AttachToController(this);
            return this;
        }
        public void UpdateAllState(ref Queue<Tuple<string,string>> output_data)
        {
            foreach (ControllerEvent controllerEvent in EventList)
            {
                switch (controllerEvent.EventType())
                {
                    case ControllerEvent.EventTypeEmun.Digital:
                        controllerEvent.DigitalFetchEventResult(ref output_data);
                        break;
                    case ControllerEvent.EventTypeEmun.Analog:
                        controllerEvent.AnalogFetchEventResult(ref output_data);
                        break;
                    case ControllerEvent.EventTypeEmun.Pose:
                        controllerEvent.PoseFetchEventResult(ref output_data);
                        break;
                }
            }
        }
    }
}
