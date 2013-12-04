#region Usings

using System;
using System.Xml;
using Lang.Erecterra.Components;
using Microsoft.Xna.Framework; 

#endregion

namespace Lang.Erecterra.Objects.UIObjects
{
    public class DeterioratingText : UIText
    {

        #region Fields

        /// <summary>
        /// number in milliseconds the text will dislay for
        /// </summary>
        private float m_lifeSpan;

        /// <summary>
        /// number in milliseconds the text has lived for
        /// </summary>
        private float m_currentAge;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="lifeSpan"></param>
        /// <param name="color"></param>
        public DeterioratingText(UIManager parent, Vector3 position, string text, float fontSize, float lifeSpan, Color color) : base(parent, position, text, fontSize, color)
        {
            m_lifeSpan = lifeSpan;
            m_currentAge = 0.0f;            
        }

        /// <summary>
        /// From XmlNode
        /// </summary>
        /// <param name="node"></param>
        public DeterioratingText(UIManager parent, XmlNode node)
            : base(parent, node)
        {
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if ( m_currentAge < m_lifeSpan )
                m_currentAge++;
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if ( m_currentAge < m_lifeSpan )
            {
                float percent = ( m_lifeSpan - m_currentAge ) / m_lifeSpan;
                Color newColor = m_Color;
                float alpha = newColor.A;
                alpha *= (float)Math.Round(percent, 2);
                newColor = new Color(m_Color.R, m_Color.G, m_Color.B, ( alpha / 255f ));

                spriteBatch.DrawString(m_Font, m_Text, new Vector2(m_Position.X, m_Position.Y), newColor);
            }
        }       

        internal void Reset()
        {
            m_currentAge = 0f;
        }

        public override bool Dispose()
        {
            return ( m_currentAge >= m_lifeSpan );
        }

        #endregion
        
    }
}
