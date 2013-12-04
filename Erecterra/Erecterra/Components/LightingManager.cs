#region Usings

using System;
using Lang.Erecterra.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Lang.Erecterra.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class LightingManager : GameComponent
    {

        #region Fields

        /// <summary>
        /// The color of the sun!
        /// </summary>
        public static Color SunDiffuse = new Color(0.98f, 1.0f, 0.53f, 1.0f);

        /// <summary>
        /// The sun's intensity
        /// </summary>
        public static float SunIntensity = 0.8f;

        /// <summary>
        /// The color of the sun!
        /// </summary>
        public static Color MoonDiffuse = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>
        /// The sun's intensity
        /// </summary>
        public static float MoonIntensity = 0.4f;

        /// <summary>
        /// The radius of the moon and sun
        /// </summary>
        public static float LightOrbitRadius = 20000.0f;

        /// <summary>
        /// Number of seconds in a day
        /// </summary>
        public static int SECONDS_IN_DAY = 86400;

        /// <summary>
        /// The time of seconds into the day at 6am
        /// </summary>
        public static int SIX_AM = 6 * 3600;

        /// <summary>
        /// The time of seconds into the day at 6pm
        /// </summary>
        public static int SIX_PM = 18 * 3600;

        /// <summary>
        /// The color of the ambient light
        /// </summary>
        private Color m_AmbientColor;

        /// <summary>
        /// The intensity of the ambient alight
        /// </summary>
        private float m_AmbientIntensity;

        /// <summary>
        /// The time of the day
        /// </summary>
        private float m_TimeOfDay;

        /// <summary>
        /// The game object
        /// </summary>
        public Erecterra m_Game;

        /// <summary>
        /// The object that represents the sun
        /// </summary>
        private Sphere m_Sun;

        /// <summary>
        /// The object that represents the sun
        /// </summary>
        private Sphere m_Moon;

        /// <summary>
        /// Color of the sky
        /// </summary>
        /// 100, 149, 237, 255
        public static Color Sky = new Color(100, 149, 237);

        /// <summary>
        /// Color of the sky depending on time
        /// </summary>
        private Color m_SkyColor;

        #endregion

        #region Properties

        /// <summary>
        /// Helper method for Ambient Color
        /// </summary>
        public Color AmbientColor
        {
            get
            {
                return m_AmbientColor;
            }

            set
            {
                m_AmbientColor = value;
            }
        }

        /// <summary>
        /// Helper method for Ambient Intensity
        /// </summary>
        public float AmbientIntensity
        {
            get
            {
                return m_AmbientIntensity;
            }

            set
            {
                m_AmbientIntensity = value;
            }
        }

        /// <summary>
        /// Position of the sun
        /// </summary>
        public Vector3 SunPosition
        {
            get
            {
                return m_Sun.Position;
            }

            set
            {
                m_Sun.Position = value;
            }
        }

        /// <summary>
        /// Position of the Moon
        /// </summary>
        public Vector3 MoonPosition
        {
            get
            {
                return m_Moon.Position;
            }

            set
            {
                m_Moon.Position = value;
            }
        }

        /// <summary>
        /// Get the color of the sky
        /// </summary>
        public Color SkyColor
        {
            get
            {
                return m_SkyColor;
            }
        }

        #endregion

        #region Constructor

        public LightingManager(Game game, InputManager inputManager)
            : base(game)
        {
            m_AmbientColor = new Color(1f, 1f, 1f, 1f);
            m_AmbientIntensity = 0.6f;
            m_TimeOfDay = 0;
            m_Game = ( (Erecterra)game );

            CameraManager camera = m_Game.Camera;
            m_Sun = new Sphere(new Vector3(camera.Position.X + LightOrbitRadius, camera.Position.Y, camera.Position.Z), 1000, Color.Yellow);
            m_Moon = new Sphere(new Vector3(camera.Position.X + LightOrbitRadius, camera.Position.Y, camera.Position.Z), 1000, Color.White);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert the time of day to a readable string
        /// </summary>
        /// <returns></returns>
        public string ToTimeString()
        {
            string mer = "AM";

            float currTime = m_TimeOfDay;
            int hours = (int)Math.Floor(currTime / 3600);

            currTime -= ( hours * 3600 );
            int min = (int)Math.Floor(currTime / 60);

            if ( hours > 12 )
            {
                mer = "PM";
                hours -= 12;
            }

            return String.Format("{0}:{1} {2}", ( hours < 9 ) ? "0" + hours.ToString() : hours.ToString(), ( min < 9 ) ? "0" + min.ToString() : min.ToString(), mer);
        }

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
            m_TimeOfDay += gameTime.ElapsedGameTime.Milliseconds;

            if ( m_TimeOfDay > SECONDS_IN_DAY )
                m_TimeOfDay = ( m_TimeOfDay - SECONDS_IN_DAY );

            float timeVal = (float)Math.Sin(( m_TimeOfDay / SECONDS_IN_DAY ) * (float)Math.PI);
            m_AmbientColor = new Color(timeVal, timeVal, timeVal, 1);
            m_SkyColor = new Color(( Sky.R * timeVal ) / 255.0f, ( Sky.G * timeVal ) / 255.0f, ( Sky.B * timeVal ) / 255.0f, 1.0f);

            UpdateSun();
            UpdateMoon();

            base.Update(gameTime);
        }

        /// <summary>
        /// Update the sun
        /// </summary>
        private void UpdateSun()
        {
            float xVal = (float)Math.Sin(( m_TimeOfDay / SECONDS_IN_DAY ) * MathHelper.Pi);
            float zVal = (float)Math.Cos(( m_TimeOfDay / SECONDS_IN_DAY ) * MathHelper.Pi);

            Vector3 movement = new Vector3(LightOrbitRadius, LightOrbitRadius * xVal, -LightOrbitRadius * zVal);
            m_Sun.Position = new Vector3(m_Game.Camera.Position.X, 0, m_Game.Camera.Position.Z);
            m_Sun.Position += movement;
        }

        /// <summary>
        /// Update the sun
        /// </summary>
        private void UpdateMoon()
        {
            if ( m_TimeOfDay <= SIX_AM )
            {
                m_Moon.Visible = true;

                float xVal = (float)Math.Sin(( ( m_TimeOfDay / SECONDS_IN_DAY ) * MathHelper.Pi ) + MathHelper.Pi * 2 / 3);
                float zVal = (float)Math.Cos(( ( m_TimeOfDay / SECONDS_IN_DAY ) * MathHelper.Pi ) + MathHelper.Pi * 2 / 3);

                Vector3 movement = new Vector3(LightOrbitRadius, LightOrbitRadius * xVal, -LightOrbitRadius * zVal);
                m_Moon.Position = new Vector3(m_Game.Camera.Position.X, 0, m_Game.Camera.Position.Z);
                m_Moon.Position += movement;
            }
            else if ( m_TimeOfDay >= SIX_PM )
            {
                m_Moon.Visible = true;

                float xVal = (float)Math.Sin(( ( m_TimeOfDay / SECONDS_IN_DAY ) * MathHelper.Pi ) - MathHelper.Pi * 2 / 3);
                float zVal = (float)Math.Cos(( ( m_TimeOfDay / SECONDS_IN_DAY ) * MathHelper.Pi ) - MathHelper.Pi * 2 / 3);

                Vector3 movement = new Vector3(LightOrbitRadius, LightOrbitRadius * xVal, -LightOrbitRadius * zVal);
                m_Moon.Position = new Vector3(m_Game.Camera.Position.X, 0, m_Game.Camera.Position.Z);
                m_Moon.Position += movement;
            }
            else
            {
                m_Moon.Visible = false;
            }
        }

        /// <summary>
        /// Draw the sun and the moon
        /// </summary>
        /// <param name="device"></param>
        /// <param name="effect"></param>
        /// <param name="world"></param>
        public void Draw(GraphicsDevice device, Effect effect, Matrix world)
        {
            //if ( m_Sun.Visible )
            //    m_Sun.Draw(device, effect);

            //if ( m_Moon.Visible )
            //    m_Moon.Draw(device, effect);

            effect.Parameters["World"].SetValue(world);
        }

        #endregion

    }
}
