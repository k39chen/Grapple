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

namespace GrappleGame
{
    class MainMenuScreen : MenuScreen
    {
        Auxiliary aux;

        public MainMenuScreen()
            : base("Main")
        {
            aux = new Auxiliary();

            aux.trace("KEVIN", "In main menu screen.");

            // Create our menu entries.
            MenuEntry startGameMenuEntry    = new MenuEntry("START GAME");
            MenuEntry gameHelpMenuEntry     = new MenuEntry("GAME HELP");
            MenuEntry creditsMenuEntry      = new MenuEntry("CREDITS");
            MenuEntry exitMenuEntry         = new MenuEntry("QUIT");

            // Hook up menu event handlers.
            startGameMenuEntry.Selected += StartGameMenuEntrySelected;
            gameHelpMenuEntry.Selected += GameHelpMenuEntrySelected;
            creditsMenuEntry.Selected += CreditsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(startGameMenuEntry);
            MenuEntries.Add(gameHelpMenuEntry);
            MenuEntries.Add(creditsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        void StartGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new GameplayScreen());
        }

        void GameHelpMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new GameHelpScreen());
        }

        void CreditsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new CreditsScreen());
        }

        protected override void OnCancel()
        {
            aux.trace("KEVIN", "Exiting application.");
            ScreenManager.Game.Exit();
        }
    }
}
