#region Usings

using System.Collections.Generic;

#endregion

namespace Lang.Utilites
{
    public class ListHelper
    {

        /// <summary>
        /// Merge two lists together.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOne"></param>
        /// <param name="listTwo"></param>
        /// <returns></returns>
        public static void MergeLists<T>(ref List<T> listOne, List<T> listTwo)
        {
            foreach ( T item in listTwo )            
                listOne.Add(item);
        }
    }
}
