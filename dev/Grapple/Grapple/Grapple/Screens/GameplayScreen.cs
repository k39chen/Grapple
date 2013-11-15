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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;

namespace GrappleGame
{
    class GameplayScreen : GameScreen
    {
        enum Direction { LEFT, RIGHT, UP, DOWN};

        const float SCREEN_WIDTH = 800.0f;
        const float SCREEN_HEIGHT = 480.0f;

        const float LEFT_OFFSET = 20.0f;
        const float TOP_OFFSET = 50.0f;
        const float BOTTOM_OFFSET = 20.0f;

        Texture2D playerTexture;    // player texture
        Texture2D enemyTexture;     // enemy texture
        Texture2D backdropTexture;  // backdrop texture
        
        Player playerInst;          // the player instance in the game
        Enemy enemyInst;            // the enemy instance in the game

        Color tint;                 // tint of the textures drawn
        SpriteFont default_font;    // application fonts

        Auxiliary aux;                  // auxiliary
        TouchCollection touchState;     // handle screen touches
        Vector2 touchPressPosition;     // current touch press position
        Vector2 touchMovePosition;      // current touch move position
        Vector2 touchReleasePosition;   // previous touch move position

        float fingerRadius;             // radius of the finger circular collision detection

        public GameplayScreen()
        {
            aux = new Auxiliary();
            aux.trace("KEVIN", "In the gameplay screen.");

            tint = Color.White;

            playerInst = new Player();
            enemyInst = new Enemy();

            touchPressPosition = new Vector2(0, 0);
            touchMovePosition = new Vector2(0, 0);
            touchReleasePosition = new Vector2(0, 0);

            fingerRadius = 15.0f;
        }

        public override void LoadContent()
        {
            ScreenManager.Game.GraphicsDevice.Clear(Color.BlueViolet);

            // set fonts
            default_font = ScreenManager.Game.Content.Load<SpriteFont>("fonts/menufont");

            // set images
            backdropTexture = ScreenManager.Game.Content.Load<Texture2D>("images/backdrop");
            playerTexture = ScreenManager.Game.Content.Load<Texture2D>("images/player");
            enemyTexture = ScreenManager.Game.Content.Load<Texture2D>("images/enemy");

            // game instances
            playerInst.texture = playerTexture;
            enemyInst.texture = enemyTexture;

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsActive)
            {
                UpdatePlayer();
                UpdateEnemy();
            }

            GetInput(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ScreenManager.SpriteBatch.Begin();

            DrawString(default_font, "something", new Vector2(LEFT_OFFSET, TOP_OFFSET), Color.Black, 5.0f);

            DrawBackdrop();
            DrawEnemy();
            DrawPlayer();

            ScreenManager.SpriteBatch.End();

            if (IsCollide(playerInst.collision, enemyInst.collision))
            {
                aux.trace("KEVIN", "Collision occurred");
            }
            else
            {
                aux.trace("KEVIN", "No collision yet.");
            }
        }

        #region HANDLE INPUT
        
        public void GetInput(GameTime gameTime)
        {
            touchState = TouchPanel.GetState();

            foreach (TouchLocation location in touchState)
            {
                switch (location.State)
                {
                    case TouchLocationState.Pressed:
                        touchPressPosition = touchState[0].Position;
                        break;
                    case TouchLocationState.Moved:
                        touchMovePosition = touchState[0].Position;
                        break;
                    case TouchLocationState.Released:
                        touchReleasePosition = touchState[0].Position;
                        break;
                }
            }
        }

        public uint GetDirectionThrown()
        {
            float angle = (float)(Math.Atan2((double)(touchReleasePosition.Y - touchPressPosition.Y), (double)(touchReleasePosition.X - touchPressPosition.X)) * 180 / Math.PI);
            if (angle < 0) angle = Math.Abs(angle);
            else angle = Math.Abs(angle - 180) + 180;

            // return direction based on stuff
            if (angle >= 0 && angle < 45) return (uint)(Direction.RIGHT);
            if (angle > 315 && angle <= 360) return (uint)(Direction.RIGHT);
            if (angle >= 45 && angle < 135) return (uint)(Direction.UP);
            if (angle >= 135 && angle < 225) return (uint)(Direction.LEFT);
            if (angle >= 225 && angle < 315) return (uint)(Direction.DOWN);

            return (uint)(Direction.LEFT);
        }

