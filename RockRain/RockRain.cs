using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace RockRain
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RockRain : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private const int START_METEOR_COUNT = 10;
        private const int ADD_METEOR_TIME = 1000;

        private Texture2D _backgroundTexture;
        private Texture2D _rockRainTexture;
        private Ship _player;
        private int _lastTickCount;
        private int _rockCount;

        private SoundEffect _explosion;
        private SoundEffect _newMeteor;
        private Song _backMusic;

        private SpriteFont _gameFont;

        public RockRain()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 765;
            Content.RootDirectory = "Content";
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
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(_spriteBatch);

            _backgroundTexture = Content.Load<Texture2D>(@"images/SpaceBackground");
            _rockRainTexture = Content.Load<Texture2D>(@"images/RockRain");

            _explosion = Content.Load<SoundEffect>(@"audios/explosion");
            _newMeteor = Content.Load<SoundEffect>(@"audios/newmeteor");
            _backMusic = Content.Load<Song>(@"audios/backmusic");

            MediaPlayer.Play(_backMusic);

            _gameFont = Content.Load<SpriteFont>(@"fonts/font");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            _backgroundTexture.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if(_player == null)
            {
                Start();
            }

            DoGameLogic();
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
            _spriteBatch.Begin();
            _spriteBatch.Draw(_backgroundTexture,
                new Rectangle(0, 0, _graphics.GraphicsDevice.DisplayMode.Width, _graphics.GraphicsDevice.DisplayMode.Height),
                Color.LightGray);

            _spriteBatch.DrawString(_gameFont, "Rocks: " + _rockCount.ToString(),
                new Vector2(15, 15), Color.YellowGreen);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            base.Draw(gameTime);
            _spriteBatch.End();
        }

        private void Start()
        {
            _lastTickCount = System.Environment.TickCount;
            _rockCount = START_METEOR_COUNT;

            if (_player == null)
            {
                _player = new Ship(this, ref _rockRainTexture);
                Components.Add(_player);
            }

            _player.PutInStartPosition();

            for (int i = 0; i < START_METEOR_COUNT; i++)
            {
                AddNewMeteor();
            }
        }

        private void DoGameLogic()
        {
            bool hasCollision = false;
            Rectangle shipRectangle = _player.GetBounds();
            foreach (GameComponent gc in Components)
            {
                if(gc is Meteor)
                {
                    hasCollision = ((Meteor)gc).CheckCollision(shipRectangle);
                    if (hasCollision)
                    {
                        RemoveAllMeteors();
                        _explosion.Play();
                        Start();

                        break;
                    }
                }
            }
            CheckForNewMeteor();
        }

        private void RemoveAllMeteors()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if(Components[i] is Meteor)
                {
                    Components.RemoveAt(i);
                    i--;
                }
            }
        }

        private void CheckForNewMeteor()
        {
            if((System.Environment.TickCount - _lastTickCount) > ADD_METEOR_TIME)
            {
                _lastTickCount = System.Environment.TickCount;
                AddNewMeteor();
                _rockCount++;
                _newMeteor.Play();
            }
        }

        private void AddNewMeteor() =>
            Components.Add(new Meteor(this, ref _rockRainTexture));
    }
}
