using System;
using GameBundle;

/*
 * 1 DB_SetUp() Establish database for game data save
 *
 * 2 SelectMode() User chooses functionality of this application
 *     
 *     | [1] Create Account ( CreateAccount() )
 *     | [2] Play Games
 *     |    [21] Rush Hour Maniac
 *     |    [22] Connect Four
 *     |    [23] Reversi
 *     |    [24] Sniper
 *     | [3] View Personal Scores
 *     | [4] View Leaderboard
 *     |    [41] Rush Hour Maniac Leaderboard
 *     |    [42] Connect Four Leaderboard
 *     |    [43] Reversi Leaderboard
 *     |    [44] Sniper Leaderboard
 *     
 * 2a   If user has chosen "[2] Play Games", "[3] View Personal Scores" or "[4] View Leaderboard",
 *    this application first authenticates the user's identity. ( Authentication() )
 *      If user is authenticated, user is granted access. ( GameLaunch() )
 *    Otherwise, user has to re-enter his/her username and password.
 *
 */
class MainClass
{
    static void Main(string[] args)
    {
        GameBundle.Database dtb = new Database();
        GameBundle.RushHourManiac rhm = new RushHourManiac();
        GameBundle.ConnectFour cn4 = new ConnectFour();
        GameBundle.Reversi rvs = new Reversi();
        GameBundle.Sniper snp = new Sniper();

        int rhm_score = 0;
        int cn4_score = 0;
        int rvs_score = 0;
        int snp_score = 0;
        int user_id = -1;
        bool auth = false;
        string continue_app = "Y";

        // 1 Establish game save database
        dtb.DB_SetUp();

        // 2 Select Functionality
        do
        {
            Console.Clear();
            int mode_selected = 0;

            while (mode_selected == 0)
            {
                mode_selected = SelectMode();
            }

            if (mode_selected == 1)
            {
                dtb.CreateAccount();
            }
            else if (mode_selected == 21 | mode_selected == 22 | mode_selected == 23 | mode_selected == 24 | mode_selected == 3 | mode_selected == 41| mode_selected == 42 | mode_selected == 43 | mode_selected == 44)
            {
                auth = dtb.Authentication(ref user_id);
                if (auth)
                {
                    if (mode_selected == 21) { rhm.Launch(ref rhm_score); dtb.SaveHighScore(rhm_score, 1, user_id); }
                    if (mode_selected == 22) { cn4.Launch(ref cn4_score); dtb.SaveHighScore(cn4_score, 2, user_id); }
                    if (mode_selected == 23) { rvs.Launch(ref rvs_score); dtb.SaveHighScore(rvs_score, 3, user_id); }
                    if (mode_selected == 24) { snp.Launch(ref snp_score); dtb.SaveHighScore(snp_score, 4, user_id); }
                    if (mode_selected == 3) { dtb.GetPersonalScore(user_id); }
                    if (mode_selected >= 41 &mode_selected<=44) { dtb.GetLeaderboard(mode_selected); }
                }
                else if (!auth)
                {
                    Console.WriteLine(" [ Warning ] Access denied. (Unauthenticated user profile)");
                }
            }
            Console.Clear();
            Console.WriteLine("[Enter] Continue Application\n[Q] Quit Application");
            continue_app = Console.ReadLine();
            Console.Clear();
        } while (continue_app=="");
    }


    static int SelectMode()
    {
        string mode;
        int choice = -1;

        Console.WriteLine("*** Select Mode ***");
        Console.WriteLine("[1] Create Account\n[2] Play Games\n[3] View Personal Scores\n[4] View Leaderboard");
        mode = Console.ReadLine();
        Console.Clear();
        switch (mode)
        {
            case "1":
                {
                    choice = 1;
                    break;
                }
            case "2":
                {
                    Console.WriteLine("*** Select Game ***");
                    Console.WriteLine("[1] Rush Hour Maniac\n[2] Connect Four\n[3] Reversi\n[4] Sniper");
                    string sub_mode = Console.ReadLine();
                    switch (sub_mode)
                    {
                        case "1":
                            {
                                choice = 21;
                                break;
                            }
                        case "2":
                            {
                                choice = 22;
                                break;
                            }
                        case "3":
                            {
                                choice = 23;
                                break;
                            }
                        case "4":
                            {
                                choice = 24;
                                break;
                            }
                        default: Console.WriteLine("Game does not exist."); choice = 0; break;
                    }
                    break;
                }
            case "3":
                {
                    choice = 3;
                    break;
                }
            case "4":
                {
                    Console.WriteLine("*** Select Game's Leaderboard ***");
                    Console.WriteLine("[1] Rush Hour Maniac\n[2] Connect Four\n[3] Reversi\n[4] Sniper");
                    string sub_mode = Console.ReadLine();
                    switch (sub_mode)
                    {
                        case "1":
                            {
                                choice = 41;
                                break;
                            }
                        case "2":
                            {
                                choice = 42;
                                break;
                            }
                        case "3":
                            {
                                choice = 43;
                                break;
                            }
                        case "4":
                            {
                                choice = 44;
                                break;
                            }
                        default: { Console.WriteLine("Game does not exist."); choice = 0; break; }
                    }
                    break;
                }
            default: Console.WriteLine("Mode does not exist."); choice = 0; break;
        }
        return choice;
    }
}
