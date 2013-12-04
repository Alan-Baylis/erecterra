#region Usings

using System;
using System.Collections.Generic;
using Lang.Utilites;
using Microsoft.Xna.Framework;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{
    public class Ocean : Biome
    {

        #region Fields

        /// <summary>
        /// The point when a beach starts
        /// </summary>        
        public static Vector2 OceanStart = Vector2.Zero;

        #endregion

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
            return 0;
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
            bool oceanBefore = true;
            bool oceanAfter = true;

            float biomeValBefore = World.PerlinNoise.Noise2(( x - 10 ) / World.WorldScale, ( y - 10 ) / World.WorldScale);
            BiomeAndWeight bw = World.FloatToType.GetBiome(biomeValBefore);
            if ( bw.types[0] != typeof(Ocean) )
            {
                oceanBefore = false;
                OceanStart = new Vector2(x + 10, y + 10);
            }

            float biomeValAfter = World.PerlinNoise.Noise2(( x + 10 ) / World.WorldScale, ( y + 10 ) / World.WorldScale);
            bw = World.FloatToType.GetBiome(biomeValBefore);
            if ( bw.types[0] != typeof(Ocean) )
            {
                oceanAfter = false;
                OceanStart = new Vector2(x + 10, y + 10);
            }

            float offset = (float)Math.Abs(World.PerlinNoise.Noise2(x / 100.0f, y / 100.0f)) / 2.0f;
            float r = 0.76f;
            float g = 0.62f;
            float b = 0.25f;
            float a = 1.0f;

            if ( oceanBefore && oceanAfter )
            {
                r = 0;
                g = 0;
                b = 0.25f;
                a = 0.5f;
                waterVertices.Add(new WaterVertice(x, y, z));
            }
            else
            {
                float weight = (float)Math.Sqrt(Math.Pow(OceanStart.X - x, 2) + Math.Pow(OceanStart.Y - z, 2)) / 10.0f;
                r += offset;
                r = Interpolation.Cosine(r, 0, weight);
                g += offset;
                g = Interpolation.Cosine(g, 0, weight);
                b += offset;
                b = Interpolation.Cosine(b, 0.25f, weight);
                a = Interpolation.Cosine(a, 0.5f, weight);
            }

            return new Color(r, g, b, a);
        }

        #endregion

    }
}
