#region Usings

using System;
using System.Collections.Generic;
using Lang.Erecterra.Components;
using Lang.Erecterra.Helpers;
using Lang.Utilites;
using Microsoft.Xna.Framework;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{
    public class GrassWithDesert : Biome
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
            float val = FractalHelper.HybridMultiFractal(xVal, yVal, 0.1f, 2.0f, octaves, 1.0f);

            return val * World.Scale / 2.0f;
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
            Color color = Desert.GetColorAt(x, y, z, ref waterVertices);
            Color color2 = Grass.GetColorAt(x, y, z, ref waterVertices);

            float xVal = x / World.Scale / 2;
            float yVal = y / World.Scale / 2;
            float weight = FractalHelper.HybridMultiFractal(xVal, yVal, 0.1f, 2.0f, 1, 1.0f);

            float r = Interpolation.Cosine(color.R, color2.R, weight) / 255.0f;
            float g = Interpolation.Cosine(color.G, color2.G, weight) / 255.0f;
            float b = Interpolation.Cosine(color.B, color2.B, weight) / 255.0f;

            return new Color(r, g, b);
        }

        #endregion

    }
}
