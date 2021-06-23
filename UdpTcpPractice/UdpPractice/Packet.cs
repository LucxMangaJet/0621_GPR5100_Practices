
 namespace  UdpPractice
{
    public abstract class Packet

    {
        public abstract byte[] GetData();
    }



    public class MessagePacket : Packet
    {
        private byte userID;
        private string message;

        public string Message {get => message;}

        public MessagePacket(byte _userID, string _message)
        {
            userID = _userID;
            message = _message;
        }

        public static MessagePacket FromBytes(byte[] data)
        {
            if(data == null || data.Length==0)
                throw new DecodingException("Data is null");

            if(data[0] != (byte) PacketType.Message)
                throw new DecodingException($"PacketID Mismatch: expected {PacketType.Message} got {data[0]}");
            
            byte userId = data[1];

            try
            {
                ushort length = System.BitConverter.ToUInt16(data, 2);
                string message = System.Text.Encoding.UTF8.GetString(data, 4, length);

            }catch (System.Exception e)
            {
                if(e is System.ArgumentOutOfRangeException || e is System.ArgumentException)
                {
                    throw new DecodingException("Packet is too small");
                }
                throw;
            }


            return new MessagePacket(userId, message);
        }


        public override byte[] GetData()
        {
            byte[] msgAsUTF8 = System.Text.Encoding.UTF8.GetBytes(message);
            
            byte[] data = new byte[4 + msgAsUTF8.Length];

            data[0] =  (byte) PacketType.Message;            
            data[1] = userID;

            ushort msgLen = (ushort)msgAsUTF8.Length;            
            byte[] msgLenBytes = System.BitConverter.GetBytes(msgLen);

            data[2] = msgLenBytes[0];
            data[3] = msgLenBytes[1];

            System.Array.Copy(msgAsUTF8, 0, data, 4, msgAsUTF8.Length);


            return data;
        }


        public class DecodingException : System.Exception
        {
            public DecodingException(string msg ) : base(msg)
            {
            }
        }
    }
}