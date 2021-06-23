using System.Collections.Generic;
using System;

namespace UdpPractice
{

    public class PacketExcutor
    {
        Dictionary<Type, Action<Packet>> packetExecutionMap = new Dictionary<Type, Action<Packet>>();


        public void Execute(Packet packet)
        {
            Type t = packet.GetType();

            if(packetExecutionMap.ContainsKey(t))
            {
                packetExecutionMap[t].Invoke(packet);
            }else
            {
                throw new Exception($"No execution method defined for {t.ToString()}");
            }
        }

        public void DefineExecutionFor(Type t, Action<Packet> method)
        {
                packetExecutionMap.Add(t,method);
        }

    }

    
}