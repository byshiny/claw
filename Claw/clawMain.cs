﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
 
#endregion

namespace Claw
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>



    public class clawMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
 

        //health bar
        Texture2D mHealthBar;
        double mCurrentHealth = 100.0;
        Texture2D healthText;

        //farseer variables
        World world;
        Body body;
        List<DrawablePhysicsObject> crateList;
        List<DrawablePhysicsObject> rubbleList;
        DrawablePhysicsObject floor;
        Random random;
        Texture2D crateImg;
        Texture2D rubbleImg;
        Texture2D floorImg;
        


        //screen textures
        Texture2D mTitleScreenBackground;

        //Screen state variables to indicate what is the current screen;
        bool mIsTitleScreenShown;
        bool startGame = false;

 
        Player player1;
        Controls controls;
        private Texture2D background;
        double spawnTimer;
        double spawnDelay = 0.0; //seconds

        double crateSpawnTimer;
        double crateSpawnDelay = 5.0; //seconds




        public clawMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

         

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            int viewWidth;
            viewWidth = GraphicsDevice.Viewport.Width;
            int viewHeight;
            viewHeight = GraphicsDevice.Viewport.Height;

            player1 = new Player(370, 400, 50, 50, viewWidth);

            base.Initialize();


            controls = new Controls();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player1.LoadContent(this.Content);
            
            //farseer world
            world = new World(new Vector2(0, 9.8f));

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("spacebg.jpg");
            mHealthBar = Content.Load<Texture2D>("healthbar_temp3.png");
            healthText = Content.Load<Texture2D>("health text.png");
            mTitleScreenBackground = Content.Load<Texture2D>("startscreenop2.3.png");
            mIsTitleScreenShown = true;

            crateImg = Content.Load<Texture2D>("Crate.png");
            rubbleImg = Content.Load<Texture2D>("Rubble.png");
            floorImg = Content.Load<Texture2D>("Floor");
           
       
            random = new Random();

            floor = new DrawablePhysicsObject(world, floorImg, new Vector2(GraphicsDevice.Viewport.Width, 40.0f), 1000.0f);
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height-20);
            floor.body.BodyType = BodyType.Static;
            crateList = new List<DrawablePhysicsObject>();
            rubbleList = new List<DrawablePhysicsObject>();
        }

        private void SpawnRubble()
        {
            DrawablePhysicsObject rubble;
            rubble = new DrawablePhysicsObject(world, rubbleImg, new Vector2(50.0f, 50.0f), 0.1f);
            rubble.Position = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            rubble.body.LinearDamping = 30;
            // rubble.body.GravityScale = 0.00f;
            rubbleList.Add(rubble);

        }
        private void SpawnCrate()
        {
            DrawablePhysicsObject crate;
            crate = new DrawablePhysicsObject(world, crateImg, new Vector2(50.0f, 50.0f), 0.1f);
            crate.Position = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            crate.body.LinearDamping = 30;
            // crate.body.GravityScale = 0.00f;
            crateList.Add(crate);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

  
        private void UpdateTitleScreen()
        {
            
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                startGame = true;
                mIsTitleScreenShown = false;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //set our keyboardstate tracker update can change the gamestate on every cycle
            controls.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (mIsTitleScreenShown)
            {
                UpdateTitleScreen();
                return;
            }

            else if (startGame)
            {
                 // TODO: Add your update logic here
                mCurrentHealth -= 0.005;

                spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                crateSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (spawnTimer >= spawnDelay)
                {
                    
                    spawnTimer -= spawnDelay; //subtract used time
                    SpawnRubble();

                    double numgen = Shared.Random.NextDouble();
                    double delay = 10.0 * numgen;
                    if (delay > 3)
                    {
                        delay /= 2;
                    }
                    spawnDelay = delay;
                }

                
                if (crateSpawnTimer >= crateSpawnDelay)
                {

                    crateSpawnTimer -= crateSpawnDelay; //subtract used time
                    SpawnCrate();

                    double delay = 10.0;
                  
                    crateSpawnDelay = delay;
                }
                player1.Update(controls, gameTime);

                //removes rubble
                for (int i = rubbleList.Count - 1; i >= 0; i--)
                {

                    if (rubbleList[i].Position.Y >= 410)
                    {
                        rubbleList[i].Destroy();
                        rubbleList.RemoveAt(i);
                    }

                }

                //removes crates
                for (int j = crateList.Count - 1; j >= 0; j--)
                {
 
                    if (crateList[j].Position.Y >= 410)
                    {
                        crateList[j].Destroy();
                        crateList.RemoveAt(j);                    
                    }
                    
                }


            
            
                }
                world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                base.Update(gameTime);           
        }
 
        private void DrawTitleScreen()
        {
            spriteBatch.Draw(mTitleScreenBackground, Vector2.Zero, Color.White);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            
 
            // TODO: Add your drawing code here
            

           //based on screen state variables, call the Draw method associated with the current screen
           if(mIsTitleScreenShown)
           {
               DrawTitleScreen();
               spriteBatch.End();
               return;
           }
           else if(startGame)
           {

               //background
               spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);

               //health text
               spriteBatch.Draw(healthText, new Rectangle(10, 5, healthText.Bounds.Width, healthText.Bounds.Height), Color.White);

               //draw the negative space for the health bar
               spriteBatch.Draw(mHealthBar, new Rectangle(this.Window.ClientBounds.Width / 5 + 4 - mHealthBar.Width / 2,
                   30, mHealthBar.Width, 30), new Rectangle(0, 30, mHealthBar.Width, 30), Color.Gray);
               //draw the current health level based on the current Health
               spriteBatch.Draw(mHealthBar, new Rectangle((this.Window.ClientBounds.Width / 5 + 4 - mHealthBar.Width / 2),
                   30, (int)(mHealthBar.Width * ((double)mCurrentHealth / 100)), 30), new Rectangle(0, 30, mHealthBar.Width, 30), Color.Red);

               //draw box around health bar
               spriteBatch.Draw(mHealthBar, new Rectangle(this.Window.ClientBounds.Width / 5 + 4 - mHealthBar.Width / 2,
                   30, mHealthBar.Width, 30), new Rectangle(0, 0, mHealthBar.Width, 30), Color.White);



               foreach (DrawablePhysicsObject rubble in rubbleList)
               {
                   rubbleImg = Content.Load<Texture2D>("Rubble.png");
                   rubble.Draw(spriteBatch);
               }

               foreach (DrawablePhysicsObject crate in crateList)
               {
                   crateImg = Content.Load<Texture2D>("Crate.png");
                   crate.Draw(spriteBatch);
               }

               floor.Draw(spriteBatch);
               player1.Draw(spriteBatch);
                


               base.Draw(gameTime);
               spriteBatch.End();
           }
        }
    }

}

