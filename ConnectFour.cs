using System;
using System.Threading;

namespace GameBundle
{
    class ConnectFour
    {
        public void Launch(ref int WinOrLose)
        {
            //規則說明
            Console.WriteLine("歡迎遊玩Connect Four! 以下為規則說明:");
            Console.WriteLine("本遊戲由雙方輪流選擇7道隔間中的其中一道落子，所下的棋子會以下落的方式掉至底部。");
            Console.WriteLine("若落子時底下已有棋子，則會堆疊於其上。");
            Console.WriteLine("獲勝條件為其中一方以4個棋子橫、豎或斜線的連成一線。\n");
            //模式選擇
            Console.WriteLine("請選擇遊戲模式:");
            Console.Write("人機對戰請輸入1，玩家對戰請輸入2: ");
            string mode = Console.ReadLine();
            while (mode != "1" && mode != "2")
            {
                Console.WriteLine("輸入有誤。請重新選擇遊戲模式:");
                Console.Write("人機對戰請輸入1，玩家對戰請輸入2: ");
                mode = Console.ReadLine();
            }

            int DropPosition; //落子點
            char[,] board = {{' ',' ',' ',' ',' ',' '},
                             {' ',' ',' ',' ',' ',' '},
                             {' ',' ',' ',' ',' ',' '},
                             {' ',' ',' ',' ',' ',' '},
                             {' ',' ',' ',' ',' ',' '},
                             {' ',' ',' ',' ',' ',' '},
                             {' ',' ',' ',' ',' ',' '}}; //棋盤二維陣列
            bool WinCondition = false; //勝利判斷
            int turn = 0; //回合數

            PrintInitialBoard(board);

            if (mode == "1")
                while (true)
                {
                    //玩家回合:
                    //落子                
                    Console.Write("玩家的回合\n請選擇落子處(1~7):");
                    while (true)
                    {
                        try
                        {
                            DropPosition = Convert.ToInt16(Console.ReadLine()) - 1;
                        }
                        catch (FormatException)
                        {
                            Console.Write("輸入有誤。請重新選擇落子處(1~7):");
                            continue;
                        }
                        if ((DropPosition != 0) && (DropPosition != 1) && (DropPosition != 2) && (DropPosition != 3) && (DropPosition != 4) && (DropPosition != 5) && (DropPosition != 6))
                        {
                            Console.Write("輸入有誤。請重新選擇落子處(1~7):");
                            continue;
                        }
                        if (board[DropPosition, 0] != ' ')
                        {
                            Console.Write("此排已滿，請選擇其他排落子。\n請重新選擇落子處(1~7):");
                            continue;
                        }
                        break;
                    }

                    //印出board
                    PrintBoard(1, DropPosition, ref board);
                    turn++;

                    //判斷是否結束
                    WinCondition = (winner(board) == 0 || winner(board) == 1);
                    if (WinCondition == true || turn == 42)
                        break;

                    //電腦回合:
                    //落子
                    Console.WriteLine("電腦的回合");
                    Thread.Sleep(1000);
                    DropPosition = cpu_turn(board);

                    //印出board
                    PrintBoard(0, DropPosition, ref board);
                    turn++;

                    //判斷是否結束
                    WinCondition = (winner(board) == 0 || winner(board) == 1);
                    if (WinCondition == true || turn == 42)
                        break;
                }

            if (mode == "2")
                while (true)
                {
                    //玩家1回合
                    Console.Write("玩家1的回合\n請選擇落子處(1~7):");
                    while (true)
                    {
                        try
                        {
                            DropPosition = Convert.ToInt16(Console.ReadLine()) - 1;
                        }
                        catch (FormatException)
                        {
                            Console.Write("輸入有誤。請重新選擇落子處(1~7):");
                            continue;
                        }
                        if ((DropPosition != 0) && (DropPosition != 1) && (DropPosition != 2) && (DropPosition != 3) && (DropPosition != 4) && (DropPosition != 5) && (DropPosition != 6))
                        {
                            Console.Write("輸入有誤。請重新選擇落子處(1~7):");
                            continue;
                        }
                        if (board[DropPosition, 0] != ' ')
                        {
                            Console.Write("此排已滿，請選擇其他排落子。\n請重新選擇落子處(1~7):");
                            continue;
                        }
                        break;
                    }
                    PrintBoard(1, DropPosition, ref board);
                    turn++;

                    WinCondition = (winner(board) == 2 || winner(board) == 1);
                    if (WinCondition == true || turn == 42)
                        break;

                    //玩家2回合
                    Console.Write("玩家2的回合\n請選擇落子處(1~7):");
                    while (true)
                    {
                        try
                        {
                            DropPosition = Convert.ToInt16(Console.ReadLine()) - 1;
                        }
                        catch (FormatException)
                        {
                            Console.Write("輸入有誤。請重新選擇落子處(1~7):");
                            continue;
                        }
                        if ((DropPosition != 0) && (DropPosition != 1) && (DropPosition != 2) && (DropPosition != 3) && (DropPosition != 4) && (DropPosition != 5) && (DropPosition != 6))
                        {
                            Console.Write("輸入有誤。請重新選擇落子處(1~7):");
                            continue;
                        }
                        if (board[DropPosition, 0] != ' ')
                        {
                            Console.Write("此排已滿，請選擇其他排落子。\n請重新選擇落子處(1~7):");
                            continue;
                        }
                        break;
                    }
                    PrintBoard(2, DropPosition, ref board);
                    turn++;

                    WinCondition = (winner(board) == 2 || winner(board) == 1);
                    if (WinCondition == true || turn == 42)
                        break;
                }

            //依勝者印出結果
            switch (winner(board))
            {
                case 0:
                    Console.WriteLine("\n遊戲結束，電腦獲勝!");
                    Console.ReadKey();
                    WinOrLose = -1;
                    break;

                case 1:
                    if (mode == "1")
                    {
                        Console.WriteLine("\n遊戲結束，玩家獲勝!");
                        Console.ReadKey();
                        WinOrLose = 1;
                    }
                    else
                    {
                        Console.WriteLine("\n遊戲結束，玩家1獲勝!");
                        Console.ReadKey();
                        WinOrLose = 1;
                    }
                    break;

                case 2:
                    {
                        Console.WriteLine("\n遊戲結束，玩家2獲勝!");
                        Console.ReadKey();
                        WinOrLose = -1;
                    }
                    break;

                default:
                    {
                        Console.WriteLine("\n遊戲結束，雙方平手!");
                        Console.ReadKey();
                        WinOrLose = 0;
                    }
                    break;
            }
        }

