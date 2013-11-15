using System;

namespace ProBendingBattle
{

#if WINDOWS || XBOX
    static class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if WINDOWS
            using (Game1 game = new Game1(true))
#else
            using (Game1 game = new Game1(false))
#endif
            {
                game.Run();
            }
        }
    }
#endif
}
