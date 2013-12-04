#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using Lang.Utilites;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

#endregion

namespace Lang.Erecterra.Objects
{

    /// <summary>
    /// Represents a vertice that is water
    /// </summary>
    public struct WaterVertice
    {
        public float x;
        public float y;
        public float z;

        /// <summary>
        /// WaterVertice constructor
        /// </summary>
        /// <param name="a">x value</param>
        /// <param name="b">y value</param>
        /// <param name="c">z value</param>
        public WaterVertice(float a, float b, float c)
        {
            x = a;
            y = b;
            z = c;
        }
    }

    public class Quadrant : IDisposable
    {

        #region Fields

        /// <summary>
        /// The width and height of a quadrant in vertices
        /// </summary>
        public const int QuadrantSize = 150;

        /// <summary>
        /// This quadrants origin x coordinate
        /// </summary>
        private int m_startX;

        /// <summary>
        /// This quadrants origin y coordinate
        /// </summary>
        private int m_startY;

        /// <summary>
        /// Contains the vertices for the quadrant        
        /// After the quadrant is generated these should never change
        /// </summary>
        private QuadrantVertex[,] m_Vertices;

        /// <summary>
        /// The number of Quadrant objects
        /// </summary>
        public static int QuadrantCount = 0;

        /// <summary>
        /// The water vertices in this quadrant
        /// </summary>
        private List<WaterVertice> m_WaterVertices;

        /// <summary>
        /// The sum of generation times
        /// </summary>
        private static float SumGenTime;

        /// <summary>
        /// The number of generated quadrants ever created
        /// </summary>
        private static float OverallCount;

        #endregion

        #region Properties

