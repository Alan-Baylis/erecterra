
#region Usings

using System.Xml;
using Lang.Erecterra.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Lang.Erecterra.Objects.UIObjects
{
    public class UIObject
    {

        #region Fields

        /// <summary>
        /// Position of the object
        /// </summary>
        protected Vector3 m_Position;

        /// <summary>
        /// Horizontal alignment of the object
        /// </summary>
        protected string m_HorizontalAlignment;

        /// <summary>
        /// UIManager
        /// </summary>
        protected UIManager m_Manager;

        #endregion

        #region Constructor

        public UIObject(UIManager manager, Vector3 position)
        {
            m_Manager = manager;
            m_Position = position;            
        }

        public UIObject(UIManager manager, XmlNode node)
        {
            m_Manager = manager;
            foreach ( XmlNode child in node.ChildNodes )
            {
                if ( child.Name == "Position" )
                {
                    m_Position = new Vector3();
                    m_HorizontalAlignment = child.Attributes[0].Value.ToString();

                    if ( m_HorizontalAlignment == "Middle" )
                    {
                        m_Position.X = ( Erecterra.WIDTH / 2.0f ) - float.Parse(child.Attributes[1].Value.ToString());
                    }
                    else if ( m_HorizontalAlignment == "Right" )
                    {
                        m_Position.X = Erecterra.WIDTH - float.Parse(child.Attributes[1].Value.ToString());                        
                    }
                    else
                    {
                        m_Position.X = float.Parse(child.Attributes[1].Value.ToString());
                    }

                    m_Position.Y = float.Parse(child.Attributes[2].Value.ToString());
                }
            }
        }

        #endregion

        #region Methods

        public virtual string Name()
        {
            return "UIObject";
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void CheckPressed(MouseState mouseState)
        {
            
        }

        public virtual bool Dispose()
        {
            return false;
        }

        #endregion

    }
}
