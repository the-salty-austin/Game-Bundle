using System;
using System.Linq;
using System.Timers;
using System.Threading;

namespace GameBundle
{
    class RushHourManiac
    {
        public void Launch(ref int score)
        {
            // [ Step 0-1 ] initialize car position at x=16 (2nd lane)
            int idx = 1; 

            // [ Step 0-2 ] set up game environment
            char[,] screen = new char[12, 60]; // display area == [12,42]
            char[] car = new char[42];
            SetUp(ref screen, ref car);

            // [ Step 1 ]
            // print out game info and instructions
            Info();

            // record time when game starts
            DateTime start_time = DateTime.Now;

            while (true)
            {
                int t = 0;
                int x1 = 50;
                int y1 = 8;
                int x2 = 50;
                int y2 = 8;
                int x3 = 50;
                int y3 = 8;
                int dir;
                bool no_exit1;
                bool no_exit2;
                bool no_exit3;
                bool no_crash1;
                bool no_crash2;
                bool no_crash3;

                // [ Step 2 ]
                /* 
                 * Generates 2~3 vehicles on the highway.
                 *
                 * The last parameter in TrafficGenerator()
                 * is the probability that the vehicle spawns.
                 *
                 * All vehicles spawn above the display area.
                 */
                TrafficGenerator(screen, ref x1, ref y1, 100);
                TrafficGenerator(screen, ref x2, ref y2, 100);
                TrafficGenerator(screen, ref x3, ref y3,  70);
                
                do
                {
                    // [ Step 3 ]
                    /*
                     * For every loop of this do-while loop,
                     * the variable "t" increases by 1, which
                     * moves down the vehicles on the highway by one pixel. 
                     *
                     * The "no_exit" variables are used at Step 7
                     * to determine whether the vehicle has exited the display area.
                     */
                    t++;
                    TrafficMove(screen, x1, y1, t, out no_exit1);
                    TrafficMove(screen, x2, y2, t, out no_exit2);
                    TrafficMove(screen, x3, y3, t, out no_exit3);

                    // [ Step 4 ]
                    /*
                     * The player then inputs [A], [D] or nothing
                     * to control their own vehicle to avoid collision
                     * with other vehicles on the highway.
                     */
                    PlayerControl(out dir, ref idx);
                    PlayerMovement(ref car, dir, ref idx);

                    // [ Step 5 ]
                    /*
                     * First, erase all previous display. Then, display current
                     * positions of traffic on the highway and player themself.
                     * 
                     * This display lasts 150~250ms before the next frame appears.
                     */
                    Display(screen, car, score);
                    Thread.Sleep(50);
                    int tmp1 = y1 + t;
                    int tmp2 = y2 + t;
                    int tmp3 = y3 + t;
                    // [ Step 6 ]
                    /*
                     * Checks whether player with crashed into any vehicle.
                     * 
                     * If player has crashed, the game stops and displays
                     * (a) player's final score and (b) the time length that the player
                     * has survived on the highway.
                     */
                    no_crash1 = CrashCheck(x1, tmp1, idx, screen, car, start_time);
                    no_crash2 = CrashCheck(x2, tmp2, idx, screen, car, start_time);
                    no_crash3 = CrashCheck(x3, tmp3, idx, screen, car, start_time);
                    if (!no_crash1 | !no_crash2 | !no_crash3) break;
                    Console.Clear();
                    score++;

                    // [ Step 7 ]
                    /* 
                     * If any of the vehicles is still inside the display area,
                     * the game enters the next do-while loop (Back to Step 3).
                     *
                     * If all vehicles have left the display area,
                     * the game quits the do-while loop (Back to Step 2).
                     */
                } while ( no_exit1 | no_exit2 | no_exit3);
                if (!no_crash1 | !no_crash2 | !no_crash3) break;
            }
            Console.WriteLine("\nFinal Score: {0}", score);
            Thread.Sleep(5000);
            Console.Clear();
        }


        static void Info()
        {
            Console.WriteLine("\n\n   ********************");
            Console.WriteLine("   * Rush Hour Maniac *");
            Console.WriteLine("   ********************");
            Thread.Sleep(1250);
            Console.Clear();
            Console.WriteLine("\n** Welcome to Rush Hour Manic **\n  ");
            Console.WriteLine("You're in a hurry, but you're now");
            Console.WriteLine("driving at rush hour on a jammed");
            Console.WriteLine("highway. Maneuver your vehicle and");
            Console.WriteLine("AVOID CRASHING YOUR CAR!!!\n");
            Console.WriteLine("Press [A] to steer LEFT.");
            Console.WriteLine("Press [D] to steer RIGHT.");
            Console.WriteLine("\nPress [Enter] to begin game.");
            Console.ReadLine();
            Console.Clear();
        }

        static void SetUp(ref char[,] screen, ref char[] car)
        {
            /*
            Traffic car size: Small 3x4 / Large 5x7
            *** Large Vehicle is not yet implemented ***

            */
            int y = -1;
            int x = -1;
            const int width = 60; // real width=33 1+9+9+9+9+1
            const int height = 12;

            /* Generate Lane Line:
            *
            * .|....(6)....|....(16)....|....(26)....|....(36)....|.  // true edge at x=42
            * .|           |            |            |            |.
            * .|           |            |            |            |.
            * .|           |            |            |            |.
            * .|....(6)....|....(16)....|....(26)....|....(36)....|.
            */
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {

                    if ( x == 1 | x == 11 | x == 21 | x == 31 | x == 41 ) { screen[y, x] = '|'; }
                    else { screen[y, x] = ' '; }
                }
            }
            // Generate Car Position ( ^^^^^ )
            for (x = 0; x < 42; x++)
            {
                car[x] = ' ';
            }
        }


