using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TcpProtocol
{
    [Serializable]
    public class Package : ISerializable
    {
        public byte type;
        public Object obj = null;

        public Package() { }

        public Package(byte type, Object obj = null)
        {
            this.type = type;
            this.obj = obj;
        }

        protected Package(SerializationInfo info, StreamingContext context)
        {
            type = (byte)info.GetByte("type");
            obj = (Object)info.GetValue("obj", typeof(Object));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("type", type);
            info.AddValue("obj", obj);
        }

        public Type TypeOfContent()
        {
            return obj.GetType();
        }
    }
}
