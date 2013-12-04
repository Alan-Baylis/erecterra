#region Usings

using System;
using Lang.Erecterra.Components;
using Lang.Erecterra.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{
    public class Grass : Biome
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

            return (float)Math.Sin(val) * World.Scale;
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
            float g = 0.25f + OffsetAt(x, y);            
            return new Color(0, g, 0);
        }

        #endregion

    }
}
