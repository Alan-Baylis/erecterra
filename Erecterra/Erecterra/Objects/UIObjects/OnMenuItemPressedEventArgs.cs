#region Usings

using System; 

#endregion

namespace Lang.Erecterra.Objects.UIObjects
{
    public class OnMenuItemPressedEventArgs : EventArgs
    {

        #region Fields

        private string m_Name;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public OnMenuItemPressedEventArgs(string name)
        {
            m_Name = name;
        }

        #endregion

    }
}
