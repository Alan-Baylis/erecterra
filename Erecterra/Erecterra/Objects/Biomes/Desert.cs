#region Usings

using System;
using System.Collections.Generic;
using Lang.Erecterra.Components;
using Lang.Utilites;
using Microsoft.Xna.Framework;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{
    public class Desert : Biome
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
            float xVal = ( x / World.Scale ) / 6.0f;
            float yVal = ( y / World.Scale ) / 6.0f;

            float val = World.PerlinNoise.Noise2(xVal, yVal) * World.Scale / 2.0f;
            float offset = World.PerlinNoise.Noise2(xVal * 4.0f, xVal * 4.0f);            
            return (float)Math.Sin(val + offset) * World.Scale;
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
            float r = 0.76f;
            float g = 0.62f;
            float b = 0.25f;

            float offset = OffsetAt(x, y);
            float waterChance = World.PerlinNoise.Noise2(x / 100.0f, y / 100.0f);

            if ( z <= World.WaterLevel )
            {
                float weight = Math.Abs(-0.95f - waterChance) / 0.03f;

                if ( waterChance <= -0.95f )
                {
                    r = 0;
                    g = 0;
                    b += offset;
                    waterVertices.Add(new WaterVertice(x, y, z));
                }
                else if ( waterChance > -0.95f && waterChance <= -0.92f )
                {
                    r = Interpolation.Cosine(r, 0.25f, 1f - weight);
                    g = Interpolation.Cosine(g, 0.25f + offset, 1f - weight);
                    b = Interpolation.Cosine(b, 0.0f, 1f - weight);
                }
                else
                {
                    r += offset;
                    g += offset;
                    b += offset;
                }
            }
            else
            {
                r += offset;
                g += offset;
                b += offset;
            }

            return new Color(r, g, b);
        }

        #endregion

    }
}
