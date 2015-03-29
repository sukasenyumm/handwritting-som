using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Leap;

namespace Tugas
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Texture2D _whiteSquare;                 //the white 64x64 pixels bitmap to draw with
        SpriteFont _defaultFont;                //font to write info with
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector2 _boardPosition;                 //where to position the board
        const int _tileSize = 80;               //how wide/tall the tiles are
        Texture2D textureObj;                   //pointr for leap motion controller
        GameObject finger;                      //object finger
        Controller leapController;              //LMC controller
        SingleListener leapListener;            //listener for LMC
        Vector2 _draggableSquarePosition;       //the draggable tile
        Rectangle _draggableSquareBorder;       //the boundaries of the draggable tile
        Vector2 _leapDownPosition;             //where the mouse was clicked down

        readonly bool[,] _board = new bool[7, 7];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //set the screen size
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 900;

            //positions the top left corner of the board - change this to move the board
            _boardPosition = new Vector2(100, 75);

            //positions the square to drag
            _draggableSquarePosition = new Vector2(800, 100);

            //show the mouse
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            leapController = new Controller();
            leapListener = new SingleListener();
            leapController.AddListener(leapListener);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load the textures
            _whiteSquare = Content.Load<Texture2D>("white_64x64");
            textureObj = Content.Load<Texture2D>("pointer");

            //load the font
            _defaultFont = Content.Load<SpriteFont>("DefaultFont");

            //instance finger
            finger = new GameObject(textureObj, Vector2.Zero);

            //remembers the draggable squares position, so we can easily test for mouseclicks on it
            _draggableSquareBorder = new Rectangle((int)_draggableSquarePosition.X, (int)_draggableSquarePosition.Y, _tileSize, _tileSize);

            //finger.color = Color.Snow;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (leapListener != null)
            {
                foreach (FingerPointStorage f in leapListener.fingerPoint)
                {
                    if (f.isActive)
                    {
                        float xObj = f.g_X * graphics.PreferredBackBufferWidth;
                        float yObj = f.g_Y * graphics.PreferredBackBufferHeight;
                        finger.Position.X = (int)xObj;
                        finger.Position.Y = (int)yObj;  

                        if(finger.Position.Length() > 10 &&_draggableSquareBorder.Contains((int)_leapDownPosition.X, (int)_leapDownPosition.Y))
                    }
                }
            }
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //start drawing
            spriteBatch.Begin();

            DrawText();             //draw helptext
            DrawBoard();            //draw the board
            finger.Draw(spriteBatch,1f);
            //end drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawText()
        {
            spriteBatch.DrawString(_defaultFont, "Handwritting Recognizer SOM", new Vector2(100, 20), Color.White);
            spriteBatch.DrawString(_defaultFont, "Drag new tile onto board with leap controller", new Vector2(100, 660), Color.White);
            spriteBatch.DrawString(_defaultFont, "@sukasenyumm", new Vector2(725, 665), Color.White);
        }

        // Draws the game board
        private void DrawBoard()
        {
            float opacity = 0.33f;                                      //how opaque/transparent to draw the square
            Color colorToUse = Color.White;                     //background color to use
            Rectangle squareToDrawPosition = new Rectangle();   //the square to draw (local variable to avoid creating a new variable per square)

            //for all columns
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                //for all rows
                for (int y = 0; y < _board.GetLength(1); y++)
                {

                    //figure out where to draw the square
                    squareToDrawPosition = new Rectangle((int)(x * _tileSize + _boardPosition.X), (int)(y * _tileSize + _boardPosition.Y), _tileSize, _tileSize);


                    Console.WriteLine("{0},{1}", squareToDrawPosition.X, squareToDrawPosition.Y);
                    //the code below will make the board checkered using only a single, white square:

                    //if we add the x and y value of the tile
                    //and it is even, we make it one third opaque
                    if ((x + y) % 2 == 0)
                    {
                        opacity = .33f;
                    }
                    else
                    {
                        //otherwise it is one tenth opaque
                        opacity = .1f;
                    }

                    //draw the white square at the given position, offset by the x- and y-offset, in the opacity desired
                    spriteBatch.Draw(_whiteSquare, squareToDrawPosition, colorToUse * opacity);

                    
                }

            }
        }

    }
}
