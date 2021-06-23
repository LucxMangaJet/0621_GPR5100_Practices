

namespace  UdpPractice
{
    
public static class PacketFactory
{
    public static Packet ToPacket(byte[] data)
    {

        PacketType type = data[0];

        switch(type)
        {
            case PacketType.Message:
                return MessagePacket.FromBytes(data);
            break;

            default:
            return null;

        }

    }    
}



}