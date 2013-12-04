#region Usings

using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using Lang.Erecterra.Objects;

#endregion

namespace Lang.Erecterra.Components
{
    public class DatabaseManager
    {

        #region Fields

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static DatabaseManager m_Instance;

        /// <summary>
        /// Sync object used to ensure that only one instance is created across multiple threads
        /// </summary>
        public static object s_SyncRoot = new Object();

        /// <summary>
        /// Name of the database
        /// </summary>
        public static string DATABASE_NAME = "Erecterra";

        /// <summary>
        /// Sql Compact Edition connection
        /// </summary>
        private SqlCeConnection m_Connection;

        #endregion

        #region Properties

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static DatabaseManager Instance
        {
            get
            {
                if ( m_Instance == null )
                {
                    // use lock for multi-threaded singleton
                    lock ( s_SyncRoot )
                    {
                        // additional check for performance reasons
                        if ( m_Instance == null )
                            m_Instance = new DatabaseManager();

                        return m_Instance;
                    }
                }
                else
                {
                    return m_Instance;
                }
            }
        }

        #endregion

        #region Private Constructor

        /// <summary>
        /// Private singleton Constructor
        /// </summary>
        private DatabaseManager()
        {
            m_Connection = new SqlCeConnection("Data Source=Erecterra.sdf;");
        }

        #endregion

        #region Method

        /// <summary>
        /// Get the worlds from the database
        /// </summary>
        /// <returns></returns>
        public List<World> GetWorlds()
        {
            List<World> worlds = new List<World>();

            m_Connection.Open();

            string sqlCommand = "SELECT * FROM Worlds ORDER BY name ASC";
            SqlCeCommand command = m_Connection.CreateCommand();
            command.CommandText = sqlCommand;
            SqlCeResultSet reader = command.ExecuteResultSet(ResultSetOptions.Scrollable);

            int name = reader.GetOrdinal("name");            
            int scale = reader.GetOrdinal("scale");
            int seed = reader.GetOrdinal("seed");

            while ( reader.Read() )
            {
                string nameValue = (string)reader.GetValue(name);                
                int scaleValue = Int32.Parse(reader.GetValue(scale).ToString());
                int seedValue = Int32.Parse(reader.GetValue(seed).ToString());
                worlds.Add(new World(nameValue, scaleValue, seedValue));
            }

            m_Connection.Close();

            return worlds;
        }

        /// <summary>
        /// Save the World
        /// </summary>
        /// <param name="name"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public int SaveWorld(string name, float scale, int seed)
        {
            int WorldId = 0;

            try
            {
                m_Connection.Open();

                string sqlCommand = string.Format("SELECT id FROM Worlds WHERE seed = {0};", seed);
                SqlCeCommand command = m_Connection.CreateCommand();
                command.CommandText = sqlCommand;
                SqlCeResultSet reader = command.ExecuteResultSet(ResultSetOptions.Scrollable);

                if ( reader.HasRows )
                {
                    reader.Read();
                    WorldId = reader.GetInt32(0);
                    reader.Close();
                }
                else
                {
                    //new world -- insert it
                    sqlCommand = string.Format("INSERT INTO Worlds (name, scale, seed) VALUES ('{0}', {1}, {2});", name, scale, seed);
                    command = m_Connection.CreateCommand();
                    command.CommandText = sqlCommand;
                    command.ExecuteNonQuery();

                    command = m_Connection.CreateCommand();
                    command.CommandText = "SELECT @@identity;";
                    WorldId = Convert.ToInt32(command.ExecuteScalar().ToString());
                }
            }
            catch
            {

            }
            finally
            {
                m_Connection.Close();
            }

            return WorldId;
        }

        #endregion

    }
}
