#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using Lang.Erecterra.Objects.Biomes;

#endregion

namespace Lang.Erecterra.Objects
{

    #region BiomeAndWeight Class

    /// <summary>
    /// A special class that is returned for Quadrant generation
    /// </summary>    
    public class BiomeAndWeight
    {

        #region Fields

        /// <summary>
        /// List of all the types to generate terrain with
        /// </summary>
        private List<Type> m_Types;

        /// <summary>
        /// The weight
        /// </summary>
        private float m_Weight;

        #endregion

        #region Properties

        /// <summary>
        /// Property to return the types
        /// </summary>
        public List<Type> types
        {
            get
            {
                return m_Types;
            }
        }

        /// <summary>
        /// Property to return the weight
        /// </summary>
        public float weight
        {
            get
            {
                return m_Weight;
            }

            set
            {
                m_Weight = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// BiomeAndWeight Parameterless Constructor
        /// </summary>
        public BiomeAndWeight()
        {
            m_Types = new List<Type>();
        }

        #endregion

    }

    #endregion

    public class FloatToType
    {

        #region Fields

        /// <summary>
        /// The singleton instance of this class
        /// </summary>
        private static FloatToType s_Instance;

        /// <summary>
        /// List of all the Biomes
        /// </summary>
        private List<Type> m_Types;

        /// <summary>
        /// The size of the interval for each Biome
        /// </summary>
        private float m_Interval;

        #endregion

        #region Properties

        public static FloatToType Instance
        {
            get
            {
                if ( s_Instance == null )
                {
                    s_Instance = new FloatToType();
                }

                return s_Instance;
            }
        }

        #endregion

        #region Singleton Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private FloatToType()
        {
            m_Types = new List<Type>();
            Assembly a = Assembly.GetExecutingAssembly();

            foreach ( Type t in a.GetTypes() )
            {
                if ( t.FullName.Contains("Biomes") && t.Name != "Biome" )
                    m_Types.Add(t);
            }

            m_Interval = 1.0f / (float)m_Types.Count;
            Shuffle();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Will shuffle the types list if necessary
        /// </summary>
        private void Shuffle()
        {
            //Shuffle as many times as there are biomes
            //After shuffling, ensure that mountains are either before or after a desert
            for ( int i = 0; i < m_Types.Count; i++ )
            {
                int index = (int)Math.Round(Math.Abs(World.PerlinNoise.Noise2(i * 10.0f, i * 10.0f)) * 10) % m_Types.Count;
                Type temp = m_Types[i];
                m_Types[i] = m_Types[index];
                m_Types[index] = temp;
            }

            #region Desert and Mountain Positioning

            for ( int i = 0; i < m_Types.Count; i++ )
            {
                if ( m_Types[i] == typeof(Desert) )
                {
                    if ( i > 0 )
                    {
                        if ( m_Types[i - 1] == typeof(Mountains) )
                        {
                            break;
                        }
                    }

                    if ( i < ( m_Types.Count - 1 ) )
                    {
                        if ( m_Types[i + 1] == typeof(Mountains) )
                        {
                            break;
                        }
                    }

                    float beforeOrAfter = Math.Abs(World.PerlinNoise.Noise2(i * 10.0f, i * 10.0f));
                    if ( i == 0 )
                        beforeOrAfter = 0.1f;
                    if ( i == m_Types.Count - 1 )
                        beforeOrAfter = -0.1f;

                    if ( beforeOrAfter < 0.0f )
                    {
                        //before
                        int index = i - 1;
                        for ( int j = 0; j < m_Types.Count; j++ )
                        {
                            if ( m_Types[j] == typeof(Mountains) )
                            {
                                Type temp = m_Types[j];
                                m_Types[j] = m_Types[index];
                                m_Types[index] = temp;
                            }
                        }
                    }
                    else
                    {
                        //after
                        int index = i + 1;
                        for ( int j = 0; j < m_Types.Count; j++ )
                        {
                            if ( m_Types[j] == typeof(Mountains) )
                            {
                                Type temp = m_Types[j];
                                m_Types[j] = m_Types[index];
                                m_Types[index] = temp;
                            }
                        }
                    }
                }
            } 

            #endregion
        }

        /// <summary>
        /// Returns a type based on the perlin noise
        /// </summary>
        /// <param name="perlinValue"></param>
        /// <returns></returns>
        public BiomeAndWeight GetBiome(float perlinValue)
        {
            BiomeAndWeight ret = new BiomeAndWeight();
            perlinValue = (float)Math.Abs(perlinValue);

            int interval = 0;
            while ( interval < m_Types.Count )
            {
                float intervalFloor = interval * m_Interval;
                float intervalCeil = ( interval + 1 ) * m_Interval;

                if ( perlinValue >= intervalFloor && perlinValue <= intervalCeil )
                {
                    float floorDifference = perlinValue - intervalFloor;

                    ret.weight = ( floorDifference / ( m_Interval / 2.0f ) );
                    if ( interval > 0 && ret.weight < 1.0 )
                        ret.types.Add(m_Types[interval - 1]);
                    ret.types.Add(m_Types[interval]);

                    break;
                }

                interval++;
            }

            return ret;
        }

        #endregion

    }
}
