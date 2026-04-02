using System;

namespace Start
{
    [Serializable]
    public class Login_C2S : IMessage 
    {
        
    }
    
    [Serializable]
    public class Login_S2C : IMessage
    {
        public int Session;
    }
}