        /// <summary>
        /// Return the vertices
        /// </summary>
        public QuadrantVertex[,] Vertices
        {
            get
            {
                return m_Vertices;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Quadrant constructor
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        public Quadrant(int startX, int startY)
        {
            float timeStart = Environment.TickCount;
            m_startX = startX;
            m_startY = startY;

            m_Vertices = new QuadrantVertex[QuadrantSize, QuadrantSize];
            m_WaterVertices = new List<WaterVertice>();
            Initialize();
            QuadrantCount++;
            OverallCount++;
            SumGenTime += Environment.TickCount - timeStart;            
        }

        /// <summary>
        /// Quadrant Deconstructor
        /// </summary>
        public void Dispose()
        {
            m_Vertices = null;
            QuadrantCount--;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize
        /// </summary>
        public void Initialize()
        {

            #region Generate Heights and Color

            for ( int y = 0; y < QuadrantSize; y++ )
            {
                for ( int x = 0; x < QuadrantSize; x++ )
                {
                    //Get the type of biome that should be here
                    float biomeVal = World.PerlinNoise.Noise2(( x + m_startX ) / World.WorldScale, ( y + m_startY ) / World.WorldScale);
                    BiomeAndWeight bw = World.FloatToType.GetBiome(biomeVal);
                    Type type = bw.types[0];
                    Type typeTwo = ( bw.types.Count > 1 ) ? bw.types[1] : null;
                    float weight = bw.weight;

                    #region Height Code

                    //Get the method to determine the height
                    MethodInfo GetHeightAt = type.GetMethod("GetHeightAt", BindingFlags.Static | BindingFlags.Public);
                    if ( GetHeightAt == null )
                        throw new BiomeNotImplementedException();

                    object[] args = { x + m_startX, y + m_startY, new object[] { 2.0f } };
                    m_Vertices[x, y] = new QuadrantVertex();
                    Vector3 position = new Vector3(( x + m_startX ) * World.Scale, (float)GetHeightAt.Invoke(null, args), ( y + m_startY ) * World.Scale);

                    if ( typeTwo != null )
                    {
                        GetHeightAt = typeTwo.GetMethod("GetHeightAt", BindingFlags.Static | BindingFlags.Public);
                        if ( GetHeightAt == null )
                            throw new BiomeNotImplementedException();

                        args = new object[] { x + m_startX, y + m_startY, new object[] { 2.0f } };
                        float biomeTwoHeight = (float)GetHeightAt.Invoke(null, args);
                        float newHeight = Interpolation.Cosine(position.Y, biomeTwoHeight, weight);
                        position.Y = newHeight;
                    }

                    m_Vertices[x, y].Position = position; 

                    #endregion

                    #region Color Code

                    //Get the method to determine the height
                    MethodInfo m = type.GetMethod("GetColorAt", BindingFlags.Static | BindingFlags.Public);
                    if ( m == null )
                        throw new BiomeNotImplementedException();

                    object[] args2 = { x + m_startX, y + m_startY, m_Vertices[x, y].Position.Y, m_WaterVertices };
                    Color color = (Color)m.Invoke(null, args2);

                    if ( typeTwo != null )
                    {
                        m = typeTwo.GetMethod("GetColorAt", BindingFlags.Static | BindingFlags.Public);
                        if ( m == null )
                            throw new BiomeNotImplementedException();

                        Color biomeTwoColor = (Color)m.Invoke(null, args2);

                        float r = Interpolation.Cosine(color.R, biomeTwoColor.R, weight) / 255.0f;
                        float g = Interpolation.Cosine(color.G, biomeTwoColor.G, weight) / 255.0f;
                        float b = Interpolation.Cosine(color.B, biomeTwoColor.B, weight) / 255.0f;
                        color = new Color(r, g, b);
                    }

                    m_Vertices[x, y].Color = color;

                    #endregion

                }
            }

            #endregion

            #region Calculate Normals

            CalculateNormals();

            #endregion

        }

        /// <summary>
        /// Calculate the normals for the given vertices
        /// </summary>
        /// <param name="Vertices"></param>
        /// <param name="Indices"></param>
        private void CalculateNormals(int xStart = 0, int xEnd = QuadrantSize - 1, int yStart = 0, int yEnd = QuadrantSize - 1)
        {
            for ( int j = 0; j < Quadrant.QuadrantSize - 1; j++ )
            {
                for ( int i = 0; i < Quadrant.QuadrantSize - 1; i++ )
                {
                    QuadrantVertex v1 = m_Vertices[i, j];
                    QuadrantVertex v2 = m_Vertices[i, j + 1];
                    QuadrantVertex v3 = m_Vertices[i + 1, j];
                    QuadrantVertex v4 = m_Vertices[i + 1, j + 1];

                    Vector3 normal = Vector3.Cross(v2.Position - v1.Position, v4.Position - v1.Position);
                    Vector3 normal2 = Vector3.Cross(v4.Position - v1.Position, v3.Position - v1.Position);

                    m_Vertices[i, j].Normal += ( normal + normal2 );
                    m_Vertices[i, j + 1].Normal += normal;
                    m_Vertices[i + 1, j].Normal += normal;
                    m_Vertices[i + 1, j + 1].Normal += ( normal + normal2 );
                }
            }

            for ( int j = 0; j < Quadrant.QuadrantSize; j++ )
                for ( int i = 0; i < Quadrant.QuadrantSize; i++ )
                    m_Vertices[i, j].Normal.Normalize();
        }

        /// <summary>
        /// Update any vertices in a Quadrant that need to be updated
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(int gameTime)
        {
            //if ( m_WaterVertices.Count == 0 )
            return;

            // Need to know the range of x and y values            
            float minX = float.MaxValue;
            float maxX = float.MinValue;

            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach ( WaterVertice w in m_WaterVertices )
            {
                minX = (float)Math.Min(minX, w.x);
                maxX = (float)Math.Max(maxX, w.x);

                minY = (float)Math.Min(minY, w.y);
                maxY = (float)Math.Max(maxY, w.y);
            }

            foreach ( WaterVertice w in m_WaterVertices )
            {
                int x = (int)( w.x - m_startX );
                int y = (int)( w.y - m_startY );
                Vector3 position = m_Vertices[x, y].Position;

                if ( position.Y <= ( w.z + 50.0f ) )
                {
                    float xVal = ( w.x / 2000.0f );
                    float yVal = ( w.y / 2000.0f );

                    position.Y = w.z + ( (float)Math.Sin(gameTime * xVal) + (float)Math.Cos(gameTime * yVal) ) * 10.0f;
                    m_Vertices[x, y].Position = position;
                }
            }

            // Only re-calculate normals in the range of the water
            CalculateNormals((int)( minX / World.Scale ), (int)( maxX / World.Scale ), (int)( minY / World.Scale ), (int)( maxY / World.Scale ));
        }

        /// <summary>
        /// Returns the average generation time of a Quadrant
        /// </summary>
        /// <returns></returns>
        public static float AverageGenerationTime()
        {
            return ( SumGenTime / OverallCount );
        }

        #endregion

    }
}
