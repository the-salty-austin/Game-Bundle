using System;
using System.Timers;
using System.Threading;

class MainClass
{
    static void Main(string[] args)
    {
        // Rush Hour Maniac
        /*
         *                  DEV LOG
         *
         * 8/15 0922PM: shorten game area to 12 (was 24)
         * (problem: too long)
         * ======
         * 8/16 0202AM: Solve Timeout Input Issue
         * source: https://stackoverflow.com/questions/57615/how-to-add-a-timeout-to-console-readline
         * https://www.codeproject.com/Questions/300894/Convert-Console-Key-to-string-Need-help-very-basic
         * ======
         * 8/17 1228AM: Update to v0.2
         * ======
         * 8/17 1252AM: Update to v0.3
         * ======
         */
        Launch();
        /*
            FUNCTIONALITY PLAN

        v0.0 set up environment [ OK ]
        (game square, score top right)

        v0.1 update game screen
        (use sleep() / clear() )

        v0.2 player movement (WASD) + car crash

        v0.3 Enhance user interface (information)
        v0.3.1 Include Timer

        v0.4 difficulty [not done]
        if score>250 car=2 faster etc.
        */

    }
    static void Launch()
    {
        int idx = 1; // initialize car position
        int score = 0;

        // set up game environment
        char[,] screen = new char[12, 33];
        char[] car = new char[33];
        SetUp(ref screen, ref car);

        // print out game info
        Info();
        // Game begins & save game start time
        DateTime start_time = DateTime.Now;

        while (true)
        {
            int t1 = 0;
            int t2 = 0;
            int x1;
            int y1;
            int x2;
            int y2;
            int dir;
            bool no_exit1;
            bool no_exit2;
            bool no_crash1;
            bool no_crash2;

            TrafficGenerator(screen, out x1, out y1);
            TrafficGenerator(screen, out x2, out y2);

            do
            {
                t1++;
                t2++;
                TrafficMove(screen, x1, y1, ref t1, out no_exit1);
                TrafficMove(screen, x2, y2, ref t2, out no_exit2);

                //PlayerPosition(car,idx); // idx== 0(6) / 1(16) / 2(26) 
                PlayerControl(out dir, ref idx);
                PlayerMovement(ref car, dir, ref idx);
                // Above needs work

                Display(screen, car, score);
                Thread.Sleep(50);
                int tmp1 = y1 + t1;
                int tmp2 = y2 + t2;
                no_crash1 = CrashCheck(x1, tmp1, idx, screen, car, start_time);
                no_crash2 = CrashCheck(x2, tmp2, idx, screen, car, start_time);
                if (!no_crash1 | !no_crash2) break;
                Console.Clear();
                score++;
                // if either of no_exit1&2 is true (car still in screen), stay in do-while loop
                // if crash, quit entire game
            } while (no_exit1 | no_exit2);
            if (!no_crash1 | !no_crash2) break;
        }
        Console.WriteLine("\nFinal Score: {0}", score);
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
        const int width = 33; // 1+9+9+9+1
        const int height = 12;

        /* Generate Lane Line:
         *
         * .|....(6)....|....(16)....|....(26)....|.
         * .|           |            |            |.
         * .|           |            |            |.
         * .|           |            |            |.
         * .|....(6)....|....(16)....|....(26)....|.
         */
        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {

                if (x == 1 | x == 11 | x == 21 | x == 31) { screen[y, x] = '|'; }
                else { screen[y, x] = ' '; }
            }
        }
        // Generate Car Position ( ^^^^^ )
        for (x = 0; x < width; x++)
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
        int[] x_pos = { 6, 16, 26 };
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
                    if (idx <= 1) { pos = x_pos[idx + 1]; idx += 1; }
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
        // car move to new location (car[pos])
        car[pos - 2] = '^';
        car[pos - 1] = '^';
        car[pos    ] = '^';
        car[pos + 1] = '^';
        car[pos + 2] = '^';
    }


    static void TrafficGenerator(char[,] screen, out int x, out int y)
    {
        int[] traffic_xpos = { 6, 16, 26 };
        int[] traffic_ypos = { -15, -10, -5, 0 };
        Random rand = new Random();

        // randomly generate (x,y) coordinate of oncoming traffic
        x = traffic_xpos[rand.Next() % 3];
        y = traffic_ypos[rand.Next() % 4];
    }


    static void TrafficMove(char[,] screen, int x, int y, ref int t, out bool exit)
    {
        int i = -1;
        int j = -1;
        y += t;
        exit = true;
        // small vehicle
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
        if (i == 12) exit = false;
    }


    static bool CrashCheck(int x, int y, int car_idx, char[,] screen, char[] car, DateTime start_time)
    {
        // if continue_game == false, game terminates
        bool continue_game = true;
        int[] x_pos = { 6, 16, 26 };
        int car_pos = x_pos[car_idx];
        if (x == car_pos & (y >= 10 & y <= 13))
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
        for (int y = 0; y < 12; y++)
        {
            for (int x = 0; x < 33; x++)
            {
                Console.Write(screen[y, x]);
            }
            Console.WriteLine("");
        }
        Console.WriteLine("");

        // Display car (player)
        for (int x = 0; x < 33; x++)
        {
            Console.Write(car[x]);
        }
        Console.WriteLine("\nScore: {0}", score);

    }


    static bool Probability(int percent)
    {
        bool prob = true;
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
