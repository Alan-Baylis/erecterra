#region Usings

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; 

#endregion

namespace Lang.Erecterra.Objects
{
    public class Sphere
    {

        #region Fields

        /// <summary>
        /// The vertices
        /// </summary>
        private QuadrantVertex[] m_Vertices;

        /// <summary>
        /// The indices
        /// </summary>
        private int[] m_Indices;

        /// <summary>
        /// Radius
        /// </summary>
        private float m_Radius;

        /// <summary>
        /// Resolution (size)
        /// </summary>
        private int m_Resolution;

        /// <summary>
        /// Position of the spehere
        /// </summary>
        private Vector3 m_Position;

        /// <summary>
        /// Color of the sphere
        /// </summary>
        private Color m_Color;

        /// <summary>
        /// Visible or not
        /// </summary>
        private bool m_Visible;

        #endregion

        #region Properties

        /// <summary>
        /// Get the position
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        /// <summary>
        /// Get the Sphere visibility
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_Visible;
            }

            set
            {
                m_Visible = value;
            }
        }

        #endregion        

        #region Constructor

        /// <summary>
        /// Constructor for the sphere
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="colour"></param>
        /// <param name="resolution"></param>
        public Sphere(Vector3 position, float radius, Color colour, int resolution = 10)
        {
            m_Position = position;
            m_Radius = radius;
            m_Resolution = resolution;
            m_Color = colour;
            m_Visible = true;
            calculateVertices();
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Calculate the vertices
        /// </summary>
        public void calculateVertices()
        {
            m_Vertices = new QuadrantVertex[( m_Resolution + 1 ) * ( m_Resolution + 1 )];

            float phi, theta;
            float dphi = MathHelper.Pi / m_Resolution;
            float dtheta = MathHelper.TwoPi / m_Resolution;
            float x, y, z, sc;
            int index = 0;

            for ( int stack = 0; stack <= m_Resolution; stack++ )
            {
                phi = MathHelper.PiOver2 - stack * dphi;
                y = m_Radius * (float)Math.Sin(phi);
                sc = -m_Radius * (float)Math.Cos(phi);

                for ( int slice = 0; slice <= m_Resolution; slice++ )
                {
                    theta = slice * dtheta;
                    x = sc * (float)Math.Sin(theta);
                    z = sc * (float)Math.Cos(theta);
                    m_Vertices[index] = new QuadrantVertex();
                    m_Vertices[index].Color = m_Color;
                    m_Vertices[index].Position = new Vector3(x, y, z);
                    index++;
                }
            }

            m_Indices = new int[m_Resolution * m_Resolution * 6];
            index = 0;
            int k = m_Resolution + 1;

            for ( int stack = 0; stack < m_Resolution; stack++ )
            {
                for ( int slice = 0; slice < m_Resolution; slice++ )
                {
                    m_Indices[index++] = ( stack + 0 ) * k + slice;
                    m_Indices[index++] = ( stack + 1 ) * k + slice;
                    m_Indices[index++] = ( stack + 0 ) * k + slice + 1;

                    m_Indices[index++] = ( stack + 0 ) * k + slice + 1;
                    m_Indices[index++] = ( stack + 1 ) * k + slice;
                    m_Indices[index++] = ( stack + 1 ) * k + slice + 1;
                }
            }
        }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="device"></param>
        /// <param name="effect"></param>
        public void Draw(GraphicsDevice device, Effect effect)
        {
            effect.Parameters["World"].SetValue(Matrix.CreateTranslation(m_Position));

            foreach ( EffectPass pass in effect.CurrentTechnique.Passes )
            {
                pass.Apply();

                VertexBuffer vertexBuffer = new VertexBuffer(device, QuadrantVertex.VertexDeclaration, m_Vertices.Length, BufferUsage.None);
                vertexBuffer.SetData<QuadrantVertex>(m_Vertices);

                IndexBuffer indexBuffer = new IndexBuffer(device, typeof(int), m_Indices.Length, BufferUsage.None);
                indexBuffer.SetData<int>(m_Indices);

                device.Indices = indexBuffer;
                device.SetVertexBuffer(vertexBuffer);
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_Vertices.Length, 0, m_Indices.Length / 3);
            }
        } 

        #endregion

    }
}