        static void PlayerControl(out int dir, ref int idx)
        {
            // Press any key in the next 150ms. Otherwise, input is ignored
            ConsoleKeyInfo k = new ConsoleKeyInfo();
            for (int cnt = 3; cnt > 0; cnt--)
            {
                if (Console.KeyAvailable)
                {
                    k = Console.ReadKey();
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
            switch (k.Key.ToString())
            {
                case "A": dir = 1; break;
                case "D": dir = 2; break;
                default: dir = 0; break;
            }
        }


        static void PlayerMovement(ref char[] car, int dir, ref int idx)
        {
            int[] x_pos = { 6, 16, 26, 36 };
            int pos = x_pos[idx];

            // Erase Old car Position
            car[pos - 2] = ' ';
            car[pos - 1] = ' ';
            car[pos] = ' ';
            car[pos + 1] = ' ';
            car[pos + 2] = ' ';
            switch (dir)
            {
                case 1:
                    {
                        if (idx >= 1) { pos = x_pos[idx - 1]; idx -= 1; }
                        else { pos = x_pos[idx]; }
                        break; // move LEFT
                    }
                case 2:
                    {
                        if (idx <= 2) { pos = x_pos[idx + 1]; idx += 1; }
                        else { pos = x_pos[idx]; }
                        break; // move RIGHT
                    }
                case 0:
                    {
                        pos = x_pos[idx + 0];
                        idx += 0;
                        break; // NO move
                    }
            }
            // player car moves to new location (car[pos])
            car[pos - 2] = '^';
            car[pos - 1] = '^';
            car[pos] = '^';
            car[pos + 1] = '^';
            car[pos + 2] = '^';
        }


        static void TrafficGenerator(char[,] screen, ref int x, ref int y, int percent)
        {
            bool generate = Probability(percent);

            if (generate)
            {
                int[] traffic_xpos = { 6, 16, 26, 36 };
                int[] traffic_ypos = { -12, -10, -8, -6, -4 };
                Random rand = new Random();

                // randomly generate (x,y) coordinate where traffic spawns
                x = traffic_xpos[rand.Next() % 4];
                y = traffic_ypos[rand.Next() % 5];
            }
            else if (!generate)
            {
                // If no-generate, generate traffic outside display area (x>43).
                x = 50;
                y = 8;
            }
        }


        static void TrafficMove(char[,] screen, int x, int y, int t, out bool no_yet_exit)
        {
            int i = -1;
            int j = -1;
            y += t;
            if (x==50) {no_yet_exit = false;}
            else 
            {
                no_yet_exit = true;
                for (i = y - 2; i <= y + 1; i++)
                {
                    // if vehicle not yet enter, no action.
                    // if vehicle exit, no action
                    if (i < 0 | i > 11) break;
                    
                    // if vehicle in screen >> display()
                    for (j = x - 1; j <= x + 1; j++)
                    {
                        screen[i, j] = '*'; // (y,x)
                    }
                }
                for (i = 0; i < 12; i++)
                {
                    if (i >= y - 2 & i <= y + 1) break;
                    
                    // erase vehicle prev position
                    for (j = x - 1; j <= x + 1; j++)
                    {
                        screen[i, j] = ' '; // (y,x)
                    }
                }
                if (y >= 13) {no_yet_exit = false;}
            }
        }


        static bool CrashCheck(int x, int y, int car_idx, char[,] screen, char[] car, DateTime start_time)
        {
            // if continue_game == false, game terminates
            bool continue_game = true;
            int[] x_pos = { 6, 16, 26, 36 };
            int car_pos = x_pos[car_idx];
            if (x == 50) 
            {
                continue_game = true;
            }
            else if (x == car_pos & (y >= 10 & y <= 12))
            {
                int game_time = TimeElapsed(start_time);

                continue_game = false;
                Thread.Sleep(3000);
                Console.Clear();
                Console.WriteLine("\n\n       *************");
                Console.WriteLine("       * CAR CRASH *");
                Console.WriteLine("       *************");
                Console.WriteLine("\n  You survived on the highway");
                Console.WriteLine("for {0} seconds.", game_time);
            }
            return continue_game;
        }


        static void Display(char[,] screen, char[] car, int score)
        {
            // Display screen (traffic)
            Console.WriteLine("");
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 42; x++)
                {
                    Console.Write(screen[y, x]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");

            // Display car (player)
            for (int x = 0; x < 42; x++)
            {
                Console.Write(car[x]);
            }
            Console.WriteLine("\nScore: {0}", score);

        }


        static bool Probability(int percent)
        {
            bool prob = false;
            if (percent == 100) { prob = true; }

            else if (percent < 100)
            {
                Random rand = new Random();
                int[] array = new int[percent];
                for (int i = 0; i < percent; i++)
                {
                    array[i] = rand.Next(1, 101);
                    for (int j = 0; j < i; j++)
                    {
                        while (array[i] == array[j])
                        {
                            array[i] = rand.Next(1, 101);
                        }
                    }
                }
                int key = rand.Next(1, 101);
                if (array.Contains(key))
                {
                    prob = true;
                }
            }
            return prob;
        }


        static int TimeElapsed(DateTime start_time)
        {
            int t1 = TimeParse(start_time);
            int t2 = TimeParse(DateTime.Now);
            return t2 - t1;
        }


        static int TimeParse(DateTime time)
        {
            int h = int.Parse(time.ToString("HH"));
            h *= 3600000;
            int m = int.Parse(time.ToString("mm"));
            m *= 60000;
            int s = int.Parse(time.ToString("ss"));
            s *= 1000;
            int f = int.Parse(time.ToString("fff"));
            int t = h + m + s + f;
            t /= 1000;
            return t;
        }
    }
}