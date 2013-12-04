#region Usings

using System;
// Visual Basic used for Prompt
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Lang.Erecterra.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputManager : GameComponent
    {

        #region Fields

        /// <summary>
        /// Keyboard state last update
        /// </summary>
        private KeyboardState m_prevKeyboardState;

        /// <summary>
        /// Mouse state last update
        /// </summary>
        private MouseState m_prevMouseState;

        /// <summary>
        /// The current game object
        /// </summary>
        private Game m_Game;

        /// <summary>
        /// GraphicsDeviceManager
        /// </summary>
        private GraphicsDeviceManager m_Manager;

        /// <summary>
        /// How often someone can save
        /// </summary>
        private float m_SaveThreshold;

        /// <summary>
        /// How often someone can change wireframe mode
        /// </summary>
        private float m_WireframeThreshold;

        /// <summary>
        /// How often someone can unlock the mouse
        /// </summary>
        private float m_UnlockThreshold;

        /// <summary>
        /// Whether or not the mouse should be locked to the screen
        /// </summary>
        private bool m_MouseUnlocked;

        #endregion

        #region Constructor

        public InputManager(Game game, GraphicsDeviceManager manager)
            : base(game)
        {
            m_Game = game;
            m_Manager = manager;
            m_SaveThreshold = 0f;
            m_WireframeThreshold = 0f;

            Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
            m_prevMouseState = Mouse.GetState();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Shows a dialog to enter a name for saving a world
        /// </summary>
        /// <returns></returns>
        public static string ShowDialog()
        {
            return Interaction.InputBox("Enter a name for the World.", "Save World");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            // If the user presses the escape key, close the game
            if ( keyboard.IsKeyDown(Keys.Escape) )
                m_Game.Exit();

            if ( keyboard.IsKeyDown(Keys.F11) )
                FunctionElevenPressed();

            if ( keyboard.IsKeyDown(Keys.LeftControl) && keyboard.IsKeyDown(Keys.S) && m_SaveThreshold <= 0.0f )
            {
                SaveWorldPressed();
                m_SaveThreshold = 10.0f;
            }

            if ( keyboard.IsKeyDown(Keys.LeftControl) && keyboard.IsKeyDown(Keys.W) && m_WireframeThreshold <= 0.0f )
            {
                WireframeToggle();
                m_WireframeThreshold = 10.0f;
            }

            if ( keyboard.IsKeyDown(Keys.LeftControl) && keyboard.IsKeyDown(Keys.U) && m_UnlockThreshold <= 0.0f )
            {
                MouseUnlockToggle();
                m_UnlockThreshold = 10.0f;
            }

            if ( m_SaveThreshold > 0 )
                m_SaveThreshold--;

            if ( m_WireframeThreshold > 0 )
                m_WireframeThreshold--;

            if ( m_UnlockThreshold > 0 )
                m_UnlockThreshold--;

            if ( keyboard.IsKeyDown(Keys.W) )
                ForwardPressed();

            if ( keyboard.IsKeyDown(Keys.S) )
                BackwardPressed();

            if ( keyboard.IsKeyDown(Keys.A) )
                LeftPressed();

            if ( keyboard.IsKeyDown(Keys.D) )
                RightPressed();

            if ( keyboard.IsKeyDown(Keys.Space) )
                UpPressed();

            if ( keyboard.IsKeyDown(Keys.X) )
                DownPressed();

            if ( !m_MouseUnlocked )
            {
                MouseMovedEventArgs args = new MouseMovedEventArgs(Mouse.GetState().X - m_prevMouseState.X, Mouse.GetState().Y - m_prevMouseState.Y);
                if ( args.xChange != 0 || args.yChange != 0 )
                    MouseMoved(args);

                if ( ( (Erecterra)m_Game ).GameStarted )
                {
                    Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
                }
            }

            if ( m_prevMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released )
            {
                ( (Erecterra)m_Game ).UIManager.CheckPressed(m_prevMouseState);
            }

            // Cache the keyboard state for the next update cycle
            m_prevKeyboardState = keyboard;
            m_prevMouseState = Mouse.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// Toggle mouse unlock
        /// </summary>
        private void MouseUnlockToggle()
        {
            m_MouseUnlocked = !m_MouseUnlocked;

            if ( !m_MouseUnlocked )
                Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Notify subscribers that F11 was pressed
        /// </summary>
        public void FunctionElevenPressed()
        {
            if ( OnFunctionElevenPressed != null )
            {
                OnFunctionElevenPressed.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers that a save was completed
        /// </summary>
        public void SaveWorldPressed()
        {
            if ( OnSaveWorld != null )
            {
                OnSaveWorld.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers of a WireframeToggle
        /// </summary>
        public void WireframeToggle()
        {
            if ( OnWireframeToggle != null )
            {
                OnWireframeToggle.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers forward was pressed
        /// </summary>
        public void ForwardPressed()
        {
            if ( OnForwardPressed != null )
            {
                OnForwardPressed.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers backward was pressed
        /// </summary>
        public void BackwardPressed()
        {
            if ( OnBackwardPressed != null )
            {
                OnBackwardPressed.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers backward was pressed
        /// </summary>
        public void LeftPressed()
        {
            if ( OnLeftPressed != null )
            {
                OnLeftPressed.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers backward was pressed
        /// </summary>
        public void RightPressed()
        {
            if ( OnRightPressed != null )
            {
                OnRightPressed.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers backward was pressed
        /// </summary>
        public void UpPressed()
        {
            if ( OnUpPressed != null )
            {
                OnUpPressed.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribers backward was pressed
        /// </summary>
        public void DownPressed()
        {
            if ( OnDownPressed != null )
            {
                OnDownPressed.Invoke(this, new InputEventArgs());
            }
        }

        /// <summary>
        /// Notify subscribs the mouse moved
        /// </summary>
        /// <param name="args"></param>
        public void MouseMoved(MouseMovedEventArgs args)
        {
            if ( OnMouseMoved != null )
            {
                OnMouseMoved.Invoke(this, args);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// When F11 is pressed
        /// </summary>
        public EventHandler<InputEventArgs> OnFunctionElevenPressed;

        /// <summary>
        /// On a save attempt
        /// </summary>
        public EventHandler<InputEventArgs> OnSaveWorld;

        /// <summary>
        /// When wireframe mode toggle is run
        /// </summary>
        public EventHandler<InputEventArgs> OnWireframeToggle;

        /// <summary>
        /// When forward is pressed
        /// </summary>
        public EventHandler<InputEventArgs> OnForwardPressed;

        /// <summary>
        /// When backward is pressed
        /// </summary>
        public EventHandler<InputEventArgs> OnBackwardPressed;

        /// <summary>
        /// When left is pressed
        /// </summary>
        public EventHandler<InputEventArgs> OnLeftPressed;

        /// <summary>
        /// When right is pressed
        /// </summary>
        public EventHandler<InputEventArgs> OnRightPressed;

        /// <summary>
        /// When up is pressed
        /// </summary>
        public EventHandler<InputEventArgs> OnUpPressed;

        /// <summary>
        /// When down is pressed
        /// </summary>
        public EventHandler<InputEventArgs> OnDownPressed;

        /// <summary>
        /// When the mouse is moved
        /// </summary>
        public EventHandler<MouseMovedEventArgs> OnMouseMoved;

        #endregion

    }

    #region Input Event Arguments

    /// <summary>
    /// Special argument type for input events
    /// </summary>
    public class InputEventArgs : EventArgs
    {
        public InputEventArgs()
        {

        }
    }

    /// <summary>
    /// Special argument type for mouse movement
    /// </summary>
    public class MouseMovedEventArgs : EventArgs
    {
        public float xChange;
        public float yChange;

        /// <summary>
        /// Create MouseMovedEventArgs
        /// </summary>
        /// <param name="x">Distance the mouse has moved in the x direction</param>
        /// <param name="y">Distance the mouse has moved in the y direction</param>
        public MouseMovedEventArgs(float x, float y)
        {
            xChange = x;
            yChange = y;
        }
    }

    #endregion

}
