#region Usings

using System.Collections.Generic;
using Lang.Erecterra.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

#endregion

namespace Lang.Erecterra.Objects.UIObjects
{
    public class UIMenu
    {

        #region Fields

        private string m_Name;
        private List<UIObject> m_Objects;
        private bool m_Visible;
        private UIManager m_Manager;

        #endregion

        #region Properties

        /// <summary>
        /// Whether or not the menu is visible
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return m_Visible;
            }

            set
            {
                m_Visible = value;
            }
        }

        /// <summary>
        /// Get the name of the menu
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Access to the menu objects
        /// </summary>
        public List<UIObject> MenuItems
        {
            get
            {
                return m_Objects;
            }
        }

        #endregion

        #region Constructor

        public UIMenu(UIManager parent, string name)
        {
            m_Name = name;
            m_Visible = false;
            m_Objects = new List<UIObject>();
            m_Manager = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            foreach ( UIObject obj in m_Objects )
            {
                obj.Update(gameTime);
            }

            //Dispose of objects that are meant to be disposed of!
            for ( int i = 0; i < m_Objects.Count; i++ )
            {
                if ( m_Objects[i].Dispose() )
                {
                    m_Objects.Remove(m_Objects[i]);
                    i--;
                }
            }
        }

        /// <summary>
        /// Get the object with the provided name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UIObject GetObject(string name)
        {
            foreach ( UIObject obj in m_Objects )
            {
                if ( obj.Name() == name )
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Draw the UI objects
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach ( UIObject obj in m_Objects )
            {
                obj.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Check if its pressed
        /// </summary>
        /// <param name="mouseState"></param>
        public void CheckPressed(MouseState mouseState)
        {
            if ( !m_Visible )
                return;

            foreach ( UIObject obj in m_Objects )
            {                
                obj.CheckPressed(mouseState);
            }
        }

        #endregion
    }
}
