#region Usings

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lang.Erecterra.Components;
using System.Xml;
using Microsoft.Xna.Framework.Input;
using System;
using Lang.Erecterra.Objects.UIObjects;

#endregion

namespace Lang.Erecterra.Objects.UIObjects
{
    public class UIText : UIObject
    {

        #region Fields

        /// <summary>
        /// The text that is displayed on the button
        /// </summary>
        protected string m_Text;

        /// <summary>
        /// The text size
        /// </summary>
        protected float m_Size;

        /// <summary>
        /// The current font
        /// </summary>
        protected SpriteFont m_Font;

        /// <summary>
        /// color of the text at youngest age
        /// </summary>
        protected Color m_Color;

        #endregion

        #region Properties

        public string Text
        {
            get
            {
                return m_Text;
            }

            set
            {
                m_Text = value;
            }
        }

        #endregion

        #region Constructors

        public UIText(UIManager parent, Vector3 position, string text, float fontSize, Color color)
            : base(parent, position)
        {
            m_Text = text;
            m_Size = fontSize;
            m_Color = color;

            if (m_Size == 14)
            {
                m_Font = UIManager.Font_14;
            }
            else if (m_Size == 16)
            {
                m_Font = UIManager.Font_16;
            }
            else if (m_Size == 18)
            {
                m_Font = UIManager.Font_18;
            }
            else if (m_Size == 72)
            {
                m_Font = UIManager.Font_72;
            }
            else
            {
                m_Font = UIManager.Font_14;
            }
        }

        /// <summary>
        /// Constructor for XML
        /// </summary>
        /// <param name="node"></param>
        public UIText(UIManager parent, XmlNode node)
            : base(parent, node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Text")
                {
                    m_Text = child.InnerText;
                }
                else if (child.Name == "Size")
                {
                    m_Size = float.Parse(child.InnerText);
                }
                else if (child.Name == "Color")
                {
                    float red = float.Parse(child.Attributes[0].Value.ToString());
                    float green = float.Parse(child.Attributes[1].Value.ToString());
                    float blue = float.Parse(child.Attributes[2].Value.ToString());

                    m_Color = new Color(red / 255f, green / 255f, blue / 255f);
                }
            }

            if (m_Size == 14)
            {
                m_Font = UIManager.Font_14;
            }
            else if (m_Size == 16)
            {
                m_Font = UIManager.Font_16;
            }
            else if (m_Size == 18)
            {
                m_Font = UIManager.Font_18;
            }
            else if (m_Size == 72)
            {
                m_Font = UIManager.Font_72;
            }
            else
            {
                m_Font = UIManager.Font_14;
            }

            //update position
            if (m_HorizontalAlignment == "Middle")
            {
                m_Position.X -= (m_Font.MeasureString(m_Text).X / 2.0f);
            }
            else if (m_HorizontalAlignment == "Right")
            {
                m_Position.X -= m_Font.MeasureString(m_Text).X;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(m_Font, m_Text, new Vector2(m_Position.X, m_Position.Y), m_Color);
        }

        /// <summary>
        /// This method checks if this UIText object was pressed
        /// </summary>
        /// <param name="mouseState"></param>
        public override void CheckPressed(MouseState mouseState)
        {
            float mouseX = mouseState.X;
            float mouseY = mouseState.Y;

            if (mouseX >= m_Position.X && mouseX <= m_Position.X + m_Font.MeasureString(m_Text).X)
            {
                if (mouseY >= m_Position.Y && mouseY <= m_Position.Y + m_Font.MeasureString(m_Text).Y)
                {
                    m_Manager.OnMousePressed(new OnMenuItemPressedEventArgs(m_Text));
                }
            }
        }

        #endregion

    }
}
