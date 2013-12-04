#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using Lang.Erecterra.Components;
using Lang.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Lang.Utilites;

#endregion

namespace Lang.Erecterra.Objects
{

    /// <summary>
    /// The World object
    /// </summary>
    public class World
    {

        #region Fields

        /// <summary>
        /// Scale
        /// </summary>
        public static float Scale;

        /// <summary>
        /// World scale for PerlinNoise
        /// </summary>
        public static float WorldScale;

        /// <summary>
        /// PerlinNoise
        /// </summary>
        public static PerlinNoise PerlinNoise = new PerlinNoise(0);

        /// <summary>
        /// The units to draw and generate at one time
        /// </summary>
        public static int UnitsToDraw = 300;

        /// <summary>
        /// The seed of the world
        /// </summary>
        private int m_Seed;

        /// <summary>
        /// The name of the world
        /// </summary>
        private string m_Name;

        /// <summary>
        /// The quadrants that represent the map
        /// </summary>
        private Quadrant[,] m_Quadrants;

        /// <summary>
        /// Whether or not a quadrant is generation
        /// </summary>
        private bool m_QuadrantGenerating;

        /// <summary>
        /// The indices
        /// </summary>
        private List<int> m_Indices;

        /// <summary>
        /// The rate at which biomes change
        /// </summary>
        public const float RateOfChange = 9.0f;

        /// <summary>
        /// The water level
        /// </summary>
        public static float WaterLevel = 0.0f;

        /// <summary>
        /// FloatToType object
        /// </summary>
        public static FloatToType FloatToType;

        /// <summary>
        /// Has the World been initialized?
        /// </summary>
        private bool m_Initialized;

        /// <summary>
        /// Whether or not a thread has been created for generating quadrants
        /// </summary>
        private bool m_QuadrantThread;

        /// <summary>
        /// Max x component of Quadrant Array
        /// </summary>
        private int maxX;

        /// <summary>
        /// Max y component of Quadrant Array
        /// </summary>
        private int maxY;

        #endregion

        #region Properties

