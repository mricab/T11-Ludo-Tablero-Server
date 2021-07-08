using System;

namespace TcpProtocol
{
    public abstract class Protocol
    {
        public static Package GetPackage(string typeName, Object obj = null)
        {
            byte code = TypeCode(typeName);
            switch (typeName)
            {
                case "connected":   return new Package(code, obj);
                case "keep":        return new Package(code);
                case "message":     return new Package(code, obj);
                default:            return null;
            }
        }

        public static String TypeName(int actionCode)
        {
            switch (actionCode)
            {
                case 0:     return "connected";
                case 1:     return "keep";
                case 2:     return "message";
                default:    return null;
            }
        }

        public static byte TypeCode(string actionName)
        {
            switch (actionName)
            {
                case "connected":   return 0;
                case "keep":        return 1;
                case "message":     return 2;
                default:            return 255; // null
            }
        }
    }
}
