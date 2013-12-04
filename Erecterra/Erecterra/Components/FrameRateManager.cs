#region Usings

using System;
using Microsoft.Xna.Framework;

#endregion


namespace Lang.Erecterra.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FrameRateManager : GameComponent
    {

        #region Fields

        /// <summary>
        /// The frame rate
        /// </summary>
        private int m_FrameRate = 0;

        /// <summary>
        /// The frame counter
        /// </summary>
        private int m_FrameCounter = 0;

        /// <summary>
        /// Used to calculate the frame rate
        /// </summary>
        private TimeSpan m_ElapsedTime = TimeSpan.Zero;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the frame rate
        /// </summary>
        public int FrameRate
        {
            get
            {
                return m_FrameRate;
            }
        }

        /// <summary>
        /// Property for the FrameCounter
        /// </summary>
        public int FrameCounter
        {
            get
            {
                return m_FrameCounter;
            }

            set
            {
                m_FrameCounter = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// FrameRateManager constructor
        /// </summary>
        /// <param name="game"></param>
        public FrameRateManager(Game game)
            : base(game)
        {

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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            m_ElapsedTime += gameTime.ElapsedGameTime;

            if (m_ElapsedTime > TimeSpan.FromSeconds(1))
            {
                m_ElapsedTime -= TimeSpan.FromSeconds(1);
                m_FrameRate = m_FrameCounter;
                m_FrameCounter = 0;
            }
        }

        /// <summary>
        /// Call this every time a draw call is made
        /// </summary>
        public void DrawCallMade()
        {
            FrameCounter++;
        }

        #endregion
        
    }
}