#region Usings

using System;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{

    #region Methods

    /// <summary>
    /// This is just so the Class Diagram can group all the different biome classes together
    /// </summary>
    public class Biome
    {

        /// <summary>
        /// Generate an offset for colors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float OffsetAt(float x, float y)
        {
            return (float)Math.Abs(World.PerlinNoise.Noise2(x / 200.0f, y / 200.0f)) / 2.0f;
        }
    }

    #endregion

}