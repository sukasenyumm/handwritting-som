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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        Texture2D textureRecog;
        GameObject finger;                      //object finger
        GameObject btnRecog;
        Controller leapController;              //LMC controller
        SingleListener leapListener;            //listener for LMC
        Vector2 leapDownPosition;             //where the mouse was clicked down
        float xObj;                             //get x pos for leap
        float yObj;                             //get x pos for leap
        float indicator;                        //get depth/z pos for leap

        readonly bool[,] _board = new bool[7, 7];
        //initialize matrix for preview
        int[] objectVector;
        TimeSpan elapTime = TimeSpan.Zero;
        
        string recognizeRes;
        string learningRes;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //set the screen size
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 900;

            //positions the top left corner of the board - change this to move the board
            _boardPosition = new Vector2(100, 75);

            
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

            //initialize vector
            objectVector = new int[_board.Length+1];
            objectVector[0] = 9999;

           

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
            textureRecog = Content.Load<Texture2D>("recognize");

            //load the font
            _defaultFont = Content.Load<SpriteFont>("DefaultFont");

            //instance finger
            finger = new GameObject(textureObj, Vector2.Zero);
            btnRecog = new GameObject(textureRecog, Vector2.Zero);

            //remembers the draggable squares position, so we can easily test for mouseclicks on it
          
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
                        xObj = f.g_X * graphics.PreferredBackBufferWidth;
                        yObj = f.g_Y * graphics.PreferredBackBufferHeight;
                        finger.Position.X = (int)xObj;
                        finger.Position.Y = (int)yObj;
                        //mengambil posisi z sebagai deteksi kedalaman
                        if (finger.BoundingBox.Intersects(btnRecog.BoundingBox))
                        {
                            btnRecog.color = Color.SlateGray;
                            btnRecog.Position = new Vector2(btnRecog.Position.X + 10, btnRecog.Position.Y + 10);
                            finger.color = Color.Black;
                            elapTime += gameTime.ElapsedGameTime;
                            if (elapTime > TimeSpan.FromSeconds(1.2))
                            {
                                
                                var sourcePath = "hwData.csv";
                                var delimiter = ",";
                                var firstLineContainsHeaders = true;
                                var tempPath = "data.csv";
                                var lineNumber = 0;

                                var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");

                                using (var writer = new StreamWriter(tempPath))
                                using (var reader = new StreamReader(sourcePath))
                                {
                                    string line = null;
                                    string[] headers = null;
                                    if (firstLineContainsHeaders)
                                    {
                                        line = reader.ReadLine();
                                        lineNumber++;

                                        if (string.IsNullOrEmpty(line)) return; // file is empty;

                                        headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                                        writer.WriteLine(line); // write the original header to the temp file.
                                    }

                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        lineNumber++;

                                        var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                                        // if there are no headers, do a simple sanity check to make sure you always have the same number of columns in a line
                                        if (headers == null) headers = new string[columns.Length];

                                        if (columns.Length != headers.Length) throw new InvalidOperationException(string.Format("Line {0} is missing one or more columns.", lineNumber));

                                        // TODO: search and replace in columns
                                        // example: replace 'v' in the first column with '\/': if (columns[0].Contains("v")) columns[0] = columns[0].Replace("v", @"\/");

                                        writer.WriteLine(string.Join(delimiter, columns));
                                    }

                                }

                                File.Delete(sourcePath);
                                File.Move(tempPath, sourcePath);
                                StringBuilder sb = new StringBuilder();
                                for (int i = 0; i < 49; i++)
                                    sb.AppendFormat("{0},", objectVector[i]);

                                sb.AppendFormat("{0}", objectVector[49]);
                                // flush all rows once time.
                                File.AppendAllText(sourcePath, sb.ToString(), Encoding.UTF8);
                                Map ma = new Map(49, 7, sourcePath);
                                recognizeRes = ma.getResult();
                                learningRes = ma.getLearningTime();
                                elapTime = TimeSpan.Zero;
                            }
                        }

                        //find out which square the mouse is over
                        Vector3 tile = new Vector3(GetSquareFromCurrentMousePosition(), 0);

                        if (f.g_Z > 0)
                        {
                            indicator = 150 - f.g_Z;
                            if (indicator > 50)
                            {
                                leapDownPosition = new Vector2((int)xObj, (int)yObj);
                                
                                //if the mousebutton was released inside the board
                                if (IsMouseInsideBoard())
                                {
                                    //and set that square to true (has a piece)
                                    _board[(int)tile.X, (int)tile.Y] = true;
                                    Console.WriteLine("{0},{1}", (int)tile.X, (int)tile.Y);
                                    int count = 1;
                                    for (int i = 0; i < 7; i++)
                                    {
                                        for (int j = 0; j < 7; j++)
                                        {

                                            if (tile.X == j && tile.Y == i)
                                            {
                                                objectVector[count] = 1;
                                                Console.WriteLine("count {0}" + count);
                                                count = 1;
                                            }
                                            else
                                            { 
                                                count++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (IsMouseInsideBoard())
                                {
                                    _board[(int)tile.X, (int)tile.Y] = false;
                                    int count = 1;
                                    for (int i = 0; i < 7; i++)
                                    {
                                        for (int j = 0; j < 7; j++)
                                        {

                                            if (tile.X == j && tile.Y == i)
                                            {
                                                objectVector[count] = 0;
                                                Console.WriteLine("count {0}" + count);
                                                count = 1;
                                            }
                                            else
                                            {
                                                count++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
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
            GraphicsDevice.Clear(Color.DarkOrchid);

            // TODO: Add your drawing code here
            //start drawing
            spriteBatch.Begin();

            DrawText();             //draw helptext
            DrawBoard();            //draw the board
            btnRecog.ValPosition = new Vector2(graphics.PreferredBackBufferWidth - btnRecog.getTexture().Width,20);
            
            btnRecog.Draw(spriteBatch, 1f);
            finger.Draw(spriteBatch, 1f);
            
            string strTimeWait = string.Format("{0}%", (Math.Floor(elapTime.TotalSeconds * 10 / 12 * 100)).ToString());
            spriteBatch.DrawString(_defaultFont, strTimeWait, new Vector2(finger.Position.X + finger.BoundingBox.Width / 2, finger.Position.Y + finger.BoundingBox.Height / 2), Color.Black);

            if(learningRes != null)
            spriteBatch.DrawString(_defaultFont, learningRes, new Vector2(btnRecog.Position.X, btnRecog.Position.Y + btnRecog.BoundingBox.Height + 10), Color.Black);
           
            if(recognizeRes != null)
            spriteBatch.DrawString(_defaultFont, recognizeRes, new Vector2(btnRecog.Position.X, btnRecog.Position.Y + btnRecog.BoundingBox.Height + 30), Color.Black);
            
            //end drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private int CountLinesInString(string s)
        {
            int count = 1;
            int start = 0;
            while ((start = s.IndexOf('\n', start)) != -1)
            {
                count++;
                start++;
            }
            return count;
        }
        private void DrawText()
        {
            spriteBatch.DrawString(_defaultFont, "'I' 'Love' 'U' Character Recognition", new Vector2(100, 20), Color.White);
            spriteBatch.DrawString(_defaultFont, "fly in and out your hand onto board with leap controller\nhttps://github.com/sukasenyumm/handwritting-som", new Vector2(100, 640), Color.White);
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


                    //Console.WriteLine("{0},{1}", squareToDrawPosition.X, squareToDrawPosition.Y);
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

                    if (IsMouseInsideBoard() && IsMouseOnTile(x, y))
                    {
                        colorToUse = Color.Red;
                        opacity = .5f;
                    }
                    else
                    {
                        colorToUse = Color.White;
                    }

                    //draw the white square at the given position, offset by the x- and y-offset, in the opacity desired
                    spriteBatch.Draw(_whiteSquare, squareToDrawPosition, colorToUse * opacity);

                    //if the square has a tile - draw it
                    if (_board[x, y])
                    {
                        spriteBatch.Draw(_whiteSquare, squareToDrawPosition, Color.Black);
                    }
                    
                }

            }
        }

        bool IsMouseInsideBoard()
        {
            if (xObj >= _boardPosition.X && xObj <= _boardPosition.X + _board.GetLength(0) * _tileSize && yObj >= _boardPosition.Y && yObj <= _boardPosition.Y + _board.GetLength(1) * _tileSize)
            {
                return true;
            }
            else
            { return false; }
        }
        Vector2 GetSquareFromCurrentMousePosition()
        {
            //adjust for the boards offset (_boardPosition) and do an integerdivision
            return new Vector2((int)(xObj - _boardPosition.X) / _tileSize, (int)(yObj - _boardPosition.Y) / _tileSize);
        }
        // Checks to see whether a given coordinate is within the board
        private bool IsMouseOnTile(int x, int y)
        {
            //do an integerdivision (whole-number) of the coordinates relative to the board offset with the tilesize in mind
            return (int)(xObj - _boardPosition.X) / _tileSize == x && (int)(yObj - _boardPosition.Y) / _tileSize == y;
        }
    }
}