        /// <summary>
        /// Return the name of the world
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public bool QuadrantGenerating
        {
            get
            {
                return m_QuadrantGenerating;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// World constructor
        /// </summary>
        /// <param name="width">The width of the map</param>
        /// <param name="length">The length of the map</param>
        /// <param name="scale">The scale (the size of the cells)</param>
        public World(float scale)
        {
            m_Seed = 0;
            m_Name = null;
            Scale = scale;
            WorldScale = Scale * RateOfChange;
            m_QuadrantGenerating = false;
            m_Initialized = false;
            Initialize();
        }

        /// <summary>
        /// World constructor with a seed
        /// </summary>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="scale"></param>
        /// <param name="seed"></param>
        public World(string name, int scale, int seed)
        {
            m_Seed = seed;
            m_Name = name;
            Scale = scale;
            WorldScale = Scale * RateOfChange;
            PerlinNoise = new PerlinNoise(m_Seed);
            m_QuadrantGenerating = false;
            m_Initialized = false;
        }

        /// <summary>
        /// Initializes the world.
        /// </summary>
        public void Initialize()
        {
            if ( !m_Initialized )
            {
                FloatToType = FloatToType.Instance;
                m_Quadrants = new Quadrant[3, 3];

                for ( int i = 0; i < 3; i++ )
                {
                    for ( int j = 0; j < 3; j++ )
                    {
                        m_Quadrants[i, j] = new Quadrant((int)( i * Scale ), (int)( j * Scale ));
                    }
                }

                maxX = 3;
                maxY = 3;
                m_Initialized = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The size of the Quadrants array
        /// </summary>
        /// <returns></returns>
        public int NumberOfQuadrants()
        {
            return m_Quadrants.Length;
        }

        /// <summary>
        /// Get all the worls stored in the database
        /// </summary>
        /// <returns></returns>
        public static List<World> GetFromDatabase()
        {
            return DatabaseManager.Instance.GetWorlds();
        }

        /// <summary>
        /// Performs update operations for the world.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="camera"></param>
        public void Update(GraphicsDevice device, CameraManager camera)
        {
            if ( !m_QuadrantThread )
            {
                int currentX = (int)Math.Floor(camera.Position.X / Scale / Scale);
                int currentY = (int)Math.Floor(camera.Position.Z / Scale / Scale);
                int startX = currentX - 5;
                int startY = currentY - 5;

                if ( startX > 0 && startY > 0 )
                {
                    for ( int i = 0; i < startX; i++ )
                    {
                        for ( int j = 0; j < startY; j++ )
                        {
                            if ( m_Quadrants[i, j] != null )
                            {
                                m_Quadrants[i, j].Dispose();
                                m_Quadrants[i, j] = null;
                            }
                        }
                    }
                }

                if ( ( currentX + 5 ) < ( maxX - 5 ) && ( currentY + 5 ) < ( maxY - 5 ) )
                {
                    for ( int i = currentX + 5; i < maxX; i++ )
                    {
                        for ( int j = currentY + 5; i < maxY; j++ )
                        {
                            if ( m_Quadrants[i, j] != null )
                            {
                                m_Quadrants[i, j].Dispose();
                                m_Quadrants[i, j] = null;
                            }
                        }
                    }
                }
                                
                ThreadPool.QueueUserWorkItem(CreatingQuadrant, camera);
            }
        }

        /// <summary>
        /// CreatingQuadrant thread method
        /// </summary>
        /// <param name="o"></param>
        public void CreatingQuadrant(object o)
        {
            m_QuadrantThread = true;

            CameraManager camera = o as CameraManager;
            if ( camera == null )
                return;

            int currentX = (int)Math.Floor(camera.Position.X / Scale / Scale);
            int currentY = (int)Math.Floor(camera.Position.Z / Scale / Scale);

            int endX = currentX + 5;
            int endY = currentY + 5;
            bool resizeNeeded = false;
            ResizeQuadrantArray(endX, endY);

            for ( int i = endX - 6; i < endX; i++ )
            {
                for ( int j = endY - 6; j < endY; j++ )
                {
                    try
                    {
                        currentX = (int)Math.Floor(camera.Position.X / Scale / Scale);
                        currentY = (int)Math.Floor(camera.Position.Z / Scale / Scale);

                        int differenceX = Math.Abs(currentX - i);
                        int differenceY = Math.Abs(currentY - j);

                        if ( m_Quadrants[i, j] == null && differenceX <= 4 && differenceY <= 4 )
                        {
                            m_QuadrantGenerating = true;
                            m_Quadrants[i, j] = new Quadrant((int)( i * Scale ), (int)( j * Scale ));
                            m_QuadrantGenerating = false;
                        }
                    }

                    catch ( IndexOutOfRangeException )
                    {
                        resizeNeeded = true;
                        break;
                    }
                }

                if ( resizeNeeded )
                    break;
            }

            if ( resizeNeeded )
                ResizeQuadrantArray(endX + 1, endY + 1);

            m_QuadrantThread = false;
        }

        /// <summary>
        /// Resizes the quadrant array to fit more quadrants!
        /// </summary>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        private void ResizeQuadrantArray(int endX, int endY)
        {
            if ( m_Quadrants.Length < ( endX * endY ) )
            {
                Quadrant[,] quadrants = new Quadrant[endX, endY];

                for ( int i = 0; i < endX; i++ )
                {
                    for ( int j = 0; j < endY; j++ )
                    {
                        try
                        {
                            quadrants[i, j] = m_Quadrants[i, j];
                        }
                        catch
                        {
                            quadrants[i, j] = null;
                        }
                    }
                }

                maxX = endX;
                maxY = endY;
                m_Quadrants = quadrants;
            }
        }

        /// <summary>
        /// Draw the biomes!
        /// </summary>
        /// <param name="device"></param>
        public void Draw(GraphicsDevice device, CameraManager camera)
        {

            #region Dimensions

            int startX = (int)Math.Floor(camera.Position.X / Scale / Scale);
            int startY = (int)Math.Floor(camera.Position.Z / Scale / Scale);
            int endX = 0;
            int endY = 0;

            if ( startX <= 0 )
            {
                startX = 0;
                endX = 3;
            }
            else
            {
                startX--;
                endX = startX + 3;
            }

            if ( startY <= 0 )
            {
                startY = 0;
                endY = 3;
            }
            else
            {
                startY--;
                endY = startY + 3;
            }

            #endregion

            #region Collective Vertice Array

            // Add all vertices of every quadrant being drawn into a collective array
            // n = 90000
            QuadrantVertex[,] collectiveVerticeArray = new QuadrantVertex[( endX - startX ) * Quadrant.QuadrantSize, ( endY - startY ) * Quadrant.QuadrantSize];
            for ( int j = 0; j < endY - startY; j++ )
            {
                for ( int i = 0; i < endX - startX; i++ )
                {
                    try
                    {
                        if (!m_QuadrantGenerating)
                            m_Quadrants[i + startX, j + startY].Update(Environment.TickCount);

                        QuadrantVertex[,] quadrantVertices = m_Quadrants[i + startX, j + startY].Vertices;

                        int xIndex = ( i * Quadrant.QuadrantSize );
                        int yIndex = ( j * Quadrant.QuadrantSize );

                        for ( int k = 0; k < Quadrant.QuadrantSize; k++ )
                        {
                            for ( int l = 0; l < Quadrant.QuadrantSize; l++ )
                            {
                                collectiveVerticeArray[k + xIndex, l + yIndex] = quadrantVertices[k, l];
                            }
                        }
                    }

                    catch ( IndexOutOfRangeException )
                    {

                    }

                    catch ( NullReferenceException )
                    {

                    }
                }
            }

            #endregion

            #region Indices

            // The indices
            // n = 90000
            if ( m_Indices == null )
            {
                m_Indices = new List<int>();

                for ( int y = 0; y < ( ( endX - startX ) * Quadrant.QuadrantSize ) - 1; y++ )
                {
                    for ( int x = 0; x < ( ( endY - startY ) * Quadrant.QuadrantSize ) - 1; x++ )
                    {
                        int first = ( x * ( ( endX - startX ) * Quadrant.QuadrantSize ) ) + y;
                        int second = first + 1;
                        int third = ( ( x + 1 ) * ( ( endX - startX ) * Quadrant.QuadrantSize ) ) + y;
                        int fourth = third + 1;

                        m_Indices.Add(first);
                        m_Indices.Add(second);
                        m_Indices.Add(fourth);

                        m_Indices.Add(first);
                        m_Indices.Add(fourth);
                        m_Indices.Add(third);
                    }
                }
            }

            #endregion

            #region Vertice List

            // Add all vertices in the collectiveVerticeArray into the list
            // n = 90000
            List<QuadrantVertex> vertices = new List<QuadrantVertex>();
            for ( int j = 0; j < ( endY - startY ) * Quadrant.QuadrantSize; j++ )
            {
                for ( int i = 0; i < ( endX - startX ) * Quadrant.QuadrantSize; i++ )
                {
                    vertices.Add(collectiveVerticeArray[i, j]);
                }
            }

            #endregion

            #region Draw

            QuadrantVertex[] verticesArray = vertices.ToArray();
            int[] indicesArray = m_Indices.ToArray();

            VertexBuffer vertexBuffer = new VertexBuffer(device, QuadrantVertex.VertexDeclaration, verticesArray.Length, BufferUsage.None);
            vertexBuffer.SetData<QuadrantVertex>(verticesArray);

            IndexBuffer indexBuffer = new IndexBuffer(device, typeof(int), indicesArray.Length, BufferUsage.None);
            indexBuffer.SetData<int>(indicesArray);

            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verticesArray.Length, 0, indicesArray.Length / 3);

            #endregion

        }

        /// <summary>
        /// Save the World
        /// </summary>
        public bool Save()
        {
            m_Name = InputManager.ShowDialog();

            if ( m_Name != null )
            {
                int WorldId = DatabaseManager.Instance.SaveWorld(m_Name, Scale, PerlinNoise.Seed);
                m_Name = null;
                return true;
            }

            return false;
        }

        #endregion

    }
}
