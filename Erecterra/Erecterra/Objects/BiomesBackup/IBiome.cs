#region Usings

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Lang.Erecterra.Objects.Biomes
{
    public interface IBiome
    {
        /// <summary>
        /// This method is run before setup
        /// </summary>        
        void PreSetup(Vector2 v1, Vector2 v2);

        /// <summary>
        /// This method performs any setup that is required for generating an area
        /// </summary>        
        void Setup(Vector2 v1, Vector2 v2);

        /// <summary>
        /// This method is run after setup
        /// </summary>        
        void PostSetup(Vector2 v1, Vector2 v2);

        /// <summary>
        /// Returns the height at the x, z coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        float HeightAt(Vector2 v1);

        /// <summary>
        /// Returns the color for the point at (x, y, z)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        Color ColorAt(Vector3 v1);

        /// <summary>
        /// Returns a list of all the heights and colors in a list of VertexPositionColors
        /// </summary>
        /// <returns></returns>
        List<VertexPositionColor> ToVertices();

        /// <summary>
        /// Determine whether this biome should be drawn
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        bool ShouldBeDrawn(Vector3 position);
    }
}
