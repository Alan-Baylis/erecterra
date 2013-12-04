//#region Usings

//using System;
//using Lang.Utilites;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//#endregion

//namespace Lang.Erecterra.Objects.Biomes
//{
//    public class GrassWithWater : Biome
//    {

//        /// <summary>
//        /// Get the height at the point x, y
//        /// Args are any additional parameters
//        /// </summary>
//        /// <param name="x"></param>
//        /// <param name="y"></param>
//        /// <param name="args"></param>
//        /// <returns>A float representing the height at x, y</returns>
//        public static float GetHeightAt(float x, float y, params object[] args)
//        {
//            return Grass.GetHeightAt(x, y, args);
//        }

//        /// <summary>
//        /// Return the color for the point at x, y, z
//        /// </summary>
//        /// <param name="x"></param>
//        /// <param name="y"></param>
//        /// <param name="z"></param>
//        /// <returns>A Color</returns>
//        public static Color GetColorAt(float x, float y, float z)
//        {
//            float r = 0.0f;
//            float g = 0.25f;
//            float b = 0.0f;

//            float offset = OffsetAt(x, y);
//            float waterChance = World.PerlinNoise.Noise2(x / 100.0f, y / 100.0f);

//            if ( z <= World.WaterLevel )
//            {
//                float weight = Math.Abs(0.4f - waterChance) / 1.4f;

//                if ( waterChance <= 1.4f )
//                {
//                    r = 0;
//                    g = 0;
//                    b += offset;
//                }
//                else if ( waterChance > 1.4f && waterChance <= 1.43f )
//                {
//                    r = Interpolation.Linear(r, 0.25f, 1f - weight);
//                    g = Interpolation.Linear(g, 0.25f + offset, 1f - weight);
//                    b = Interpolation.Linear(b, 0.0f, 1f - weight);
//                }
//                else if ( waterChance > 1.43f && waterChance <= 1.46f )
//                {
//                    r = 0;
//                    g = 0;
//                    b = 0.15f + offset;
//                }
//                else
//                {
//                    r += offset;
//                    g += offset;
//                    b += offset;
//                }
//            }
//            else
//            {
//                g += offset;
//            }

//            return new Color(r, g, b);
//        }

//        public static void PostGenerated(ref QuadrantVertex[,] vertices, Type[,] types)
//        {

//        }
//    }
//}
