using System;

namespace Lang.Erecterra {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Erecterra game = new Erecterra())
            {
                game.Run();
            }
        }
    }
#endif
}

