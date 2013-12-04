#region Usings

using System; 

#endregion

namespace Lang.Utilites
{
    public class Interpolation
    {

        #region Methods

        /// <summary>
        /// Linear interpolation
        /// </summary>
        /// <param name="a">left value a</param>
        /// <param name="b">right value b</param>
        /// <param name="x">the value that determines the interpolated value between a and b</param>
        /// <returns>interpolated value</returns>
        public static float Linear(float a, float b, float x)
        {
            return a * ( 1 - x ) + b * x;
        }

        /// <summary>
        /// Cosine interpolation
        /// </summary>
        /// <param name="a">left value a</param>
        /// <param name="b">right value b</param>
        /// <param name="x">the value that determines the interpolated value between a and b</param>
        /// <returns>interpolated value</returns>
        public static float Cosine(float a, float b, float x)
        {
            float ft = (float)( x * Math.PI );
            float f = (float)( ( 1 - Math.Cos(ft) ) * 0.5 );

            return a * ( 1 - f ) + b * f;
        }

        /// <summary>
        /// Cubic interpolation
        /// </summary>
        /// <param name="beforeA">The value before a</param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="afterB">The value after b</param>
        /// <param name="x">the value that determines the interpolated value between a and b</param>
        /// <returns>interpolated value</returns>
        public static float Cubic(float beforeA, float A, float B, float afterB, float x)
        {
            float P = ( afterB - B ) - ( beforeA - A );
            float Q = ( beforeA - A ) - P;
            float R = ( B - beforeA );
            float S = A;

            return (float)( P * Math.Pow(x, 3) + Q * Math.Pow(x, 2) + ( R * x ) + S );
        } 

        #endregion

    }
}


