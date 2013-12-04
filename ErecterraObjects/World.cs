using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ErecterraObjects {
    public class World {

        #region Fields

        private int m_Width;
        private int m_Length;
        private int m_Scale;

        VertexPositionNormalTexture[] m_Vertices;

        #endregion

        /// <summary>
        /// World constructor
        /// </summary>
        /// <param name="width">The width of the map</param>
        /// <param name="length">The length of the map</param>
        /// <param name="scale">The scale (the size of the cells)</param>
        public World(int width, int length, int scale) {
            m_Width = width;
            m_Length = length;
            m_Scale = scale;
        }


    }
}
