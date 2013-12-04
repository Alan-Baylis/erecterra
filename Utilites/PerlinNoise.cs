#region Usings

using System;
using Lang.Utilites;

#endregion

namespace Lang.Utilities
{

    /// <summary>
    /// A class that generates Perlin Noise
    /// 
    /// http://www.mrl.nyu.edu/~perlin/doc/oscar.html#noise
    /// </summary>
    public class PerlinNoise
    {

        #region Fields

        /// <summary>
        /// Seed used to initialize the Random Number Generator with
        /// </summary>
        private int m_Seed;

        /// <summary>
        /// Size of the Gradient Table
        /// </summary>
        public static int GRADIENT_TABLE_SIZE = 512;

        /// <summary>
        /// This is a lattice table. Concept taken from Darwyn Peachey's chapter from
        /// Texturing & Modeling: A Procedural Approach pg 70 3rd ed.
        /// ISBN: 1-55860-848-6
        /// </summary>
        private static float[] m_GrandientTable = new float[GRADIENT_TABLE_SIZE];
        private static byte[] m_SelectedPerm = new byte[GRADIENT_TABLE_SIZE];

        /// <summary>
        /// Random Number Generator
        /// </summary>
        private static Random m_Random;

        #endregion

        #region Properties

        /// <summary>
        /// The Seed
        /// </summary>
        public int Seed
        {
            get
            {
                return m_Seed;
            }
        }

        #endregion

        #region Constructor

        public PerlinNoise(int seed)
        {
            if ( seed > 0 )
            {
                m_Seed = seed;
                m_Random = new Random(m_Seed);
            }
            else
            {
                m_Seed = (int)Math.Abs(Environment.TickCount);
                m_Random = new Random(m_Seed);
            }

            InitTables();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the lattice tables
        /// </summary>
        private void InitTables()
        {
            byte[] randomPerm = new byte[GRADIENT_TABLE_SIZE];

            m_Random.NextBytes(randomPerm);
            for ( int i = 0; i < ( GRADIENT_TABLE_SIZE / 2 ); i++ )
            {
                m_SelectedPerm[( GRADIENT_TABLE_SIZE / 2 ) + i] = m_SelectedPerm[i] = randomPerm[i];
            }

            //now that the permutations are all setup, create a new gradient table
            float[] newGradientTable = new float[256];

            for ( int i = 0; i < ( GRADIENT_TABLE_SIZE / 2 ); i++ )
            {
                newGradientTable[i] = -1.0f + 2.0f * ( (float)i / ( ( GRADIENT_TABLE_SIZE / 2.0f ) - 1 ) );
            }

            for ( int i = 0; i < (GRADIENT_TABLE_SIZE / 2); i++ )
            {
                m_GrandientTable[i] = newGradientTable[m_SelectedPerm[i]];
            }

            for ( int i = 256; i < GRADIENT_TABLE_SIZE; i++ )
            {
                m_GrandientTable[i] = m_GrandientTable[i - ( GRADIENT_TABLE_SIZE / 2 )];
            }
        }


        /// <summary>
        /// Basic 2D noise function
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        /// <returns>number in interval [0, 1]</returns>
        public float Noise2(float x, float y)
        {
            int x0 = ( x > 0.0f ? (int)x : (int)x - 1 );
            int y0 = ( y > 0.0f ? (int)y : (int)y - 1 );

            int X = x0 & 255;
            int Y = y0 & 255;

            float u = Smooth(x - x0);
            float v = Smooth(y - y0);

            int A = m_SelectedPerm[X] + Y;
            int AA = m_SelectedPerm[A];
            int AB = m_SelectedPerm[A + 1];
            int B = m_SelectedPerm[X + 1] + Y;
            int BA = m_SelectedPerm[B];
            int BB = m_SelectedPerm[B + 1];

            float a = Interpolation.Linear(m_GrandientTable[AA], m_GrandientTable[BA], u);
            float b = Interpolation.Linear(m_GrandientTable[AB], m_GrandientTable[BB], u);
            float c = Interpolation.Linear(a, b, v);
            float d = Interpolation.Linear(m_GrandientTable[AA + 1], m_GrandientTable[BA + 1], u);
            float e = Interpolation.Linear(m_GrandientTable[AB + 1], m_GrandientTable[BB + 1], u);
            float f = Interpolation.Linear(d, e, v);

            return Interpolation.Linear(c, f, 0);
        }

        /// <summary>
        /// Smooth value
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private float Smooth(double x)
        {
            return (float)( Math.Pow(x, 2) * ( 3 - 2 * x ) );
        }

        #endregion

    }
}