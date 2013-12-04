#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Lang.Erecterra.Objects.UIObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Lang.Erecterra.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class UIManager : GameComponent
    {

        #region Fields

        public const string UIMENUTAG = "UIMenu";

        /// <summary>
        /// Fonts
        /// </summary>
        public static SpriteFont Font_14;
        public static SpriteFont Font_16;
        public static SpriteFont Font_18;
        public static SpriteFont Font_72;

        /// <summary>
        /// Static menu xml file
        /// </summary>
        public static string s_MenuXml = @"Content\Menus\Menus.xml";

        /// <summary>
        /// The game
        /// </summary>
        private Game m_Game;

        /// <summary>
        /// SpriteBatch
        /// </summary>
        private SpriteBatch m_SpriteBatch;

        /// <summary>
        /// List of all the current menus loaded
        /// </summary>
        private List<UIMenu> m_Menus;

        /// <summary>
        /// Reference to the UIManager
        /// </summary>
        private static UIManager s_Instance;

        #endregion

        #region Properties

        public static UIManager Instance
        {
            get
            {
                return s_Instance;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// UIManager constructor
        /// </summary>
        /// <param name="game"></param>
        public UIManager(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            m_Game = game;
            m_SpriteBatch = spriteBatch;
            m_Menus = new List<UIMenu>();
            s_Instance = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check if any of the objects have been pressed
        /// </summary>
        /// <param name="mouseState"></param>
        public void CheckPressed(MouseState mouseState)
        {
            for ( int i = 0; i < m_Menus.Count; i++ )
            {
                if ( m_Menus[i].IsVisible )
                {
                    m_Menus[i].CheckPressed(mouseState);
                }
            }
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            UIManager.Font_14 = m_Game.Content.Load<SpriteFont>(@"Fonts\arial_14");
            UIManager.Font_16 = m_Game.Content.Load<SpriteFont>(@"Fonts\arial_16");
            UIManager.Font_18 = m_Game.Content.Load<SpriteFont>(@"Fonts\arial_18");
            UIManager.Font_72 = m_Game.Content.Load<SpriteFont>(@"Fonts\arial_72");
            LoadMenus();
        }

        /// <summary>
        /// When the mouse presses on an object
        /// </summary>
        /// <param name="args"></param>
        public void OnMousePressed(OnMenuItemPressedEventArgs args)
        {
            EventHandler<OnMenuItemPressedEventArgs> evh = OnMousePressedEvent;
            if ( evh != null )
                evh(this, args);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach ( UIMenu m in m_Menus )
            {
                m.Update(gameTime);
            }

            for ( int i = 0; i < m_Menus.Count; i++ )
            {
                if ( m_Menus[i].MenuItems.Count == 0 )
                {
                    m_Menus.Remove(m_Menus[i]);
                    i--;
                }
            }
        }

        /// <summary>
        /// Draw the UI objects
        /// </summary>
        public void Draw()
        {
            m_SpriteBatch.Begin();

            foreach ( UIMenu m in m_Menus )
            {
                if ( m.IsVisible )
                {
                    m.Draw(m_SpriteBatch);
                }
            }

            m_SpriteBatch.End();
        }

        /// <summary>
        /// Load all menus in the XML file
        /// </summary>
        private void LoadMenus()
        {
            string xml = File.ReadAllText(s_MenuXml);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNodeList menus = doc.GetElementsByTagName(UIMENUTAG);
            foreach ( XmlNode node in menus )
            {
                UIMenu menu = new UIMenu(this, node.Attributes[0].Value);

                foreach ( XmlNode child in node.ChildNodes )
                {
                    if ( child.Name == "UIText" )
                    {
                        menu.MenuItems.Add(new UIText(this, child));
                    }
                    else if ( child.Name == "DeterioratingText" )
                    {
                        menu.MenuItems.Add(new DeterioratingText(this, child));
                    }
                }

                m_Menus.Add(menu);
            }
        }

        /// <summary>
        /// Gets the menu with the specified name
        /// </summary>
        /// <param name="menuName"></param>
        /// <returns>UIMenu object</returns>
        public UIMenu GetMenu(string menuName)
        {
            foreach ( UIMenu m in m_Menus )
            {
                if ( m.Name == menuName )
                {
                    return m;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a menu to the UIManager
        /// </summary>
        /// <param name="menu"></param>
        public void AddMenu(UIMenu menu)
        {
            m_Menus.Add(menu);
        }

        #endregion

        #region Events

        public EventHandler<OnMenuItemPressedEventArgs> OnMousePressedEvent;

        #endregion

    }
}
