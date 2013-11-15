// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameManager;
using System.Threading;
using Microsoft.Xna.Framework;

namespace GrappleGame
{
    class LoadingScreen : GameScreen
    {
        private Thread backgroundThread;
        Auxiliary aux;

        public LoadingScreen()
        {
            aux = new Auxiliary();

            aux.trace("KEVIN", "In loading screen.");

            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
        }

        void BackgroundLoadContent()
        {
            // load images
            ScreenManager.Game.Content.Load<object>("images/alien1");
            ScreenManager.Game.Content.Load<object>("images/backdrop");
            ScreenManager.Game.Content.Load<object>("images/background");
            ScreenManager.Game.Content.Load<object>("images/badguy_blue");
            ScreenManager.Game.Content.Load<object>("images/badguy_green");
            ScreenManager.Game.Content.Load<object>("images/badguy_orange");
            ScreenManager.Game.Content.Load<object>("images/badguy_red");
            ScreenManager.Game.Content.Load<object>("images/bullet");
            ScreenManager.Game.Content.Load<object>("images/cloud1");
            ScreenManager.Game.Content.Load<object>("images/cloud2");
            ScreenManager.Game.Content.Load<object>("images/enemy");
            ScreenManager.Game.Content.Load<object>("images/fire");
            ScreenManager.Game.Content.Load<object>("images/ground");
            ScreenManager.Game.Content.Load<object>("images/hills");
            ScreenManager.Game.Content.Load<object>("images/laser");
            ScreenManager.Game.Content.Load<object>("images/moon");
            ScreenManager.Game.Content.Load<object>("images/mountains_blurred");
            ScreenManager.Game.Content.Load<object>("images/player");
            ScreenManager.Game.Content.Load<object>("images/smoke");
            ScreenManager.Game.Content.Load<object>("images/sun");
            ScreenManager.Game.Content.Load<object>("images/tank");
            ScreenManager.Game.Content.Load<object>("images/tank_tire");
            ScreenManager.Game.Content.Load<object>("images/tank_top");
            ScreenManager.Game.Content.Load<object>("images/title");

            // load sounds
            ScreenManager.Game.Content.Load<object>("sounds/player_hit");
            ScreenManager.Game.Content.Load<object>("sounds/tank_fire");
            ScreenManager.Game.Content.Load<object>("sounds/alien_hit");

            // load fonts
            ScreenManager.Game.Content.Load<object>("fonts/gamefont");
            ScreenManager.Game.Content.Load<object>("fonts/menufont"); 
            ScreenManager.Game.Content.Load<object>("fonts/scorefont");
            ScreenManager.Game.Content.Load<object>("fonts/titlefont");
        }

        public override void LoadContent()
        {
            if (backgroundThread == null)
            {
                backgroundThread = new Thread(BackgroundLoadContent);
                backgroundThread.Start();
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (backgroundThread != null && backgroundThread.Join(10))
            {
                backgroundThread = null;
                this.ExitScreen();
                ScreenManager.AddScreen(new MainMenuScreen());
                ScreenManager.Game.ResetElapsedTime();
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
