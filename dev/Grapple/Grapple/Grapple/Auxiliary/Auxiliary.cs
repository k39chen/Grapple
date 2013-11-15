using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrappleGame
{
    class Auxiliary
    {
        public Auxiliary()
        {
            System.Diagnostics.Debug.WriteLine("<<<< KEVIN >>>> Created instance of auxiliary helper functions class.");   
        }

        public void trace(string owner, string message)
        {
            System.Diagnostics.Debug.WriteLine("<<<< " + owner + " >>>> " + message);
        }
    }
}
