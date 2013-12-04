#region Usings

using System;
using System.Collections.Generic;
using Lang.Erecterra.Components;
using Lang.Erecterra.Helpers;
using Microsoft.Xna.Framework;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{

    public class Mountains : Biome
    {

        #region Methods

        /// <summary>
        /// Get the height at the point x, y
        /// Args are any additional parameters
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="args"></param>
        /// <returns>A float representing the height at x, y</returns>
        public static float GetHeightAt(float x, float y, params object[] args)
        {
            float xVal = x / World.Scale / 2;
            float yVal = y / World.Scale / 2;

            float octaves = (float)Math.Log(1680f, 2) - 2;
            float val = FractalHelper.HybridMultiFractal(xVal, yVal, 0.1f, 2.0f, octaves, 1.0f) * (float)Math.Pow(World.Scale, 1.2f);

            return val;
        }

        /// <summary>
        /// Return the color for the point at x, y, z
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>A Color</returns>
        public static Color GetColorAt(float x, float y, float z, ref List<WaterVertice> waterVertices)
        {
            float r = 0;
            float g = 0;
            float b = 0;

            float offset = World.PerlinNoise.Noise2(x, y) * 10;
            float colorSkew = OffsetAt(x, y);

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

        #endregion

    }
}
