using Google.Protobuf;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netwrok
{
    public class Proxy<T> : Singleton_CSharp<T> where T : class, new()
    {
        protected string ChannelName = "Main";
        protected void AddServerListener(int type, Action<IMessage> handle)
        {
            ServerListener.AddHandle(type, handle);
        }

        public void SendSocket(Packet packet)
        {
            NetworkManager.SendMessage(ChannelName, packet);
        }

        public PacketEntity<T> CreatePacket<T>(int msgType) where T : IMessage, new()
        {
            var packet = ObjectPool<PacketEntity<T>>.Instance.Get();
            packet.ID = (short)msgType;
            packet.message = Activator.CreateInstance<T>();
            return packet;
        }
}
}