        static void PrintBoard(int PlayerNumber, int DropPosition, ref char[,] board)
        {
            char symbol; //玩家或電腦代表符號
            switch (PlayerNumber)
            {
                case 0:
                    symbol = 'X';
                    break;

                case 1:
                    symbol = '●';
                    break;

                case 2:
                    symbol = '○';
                    break;

                default:
                    symbol = ' ';
                    break;
            }

            //找到第一個空格，並換為代表符號
            int DropHeight = 0;
            for (int j = 5; j >= 0; j--)
                if (board[DropPosition, j] == ' ')
                {
                    board[DropPosition, j] = symbol;
                    DropHeight = j;
                    break;
                }

            Console.Clear();
            Console.SetCursorPosition(0, 16);

            //印出橫坐標
            Console.WriteLine("   1   2   3   4   5   6   7\n");
            //以列處理印出縱座標與board
            for (int j = 0; j < 6; j++)
            {
                Console.Write("{0}  ", (j + 1));
                for (int i = 0; i < 7; i++)
                {
                    Console.SetCursorPosition((4 * (i + 1) - 1), (16 + 2 * (j + 1)));
                    if (i == DropPosition && j == DropHeight)
                        Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("{0}", board[i, j]);
                    Console.ResetColor();
                }
                Console.Write("\n\n");
            }
            Console.SetCursorPosition(0, 0);
        }

        static void PrintInitialBoard(char[,] board)
        {

            Console.Clear();
            Console.SetCursorPosition(0, 16);

            //印出橫坐標
            Console.WriteLine("   1   2   3   4   5   6   7\n");
            //以列處理印出縱座標與board
            for (int j = 0; j < 6; j++)
            {
                Console.Write("{0}  ", (j + 1));
                for (int i = 0; i < 7; i++)
                {
                    Console.SetCursorPosition((4 * (i + 1) - 1), (16 + 2 * (j + 1)));
                    Console.Write("{0}", board[i, j]);
                }
                Console.Write("\n\n");
            }
            Console.SetCursorPosition(0, 0);
        }

        static int winner(char[,] board)
        {
            char WinnerSymbol = ' ';

            //橫向確認
            for (int j = 5; j >= 0; j--)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (board[i, j] != ' ' &&
                       board[i, j] == board[(i + 1), j] &&
                       board[(i + 1), j] == board[(i + 2), j] &&
                       board[(i + 2), j] == board[(i + 3), j])
                    {
                        WinnerSymbol = board[i, j];
                        break;
                    }
                }
                if (WinnerSymbol != ' ')
                    break;
            }

