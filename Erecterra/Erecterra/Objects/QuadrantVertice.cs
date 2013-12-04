#region Usings

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Lang.Erecterra.Objects
{

    #region QuadrantVerticeType

    /// <summary>
    /// Represents what type of Vertice the QuadrantVertice should use
    /// </summary>
    public enum QuadrantVerticeType
    {
        VERTEX_POSITION_COLOR,
        VERTEX_POSITION_TEXTURE
    }

    #endregion

    public struct QuadrantVertex : IVertexType
    {

        #region Fields

        /// <summary>
        /// Position of the vertex
        /// </summary>
        private Vector3 m_Position;

        /// <summary>
        /// The normal of the vertex (for lighting purposes);
        /// </summary>
        private Vector3 m_Normal;

        /// <summary>
        /// Color of the vertex
        /// </summary>
        private Color m_Color;

        /// <summary>
        /// These are the sizes of the types in the custom vertex declaration. They are added for easy reference
        /// </summary>
        private static int vector3Size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3));
        private static int colorSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Color));
        private static int vector2Size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector2));

        #endregion

        #region Properties

        /// <summary>
        /// Set the color for this vertice
        /// </summary>
        public Color Color
        {
            get
            {
                return m_Color;
            }

            set
            {
                m_Color = value;
            }
        }

        /// <summary>
        /// Set the position of the vertice
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
        /// Set the normal of the vertice
        /// </summary>
        public Vector3 Normal
        {
            get
            {
                return m_Normal;
            }

            set
            {
                m_Normal = value;                
            }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }

        #endregion

        #region IVertexType

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            //position
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            //normal
            new VertexElement(vector3Size, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0),
            //color
            new VertexElement(vector3Size + vector3Size, VertexElementFormat.Color, VertexElementUsage.Color, 0)            
        );

        #endregion

    }
}
