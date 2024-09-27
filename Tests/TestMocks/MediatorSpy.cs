using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;

namespace ReupVirtualTwinTests.mocks
{
    public class MediatorSpy : IMediator
    {
        public List<ReupEvent> receivedEvents = new List<ReupEvent>();
        public List<object> receivedPayloads = new List<object>();
        public void Notify(ReupEvent eventName)
        {
            receivedEvents.Add(eventName);
            receivedPayloads.Add(null);
        }

        public void Notify<T>(ReupEvent eventName, T payload)
        {
            receivedEvents.Add(eventName);
            receivedPayloads.Add(payload);
        }
    }

}