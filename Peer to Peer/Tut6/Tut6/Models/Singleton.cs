using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    public class Singleton 
    {
        private static Server instance;

        public Singleton()
        {
            if(instance == null)
            {
                instance = new Server(); 
            }
        }

        public static Server getServer()
        {
            if (instance == null)
            {
                instance = new Server();
            }
            return instance;
        }
    }
       

     
}