        #endregion

        #region PLAYER CLASS
        
        public class Player
        {
            enum State {IDLE, DASH, GRAB, THROW, STRIKE, STRUGGLE};
            
            public Texture2D texture;
            public Vector2 pos;
            public Vector2 vel;
            public Vector2 acc;
            public uint state;
            public uint direction;
            public Circle collision;

            public Player()
            {
                pos = new Vector2(0, 0);
                vel = new Vector2(0, 0);
                acc = new Vector2(0, 0);

                state       = (uint)(State.IDLE);
                direction   = (uint)(Direction.RIGHT);
            }
        }

        public void DrawPlayer()
        { 
            ScreenManager.SpriteBatch.Draw(playerInst.texture, 
                new Vector2(playerInst.pos.X - playerInst.texture.Width/2,
                    playerInst.pos.Y - playerInst.texture.Height/2), tint);
        }

        public void UpdatePlayer()
        {
            playerInst.pos = touchMovePosition;
            playerInst.collision = new Circle(playerInst.pos, playerInst.texture.Width/2);
        }
        
        #endregion

        #region ENEMY CLASS

        public class Enemy
        {
            enum State { SEARCH, CATCH, CHEER, GRABBED, THROWN, DEAD };

            public Texture2D texture;
            public Vector2 pos;
            public Vector2 vel;
            public Vector2 acc;
            public uint state;
            public uint direction;
            public uint orientation;
            public Circle collision;

            public Enemy()
            {
                pos = new Vector2(0, 0);
                vel = new Vector2(0, 0);
                acc = new Vector2(0, 0);

                state = (uint)(State.SEARCH);
                direction = (uint)(Direction.RIGHT);
                orientation = (uint)(Direction.LEFT);
            }
        }

        public void DrawEnemy()
        {
            ScreenManager.SpriteBatch.Draw(enemyInst.texture,
                new Vector2(enemyInst.pos.X - enemyInst.texture.Width / 2,
                    enemyInst.pos.Y - enemyInst.texture.Height / 2), tint);
        }

        public void UpdateEnemy()
        {
            //enemyInst.pos.Y++;

            enemyInst.pos = new Vector2(50, 50);
            enemyInst.collision = new Circle(enemyInst.pos, enemyInst.texture.Width / 2);
        }

        #endregion

        #region DRAW HELPERS

        public class Circle
        {
            public Vector2 origin;
            public double radius;

            public Circle(Vector2 rOrigin, double rRadius)
            {
                origin = rOrigin;
                radius = rRadius;
            }
        }

        public void DrawBackdrop()
        {
            ScreenManager.SpriteBatch.Draw(backdropTexture, new Vector2(0, 0), tint);
        }
        
        public void DrawString(SpriteFont font, string text, Vector2 position, Color color)
        {
            ScreenManager.SpriteBatch.DrawString(font, text, new Vector2(position.X + 1, position.Y + 1), Color.Black);
            ScreenManager.SpriteBatch.DrawString(font, text, position, color);
        }

        public void DrawString(SpriteFont font, string text, Vector2 position, Color color, float fontScale)
        {
            ScreenManager.SpriteBatch.DrawString(font, text, new Vector2(position.X + 1, position.Y + 1), Color.Black, 0, new Vector2(0, font.LineSpacing / 2), fontScale, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.DrawString(font, text, position, color, 0, new Vector2(0, font.LineSpacing / 2), fontScale, SpriteEffects.None, 0);
        }

        public float CalculateDistance(Vector2 pos1, Vector2 pos2)
        {
            return (float)(Math.Sqrt(Math.Pow((pos2.X - pos1.X), 2.0) + Math.Pow((pos2.Y - pos1.Y), 2.0)));
        }

        public bool IsCollide(Circle circ1, Circle circ2)
        {
            if (circ1 == null || circ2 == null) return false;
            return (CalculateDistance(circ1.origin, circ2.origin) <= circ1.radius + circ2.radius);
        }

        #endregion
    }
}