            //縱向確認
            if (WinnerSymbol == ' ')
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 5; j >= 3; j--)
                    {
                        if (board[i, j] != ' ' &&
                           board[i, j] == board[i, (j - 1)] &&
                           board[i, (j - 1)] == board[i, (j - 2)] &&
                           board[i, (j - 2)] == board[i, (j - 3)])
                        {
                            WinnerSymbol = board[i, j];
                            break;
                        }
                    }
                    if (WinnerSymbol != ' ')
                        break;
                }

            //斜向確認
            if (WinnerSymbol == ' ')
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        if (((i + 3) > 6) || ((j + 3) > 5))
                            continue;

                        if (board[i, j] != ' ' &&
                           board[i, j] == board[(i + 1), (j + 1)] &&
                           board[(i + 1), (j + 1)] == board[(i + 2), (j + 2)] &&
                           board[(i + 2), (j + 2)] == board[(i + 3), (j + 3)])
                        {
                            WinnerSymbol = board[i, j];
                            break;
                        }
                    }
                }

            if (WinnerSymbol == ' ')
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (((i + 3) > 6) || ((j - 3) < 0))
                            continue;

                        if (board[i, j] != ' ' &&
                           board[i, j] == board[(i + 1), (j - 1)] &&
                           board[(i + 1), (j - 1)] == board[(i + 2), (j - 2)] &&
                           board[(i + 2), (j - 2)] == board[(i + 3), (j - 3)])
                        {
                            WinnerSymbol = board[i, j];
                            break;
                        }

                    }
                }

            //回傳勝利者:0為電腦，1為「玩家1」，2為「玩家2」，99代表無勝利者
            switch (WinnerSymbol)
            {
                case 'X':
                    return 0;

                case '●':
                    return 1;

                case '○':
                    return 2;

                default:
                    return 99;
            }
        }

        static int cpu_turn(char[,] InitialBoard)
        {
            //定位可能落子空格:board[i,AvalibleSpot[i]]
            char[] AvalibleSpot = { 'N', 'N', 'N', 'N', 'N', 'N', 'N' };
            for (int i = 0; i < 7; i++)
            {
                for (int j = 5; j >= 0; j--)
                {
                    if (InitialBoard[i, j] == ' ')
                    {
                        AvalibleSpot[i] = Convert.ToChar(j);
                        break;
                    }
                }
            }

            char[,] testBoard = new char[7, 6];
            Array.Copy(InitialBoard, testBoard, 42);

            //檢查:電腦可獲勝
            for (int i = 0; i < 7; i++)
            {
                if (AvalibleSpot[i] == 'N')
                    continue;

                testBoard[i, AvalibleSpot[i]] = 'X';
                if (winner(testBoard) == 0)
                    return i;
                Array.Copy(InitialBoard, testBoard, 42);
            }

            //檢查:電腦無法直接獲勝，可直接阻止玩家獲勝
            for (int i = 0; i < 7; i++)
            {
                if (AvalibleSpot[i] == 'N')
                    continue;

                testBoard[i, AvalibleSpot[i]] = '●';
                if (winner(testBoard) == 1)
                    return i;
                Array.Copy(InitialBoard, testBoard, 42);
            }

            //檢查:電腦避免幫助玩家獲勝or該排以滿不可再下(AvoidSpot[])
            int[] AvoidSpot = { 9, 9, 9, 9, 9, 9, 9 };
            for (int i = 0; i < 7; i++)
            {
                if (AvalibleSpot[i] == 'N')
                {
                    AvoidSpot[i] = i;
                    continue;
                }

                if (AvalibleSpot[i] == '\0')
                    continue;

                testBoard[i, AvalibleSpot[i]] = 'X';
                testBoard[i, (AvalibleSpot[i] - 1)] = '●';
                if (winner(testBoard) == 1)
                    AvoidSpot[i] = i;

                Array.Copy(InitialBoard, testBoard, 42);
            }


            //避開AvoidSpot的前提下，電腦隨機落子
            Random cpuPlay = new Random();
            int returnValue;
            bool NoSpot = true;
            for (int i = 0; i < 7; i++)
            {
                if (AvoidSpot[i] == 9)
                    NoSpot = false;
            }

            //例外處理:7排皆為AvoidSpot
            if (NoSpot)
            {
                do
                    returnValue = (cpuPlay.Next() % 7);
                while (AvalibleSpot[returnValue] == 'N');
                return returnValue;
            }

            do
                returnValue = (cpuPlay.Next() % 7);
            while (AvoidSpot[returnValue] == returnValue);

            return returnValue;
        }
    }
}