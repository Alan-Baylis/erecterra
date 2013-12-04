#region Usings

using System;
using System.Collections.Generic;
using Lang.Erecterra.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{
    public class Mountains : IBiome
    {

        #region Fields

        /// <summary>
        /// The intensity of the mountains
        /// </summary>
        private float m_Intensity;

        /// <summary>
        /// Width of this biome
        /// </summary>
        private int m_Width;

        /// <summary>
        /// Length of this biome
        /// </summary>
        private int m_Length;

        /// <summary>
        /// Represents all the heights
        /// </summary>
        private float[,] m_Heights;

        /// <summary>
        /// Represents the color of each point
        /// </summary>
        private Color[,] m_Colors;

        /// <summary>
        /// Represents the origin of this biome
        /// </summary>
        private Vector2 m_Start;

        /// <summary>
        /// Represents the end of this biome
        /// </summary>
        private Vector2 m_End;

        #endregion

        #region Constructor

        public Mountains(float intensity)
        {
            m_Intensity = intensity;
        }

        #endregion

        #region IBiome

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void PreSetup(Vector2 v1, Vector2 v2)
        {
            m_Start = v1;
            m_End = v2;
            m_Width = (int)Math.Abs(v1.X - v2.X);
            m_Length = (int)Math.Abs(v1.Y - v2.Y);
            m_Heights = new float[m_Width, m_Length];
            m_Colors = new Color[m_Width, m_Length];
        }

        /// <summary>
        /// Create mountains!
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void Setup(Vector2 v1, Vector2 v2)
        {
            PreSetup(v1, v2);

            for ( int j = 0; j < m_Length; j++ )
            {
                for ( int i = 0; j < m_Length; i++ )
                {
                    float x = i + v1.X;
                    float y = j + v1.Y;

                    m_Heights[i, j] = HeightAt(new Vector2(x, y));                    
                }
            }

            PostSetup(v1, v2);

            for ( int j = 0; j < m_Length; j++ )
            {
                for ( int i = 0; j < m_Length; i++ )
                {
                    float x = i + v1.X;
                    float y = j + v1.Y;

                    m_Colors[i, j] = ColorAt(new Vector3(x, y, m_Heights[i, j]));
                }
            }
        }

        /// <summary>
        /// Smoothe and reduce the height of the generated terrain
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void PostSetup(Vector2 v1, Vector2 v2)
        {
            //Smoothe the terrain
            float total;
            for ( int i = 1; i < m_Width - 1; i++ )
            {
                for ( int j = 1; j < m_Length - 1; j++ )
                {
                    total = 0.0f;

                    for ( int u = -1; u <= 1; u++ )
                        for ( int v = -1; v <= 1; v++ )
                            total += m_Heights[i + u, j + v];

                    if ( m_Heights[i, j] < 1450.0f )
                        m_Heights[i, j] = total / 9.0f;
                }
            }

            // Shift down
            float min = float.MaxValue;
            for ( int i = 0; i < m_Width; i++ )
                for ( int j = 0; j < m_Length; j++ )
                    if ( m_Heights[i, j] > 0 )
                        min = (float)Math.Min(min, m_Heights[i, j]);

            for ( int i = 0; i < m_Width; i++ )
                for ( int j = 0; j < m_Length; j++ )
                    m_Heights[i, j] -= min;
        }

        public float HeightAt(Vector2 v1)
        {
            float xVal = v1.X / World.Scale / 2;
            float yVal = v1.Y / World.Scale / 2;

            float octaves = (float)Math.Log(1680f, 2) - 2;
            float val = FractalHelper.HybridMultiFractal(xVal, yVal, 0.1f, 2.0f, octaves, 1.0f) * (float)Math.Pow(World.Scale, 1.09f) * m_Intensity;

            return val;
        }

        public Color ColorAt(Vector3 v1)
        {
            float x = v1.X;
            float y = v1.Y;
            float z = v1.Z;

            float r = 0;
            float g = 0;
            float b = 0;

            float offset = World.PerlinNoise.Noise2(x, y) * 10;
            float colorSkew = (float)Math.Abs(World.PerlinNoise.Noise2(x / 100.0f, y / 100.0f));

            if ( z < ( 300 + offset ) )
            {
                g = 0.25f + colorSkew;
            }
            else if ( z > ( 300 + offset ) && z < ( 1200 + offset ) )
            {
                r = 0.20f + colorSkew;
                g = 0.20f + colorSkew;
                b = 0.20f + colorSkew;
            }
            else
            {
                r = 0.88f - ( colorSkew / 9.0f );
                b = 0.88f - ( colorSkew / 9.0f );
                g = 0.95f - ( colorSkew / 9.0f );
            }

            return new Color(r, g, b);
        }

        /// <summary>
        /// Returns all the vertices that the biome generated
        /// </summary>
        /// <param name="v1"></param>
        /// <returns></returns>
        public List<VertexPositionColor> ToVertices()
        {
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            for ( int j = 0; j < m_Length; j++ )
            {
                for ( int i = 0; j < m_Length; i++ )
                {
                    float x = ( i + m_Start.X ) * World.Scale;
                    float y = m_Heights[i, j];
                    float z = ( j + m_Start.Y ) * World.Scale;
                    Color c = m_Colors[i, j];
                    vertices.Add(new VertexPositionColor(new Vector3(x, y, z), c));
                }
            }

            return vertices;
        }

        public bool ShouldBeDrawn(Vector3 position)
        {
            return true;
        }

        #endregion

    }
}
