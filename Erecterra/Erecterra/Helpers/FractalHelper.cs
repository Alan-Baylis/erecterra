#region Usings

using System;
using Lang.Erecterra.Objects;

#endregion

namespace Lang.Erecterra.Helpers
{
    public class FractalHelper
    {

        #region Fields

        /// <summary>
        /// Flag to specify whether the exponent array has been generated
        /// </summary>
        public static bool m_First = true;

        /// <summary>
        /// Exponent array, result values are multiplied by the values in this array by octave
        /// </summary>
        public static float[] m_ExponentArray;

        #endregion

        #region Methods

        /// <summary>
        /// Procedural multifractal evaluated at point
        /// Based on HybridMultifractal function on pg 440  of
        /// Texturing & Modeling: A Procedural Approach 3rd ed. ISBN: 1-55860-848-6
        /// </summary>
        /// <param name="x">x coord</param>
        /// <param name="y">y coord</param>
        /// <param name="hVal">fractal increment parameter</param>
        /// <param name="lacunarity">gap between successive frequencies</param>
        /// <param name="octaves">number of frequencies</param>
        /// <param name="offset">zero offset; determins multifracticality</param>
        /// <returns></returns>
        public static float HybridMultiFractal(float x, float y, float hVal, float lacunarity, float octaves, float offset)
        {
            float frequency = 0f;
            float result = 0f;
            float signal = 0f;
            float weight = 0f;
            float remainder = 0f;
           
            if ( m_First )
            {
                m_ExponentArray = new float[(int)octaves + 1];

                frequency = 1.0f;
                for ( int i = 0; i <= octaves; i++ )
                {
                    m_ExponentArray[i] = (float)Math.Pow(frequency, -hVal);
                    frequency *= lacunarity;
                }

                m_First = false;
            }

            result = ( World.PerlinNoise.Noise2(x, y) + offset ) * m_ExponentArray[0];
            weight = result;

            x *= lacunarity;
            y *= lacunarity;

            for ( int i = 1; i < octaves; i++ )
            {
                if ( weight > 1.0f )
                    weight = 1.0f;

                signal = ( World.PerlinNoise.Noise2(x, y) + offset ) * m_ExponentArray[i];
                result += weight * signal;
                weight *= signal;

                x *= lacunarity;
                y *= lacunarity;
            }

            remainder = octaves - (int)octaves;
            if ( remainder > 0 )
                result += remainder * World.PerlinNoise.Noise2(x, y) * m_ExponentArray[(int)octaves - 1];

            return result;
        }

        #endregion

    }
}
