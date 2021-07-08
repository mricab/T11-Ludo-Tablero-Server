using System;

namespace LudoServer
{
    public abstract class ProgramHelperFunctions
    {

        public static bool GetPort(String arg, out int Port)
        {
            ushort Aux;
            if (ushort.TryParse(arg, out Aux) && Aux > 1023) // Well-known ports: 0 to 1023
            {
                Port = Aux;
                return true;
            }
            Port = 0;
            return false;
        }
    }
}
