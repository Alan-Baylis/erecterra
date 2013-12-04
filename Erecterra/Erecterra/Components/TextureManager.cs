#region Usings

using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Lang.Erecterra.Components
{

    public class TextureManager : GameComponent
    {

        #region Fields

        /// <summary>
        /// The Erecterra object
        /// </summary>
        private Game m_Game;

        /// <summary>
        /// A list of textures
        /// </summary>
        List<Texture2D> m_Textures;

        private const string TEXTURE_FOLDER = @"Content\Textures\";
        private const string TEXTURE_FILETYPE = ".xnb";

        public static TextureManager Instance;

        #endregion

        #region Properties

        /// <summary>
        /// Return all the Textures
        /// </summary>
        public List<Texture2D> Textures
        {
            get
            {
                return m_Textures;
            }
        }

        #endregion

        #region Constructor

        public TextureManager(Game game)
            : base(game)
        {
            m_Game = game;
            m_Textures = new List<Texture2D>();
            if ( Instance != null )
                throw new AlreadyInstantiatedException();
            Instance = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {            
            string[] filePaths = Directory.GetFiles(TEXTURE_FOLDER);

            foreach ( string s in filePaths )
            {
                string name = s.Replace(TEXTURE_FOLDER, "").Replace(TEXTURE_FILETYPE, "");
                Texture2D tex = m_Game.Content.Load<Texture2D>(@"Textures\" + name);
                tex.Name = name;
                m_Textures.Add(tex);
            }

            base.Initialize();
        }

        /// <summary>
        /// Get the texture with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Texture2D GetTexture(string name)
        {
            foreach ( Texture2D tex in m_Textures )
            {
                if ( tex.Name == name )
                    return tex;
            }

            return null;
        }

        /// <summary>
        /// Get the index for the specified texture
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetTextureIndex(string name)
        {
            for ( int i = 0; i < m_Textures.Count; i++ )
            {                
                if ( m_Textures[i].Name == name )
                    return i;
            }

            return 3;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion

    }
}
