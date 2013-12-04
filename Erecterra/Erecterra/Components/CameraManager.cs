#region Usings

using Microsoft.Xna.Framework;

#endregion

namespace Lang.Erecterra.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CameraManager : GameComponent
    {

        #region Fields

        /// <summary>
        /// The position of the camera
        /// </summary>
        private Vector3 m_Position;

        /// <summary>
        /// The direction the camera is looking
        /// </summary>
        private Vector3 m_Direction;

        /// <summary>
        /// The up direction
        /// </summary>
        private Vector3 m_Up;

        /// <summary>
        /// The forward direction of the camera
        /// </summary>
        private Vector3 m_Forward;

        /// <summary>
        /// The left direction of the camera
        /// </summary>
        private Vector3 m_Left;

        /// <summary>
        /// The view matrix
        /// </summary>
        private Matrix m_View;

        /// <summary>
        /// The projection matrix
        /// </summary>
        private Matrix m_Projection;

        /// <summary>
        /// The speed at which the camera moves
        /// </summary>
        public static float SPEED = 40.0f;

        /// <summary>
        /// The lowest the camera can be
        /// </summary>
        public static float MIN_Y = 20.0f;

        #endregion

        #region Properties

        public Vector3 Position
        {
            get
            {
                return m_Position;
            }

            set
            {
                m_Position = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return m_Direction;
            }

            set
            {
                m_Direction = value;
                m_Direction.Normalize();
            }
        }

        public Vector3 Up
        {
            get
            {
                return m_Up;
            }

            set
            {
                m_Up = value;
                m_Up.Normalize();
            }
        }

        public Vector3 Left
        {
            get
            {
                return m_Left;
            }

            set
            {
                m_Left = value;
                m_Left.Normalize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix View
        {
            get
            {
                return m_View;
            }

            set
            {
                m_View = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix Projection
        {
            get
            {
                return m_Projection;
            }

            set
            {
                m_Projection = value;
            }
        }

        #endregion

        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the Camera class
        /// </summary>
        /// <param name="game">Represents the game object</param>
        /// <param name="pos">The position of the camera</param>
        /// <param name="target">The direction the camera is looking</param>
        /// <param name="up">The up vector</param>
        public CameraManager(Game game, Vector3 pos, Vector3 target, InputManager inputManager)
            : base(game)
        {
            m_Position = pos;
            m_Direction = target - pos;
            m_Direction.Normalize();
            m_Up = Vector3.Up;
            m_Forward = Vector3.Forward;
            m_Left = Vector3.Left;

            Subscribe(inputManager);
            CreateLookAt();
            m_Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 0.1f, 25000f);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Subscribe to all events
        /// </summary>
        private void Subscribe(InputManager inputManager)
        {
            inputManager.OnForwardPressed += ForwardPressed;
            inputManager.OnBackwardPressed += BackwardPressed;
            inputManager.OnLeftPressed += LeftPressed;
            inputManager.OnRightPressed += RightPressed;
            inputManager.OnUpPressed += UpPressed;
            inputManager.OnDownPressed += DownPressed;
            inputManager.OnMouseMoved += MouseMoved;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if ( Position.Y < MIN_Y )
                Position = new Vector3(Position.X, MIN_Y, Position.Z);

            CreateLookAt();
            base.Update(gameTime);
        }

        /// <summary>
        /// Creates the LookAt matrix
        /// </summary>
        private void CreateLookAt()
        {
            m_View = Matrix.CreateLookAt(m_Position, m_Position + m_Direction, m_Up);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// When forward is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ForwardPressed(object sender, InputEventArgs args)
        {
            Position += m_Direction * SPEED;
        }

        /// <summary>
        /// When backward is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void BackwardPressed(object sender, InputEventArgs args)
        {
            Position -= m_Direction * SPEED;
        }

        /// <summary>
        /// When left is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void LeftPressed(object sender, InputEventArgs args)
        {
            Position += Vector3.Cross(m_Up, m_Direction) * SPEED;
        }

        /// <summary>
        /// When right is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RightPressed(object sender, InputEventArgs args)
        {
            Position -= Vector3.Cross(m_Up, m_Direction) * SPEED;
        }

        /// <summary>
        /// When up is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UpPressed(object sender, InputEventArgs args)
        {
            Position += Vector3.Up * SPEED;
        }

        /// <summary>
        /// When down is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DownPressed(object sender, InputEventArgs args)
        {
            Position -= Vector3.Up * SPEED;
        }

        /// <summary>
        /// When the mouse is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void MouseMoved(object sender, MouseMovedEventArgs args)
        {
            Quaternion q = Quaternion.CreateFromAxisAngle(m_Left, ( MathHelper.PiOver4 / 200 ) * args.yChange) * Quaternion.CreateFromAxisAngle(Vector3.Up, ( -MathHelper.PiOver4 / 200 ) * args.xChange);
            Direction = Vector3.Transform(m_Direction, q);
            Left = Vector3.Transform(m_Left, q);
        }

        #endregion

    }
}
