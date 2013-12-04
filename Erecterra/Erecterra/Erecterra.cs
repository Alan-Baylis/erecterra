#region Usings

using System;
using System.Collections.Generic;
using Lang.Erecterra.Components;
using Lang.Erecterra.Objects;
using Lang.Erecterra.Objects.UIObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Lang.Erecterra
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Erecterra : Game
    {

        #region Fields

        /// <summary>
        /// Window width
        /// </summary>
        public static int WIDTH = 1680;

        /// <summary>
        /// Window height
        /// </summary>
        public static int HEIGHT = 900;

        /// <summary>
        /// Last called event
        /// </summary>
        private int m_LastCalled;

        /// <summary>
        /// Graphics device manager
        /// </summary>
        private GraphicsDeviceManager m_Graphics;

        /// <summary>
        /// World object
        /// </summary>
        private World m_World;

        /// <summary>
        /// Previous Keyboard state
        /// </summary>
        private KeyboardState prevKeyboardState = Keyboard.GetState();

        /// <summary>
        /// The effect the shader uses
        /// </summary>
        private Effect m_Effect;

        /// <summary>
        /// World Translation Matrix
        /// </summary>
        private Matrix m_WorldTranslation;

        /// <summary>
        /// World Rotation Matrix
        /// </summary>
        private Matrix m_WorldRotation;

        /// <summary>
        /// Camera object
        /// </summary>
        private CameraManager m_Camera;

        /// <summary>
        /// InputManager
        /// </summary>
        private InputManager m_Keyboard;

        /// <summary>
        /// UIManager
        /// </summary>
        private UIManager m_UIManager;

        /// <summary>
        /// SpriteBatch for the UIManager
        /// </summary>
        private SpriteBatch m_SpriteBatch;

        /// <summary>
        /// Indicates whether the game has started
        /// </summary>
        private bool m_GameStarted;

        /// <summary>
        /// List of worlds loaded from the database
        /// </summary>
        private List<World> m_Worlds;

        /// <summary>
        /// Takes care of frame rate calculation
        /// </summary>
        private FrameRateManager m_FrameRate;

        /// <summary>
        /// Takes care of lighting
        /// </summary>
        private LightingManager m_LightingManager;

        /// <summary>
        /// Whether or not wireframe mode is enabled
        /// </summary>
        private bool m_Wireframe;

        /// <summary>
        /// The type of vertices to draw with
        /// </summary>
        public static QuadrantVerticeType VertexType = QuadrantVerticeType.VERTEX_POSITION_COLOR;

        #endregion

        #region Properties

        /// <summary>
        /// Whether or not the game has started
        /// </summary>
        public bool GameStarted
        {
            get
            {
                return m_GameStarted;
            }
        }

        /// <summary>
        /// UIManager
        /// </summary>
        public UIManager UIManager
        {
            get
            {
                return m_UIManager;
            }
        }

        /// <summary>
        /// InputManager
        /// </summary>
        public InputManager InputManager
        {
            get
            {
                return m_Keyboard;
            }
        }

        /// <summary>
        /// World Translation Matrix
        /// </summary>
        public Matrix WorldTranslation
        {
            get
            {
                return m_WorldTranslation;
            }

            set
            {
                m_WorldTranslation = value;
            }
        }

        /// <summary>
        /// World Rotation Matrix
        /// </summary>
        public Matrix WorldRotation
        {
            get
            {
                return m_WorldRotation;
            }

            set
            {
                m_WorldRotation = value;
            }
        }

        /// <summary>
        /// Camera
        /// </summary>
        public CameraManager Camera
        {
            get
            {
                return m_Camera;
            }

            set
            {
                m_Camera = value;
            }
        }

        #endregion

        #region Constructor

        public Erecterra()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.PreferredBackBufferWidth = WIDTH;
            m_Graphics.PreferredBackBufferHeight = HEIGHT;
            Content.RootDirectory = "Content";
            m_LastCalled = 0;
            m_GameStarted = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_Keyboard = new InputManager(this, m_Graphics);
            Components.Add(m_Keyboard);

            m_Camera = new CameraManager(this, new Vector3(0, 10, 100), Vector3.Zero, m_Keyboard);
            Components.Add(m_Camera);

            m_SpriteBatch = new SpriteBatch(m_Graphics.GraphicsDevice);
            m_UIManager = new UIManager(this, m_SpriteBatch);
            Components.Add(m_UIManager);

            m_FrameRate = new FrameRateManager(this);
            Components.Add(m_FrameRate);

            m_LightingManager = new LightingManager(this, m_Keyboard);
            Components.Add(m_LightingManager);

            Subscribe();

            m_WorldTranslation = Matrix.Identity;
            m_WorldRotation = Matrix.Identity;

            base.Initialize();
        }

        /// <summary>
        /// Subscribe to various sources
        /// </summary>
        private void Subscribe()
        {
            m_UIManager.OnMousePressedEvent += OnMousePressed;
            m_Keyboard.OnFunctionElevenPressed += FullScreenToggle;
            m_Keyboard.OnSaveWorld += SaveWorld;
            m_Keyboard.OnWireframeToggle += WireframeToggle;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {            
            m_Effect = Content.Load<Effect>(@"Effects\ColorEffect");
            
            //Set Cullmode
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            UIMenu menu = m_UIManager.GetMenu("StartMenu");
            if ( menu != null )
                menu.IsVisible = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            foreach ( GameComponent compontent in Components )
                compontent.Update(gameTime);

            if ( m_World != null )
            {
                UIMenu menu = m_UIManager.GetMenu("Information");
                if ( menu != null )
                {
                    if ( menu.MenuItems.Count > 0 )
                    {
                        UIText text = menu.MenuItems[0] as UIText;
                        if ( text != null )
                        {
                            text.Text = "Quadrant Count: " + Quadrant.QuadrantCount.ToString();
                        }

                        text = menu.MenuItems[1] as UIText;
                        if ( text != null )
                        {
                            if ( m_World.QuadrantGenerating )
                            {
                                text.Text = "Generating quadrant...";
                            }
                            else
                            {
                                text.Text = "";
                            }
                        }

                        text = menu.MenuItems[2] as UIText;
                        if ( text != null )
                            text.Text = "FPS: " + m_FrameRate.FrameRate.ToString();

                        text = menu.MenuItems[3] as UIText;
                        if ( text != null )
                            text.Text = string.Format("[X = {0}k, Y = {1}k, Z = {2}k]", Math.Round(m_Camera.Position.X / ( Quadrant.QuadrantSize * World.Scale ), 1), Math.Round(m_Camera.Position.Y / ( Quadrant.QuadrantSize * World.Scale ), 1), Math.Round(m_Camera.Position.Z / ( Quadrant.QuadrantSize * World.Scale ), 1));

                        text = menu.MenuItems[4] as UIText;
                        if ( text != null )
                            text.Text = m_LightingManager.ToTimeString();

                        text = menu.MenuItems[5] as UIText;
                        if ( text != null )
                            text.Text = Quadrant.AverageGenerationTime().ToString();
                    }
                }

                m_World.Update(GraphicsDevice, m_Camera);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if ( m_GameStarted )
            {
                GraphicsDevice.Clear(m_LightingManager.SkyColor);
            }
            else
            {
                GraphicsDevice.Clear(Color.LightGray);
            }
            m_Effect.CurrentTechnique = m_Effect.Techniques["Technique1"];
            m_Effect.Parameters["World"].SetValue(Matrix.Identity);
            m_Effect.Parameters["View"].SetValue(m_Camera.View);
            m_Effect.Parameters["Projection"].SetValue(m_Camera.Projection);

            // We set these values because they change depending on the time of the day
            m_Effect.Parameters["AmbientColor"].SetValue(m_LightingManager.AmbientColor.ToVector4());
            m_Effect.Parameters["AmbientIntensity"].SetValue(m_LightingManager.AmbientIntensity);
            m_Effect.Parameters["SunDiffuseColor"].SetValue(LightingManager.SunDiffuse.ToVector4());
            m_Effect.Parameters["SunDiffuseIntensity"].SetValue(LightingManager.SunIntensity);
            m_Effect.Parameters["SunDiffuseLightDirection"].SetValue(m_LightingManager.SunPosition);
            m_Effect.Parameters["MoonDiffuseColor"].SetValue(LightingManager.MoonDiffuse.ToVector4());
            m_Effect.Parameters["MoonDiffuseIntensity"].SetValue(LightingManager.MoonIntensity);
            m_Effect.Parameters["MoonDiffuseLightDirection"].SetValue(m_LightingManager.MoonPosition);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if ( m_Wireframe )
            {
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                rs.FillMode = FillMode.WireFrame;
                GraphicsDevice.RasterizerState = rs;
            }

            if ( m_GameStarted )
            {                
                IsMouseVisible = false;

                foreach ( EffectPass pass in m_Effect.CurrentTechnique.Passes )
                {
                    pass.Apply();
                    m_World.Draw(GraphicsDevice, m_Camera);
                }

                m_LightingManager.Draw(GraphicsDevice, m_Effect, Matrix.Identity);
                m_FrameRate.DrawCallMade();
            }
            else
            {
                IsMouseVisible = true;
            }

            m_UIManager.Draw();
        }

        /// <summary>
        /// Starts the gmae
        /// </summary>
        public void StartGame()
        {
            UIMenu menu = m_UIManager.GetMenu("StartMenu");
            if ( menu != null )
                menu.IsVisible = false;

            menu = m_UIManager.GetMenu("Information");
            if ( menu != null )
                menu.IsVisible = true;

            m_GameStarted = true;
            m_World = new World(150);
        }

        /// <summary>
        /// Display the worls that have been saved
        /// </summary>
        public void DisplayWorlds()
        {
            UIMenu menu = m_UIManager.GetMenu("StartMenu");
            if ( menu != null )
                menu.IsVisible = false;

            m_Worlds = DatabaseManager.Instance.GetWorlds();
            menu = m_UIManager.GetMenu("LoadMenu");

            float x = ( WIDTH / 2.0f ) - 40.0f;
            float y = 400;

            if ( menu != null )
            {
                menu.IsVisible = true;

                foreach ( World w in m_Worlds )
                {
                    UIText text = new UIText(m_UIManager, new Vector3(x, y, 0), w.Name, 14, Color.Black);
                    menu.MenuItems.Add(text);
                    y += 40;
                }
            }
        }

        /// <summary>
        /// Loads a game
        /// </summary>
        public void LoadGame(string worldName)
        {
            UIMenu menu = m_UIManager.GetMenu("LoadMenu");
            if ( menu != null )
                menu.IsVisible = false;

            foreach ( World w in m_Worlds )
            {
                if ( w.Name == worldName )
                {
                    m_World = w;
                    m_World.Initialize();
                    break;
                }
            }

            menu = m_UIManager.GetMenu("Information");
            if ( menu != null )
                menu.IsVisible = true;

            m_GameStarted = true;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// On mouse pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnMousePressed(object sender, OnMenuItemPressedEventArgs args)
        {
            int time = (int)Math.Abs(Environment.TickCount);

            if ( time - m_LastCalled < 500 )
                return;

            m_LastCalled = time;
            if ( args.Name == "New World" )
            {
                StartGame();
            }
            else if ( args.Name == "Load World" )
            {
                DisplayWorlds();
            }
            else
            {
                if ( args.Name != null )
                {
                    LoadGame(args.Name);
                }
            }
        }

        /// <summary>
        /// Toggle full screen
        /// </summary>
        public void FullScreenToggle(object sender, InputEventArgs args)
        {
            if ( m_Graphics.IsFullScreen )
                m_Graphics.IsFullScreen = false;

            if ( !m_Graphics.IsFullScreen )
                m_Graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Save the currently loaded World
        /// </summary>
        public void SaveWorld(object sender, InputEventArgs args)
        {
            if ( m_World.Save() )
            {
                UIMenu menu = new UIMenu(m_UIManager, "World Saved");
                m_UIManager.AddMenu(menu);
                DeterioratingText text = new DeterioratingText(m_UIManager, new Vector3(10, 10, 0), "World Saved!", 14, 500.0f, Color.Black);
                menu.MenuItems.Add(text);
                menu.IsVisible = true;
            }
        }

        /// <summary>
        /// Toggle Wireframe mode
        /// </summary>
        public void WireframeToggle(object sender, InputEventArgs args)
        {
            m_Wireframe = !m_Wireframe;

            if ( !m_Wireframe )
            {
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.None;
                rs.FillMode = FillMode.Solid;
                GraphicsDevice.RasterizerState = rs;
            }
        }

        #endregion

    }
}
