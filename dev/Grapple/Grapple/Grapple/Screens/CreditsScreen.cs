using GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrappleGame
{
    class CreditsScreen : GameScreen
    {
        Auxiliary aux;

        public CreditsScreen()
        {
            aux = new Auxiliary();
            aux.trace("KEVIN", "In the credits screen.");
        }
    }
}
