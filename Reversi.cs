using System;
using System.Threading;

namespace GameBundle
{
    class Reversi
    {
        public void Launch(ref int WinOrLose)
        {
            //規則說明
            Console.WriteLine("歡迎遊玩黑白棋(Reversi)");
            Console.WriteLine("以下為規則說明:");
            Console.WriteLine("雙方各執黑、白子，輪流在棋盤上下棋");
            Console.WriteLine("只要落子和棋盤上任一枚己方的棋子在一條線上（橫、直、斜線皆可）夾著對方棋子，就能將對方的這些棋子轉變為我己方");
            Console.WriteLine("如果在任一位置落子都不能夾住對手的任一顆棋子，就要讓對手下子");
            Console.WriteLine("當雙方皆不能下子時，遊戲就結束，子多的一方勝");
            Console.WriteLine("請按任意鍵開始遊戲");
            Console.ReadKey();

            string[] DropPosition = new string[2];
            string read;
            WinOrLose = 0;
            int freedomA = 4;
            int freedomB = 4;
            int black;
            int white;
            char[,] board = {{' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '}};
            board[3, 3] = '●';
            board[4, 4] = '●';
            board[3, 4] = '○';
            board[4, 3] = '○';
            char[,] AvailibleBoard = new char[8, 8];

            printInitialBoard(board, '●');


            while (true)
            {
                //玩家1回合:
                //落子                
                if (freedomA != 0)
                {
                    Console.Write("玩家1的回合\n請選擇落子處，並以逗號分隔，如:1,A\n");
                    read = Console.ReadLine();
                    DropPosition = read.Split(",");
                    while (true)
                    {
                        if ((DropPosition[0] == "1" || DropPosition[0] == "2" || DropPosition[0] == "3" || DropPosition[0] == "4" || DropPosition[0] == "5" || DropPosition[0] == "6" || DropPosition[0] == "7" || DropPosition[0] == "8") &&
                            (DropPosition[1] == "A" || DropPosition[1] == "B" || DropPosition[1] == "C" || DropPosition[1] == "D" || DropPosition[1] == "E" || DropPosition[1] == "F" || DropPosition[1] == "G" || DropPosition[1] == "H"))
                        {
                            translate(ref DropPosition);
                            AvailibleSpot(board, out AvailibleBoard, '●');
                            if (AvailibleBoard[Convert.ToInt16(DropPosition[0]), Convert.ToInt16(DropPosition[1])] == 'a')
                                break;
                        }

                        Console.Clear();
                        printInitialBoard(board, '●');
                        Console.Write("輸入有誤，請重新選擇落子處，並以逗號分隔，如:1,A\n");
                        read = Console.ReadLine();
                        DropPosition = read.Split(',');
                    }
                    //印出board
                    PrintBoard('●', DropPosition, ref board, freedomA, freedomB);
                    CountFreedom(ref freedomA, ref freedomB, board);
                    if (freedomA == 0 && freedomB == 0)
                        break;
                }

                //玩家2回合:
                //落子                
                if (freedomB != 0)
                {
                    Console.Write("玩家2的回合\n請選擇落子處，並以逗號分隔，如:1,A\n");
                    read = Console.ReadLine();
                    DropPosition = read.Split(',');
                    while (true)
                    {
                        if ((DropPosition[0] == "1" || DropPosition[0] == "2" || DropPosition[0] == "3" || DropPosition[0] == "4" || DropPosition[0] == "5" || DropPosition[0] == "6" || DropPosition[0] == "7" || DropPosition[0] == "8") &&
                            (DropPosition[1] == "A" || DropPosition[1] == "B" || DropPosition[1] == "C" || DropPosition[1] == "D" || DropPosition[1] == "E" || DropPosition[1] == "F" || DropPosition[1] == "G" || DropPosition[1] == "H"))
                        {
                            translate(ref DropPosition);
                            AvailibleSpot(board, out AvailibleBoard, '○');
                            if (AvailibleBoard[Convert.ToInt16(DropPosition[0]), Convert.ToInt16(DropPosition[1])] == 'a')
                                break;
                        }

                        Console.Clear();
                        printInitialBoard(board, '○');
                        Console.Write("輸入有誤，請重新選擇落子處，並以逗號分隔，如:1,A\n");
                        read = Console.ReadLine();
                        DropPosition = read.Split(',');
                    }
                    //印出board
                    PrintBoard('○', DropPosition, ref board, freedomA, freedomB);
                    CountFreedom(ref freedomA, ref freedomB, board);
                    if (freedomA == 0 && freedomB == 0)
                        break;
                }
            }

            count(board, out black, out white);

            if (black > white)
            {
                Console.WriteLine("\n遊戲結束，玩家1({0}:{1})獲勝!", black, white);
                Console.ReadKey();
                WinOrLose = 1;
            }


            if (black < white)
            {
                Console.WriteLine("\n遊戲結束，玩家2({0}:{1})獲勝!", white, black);
                Console.ReadKey();
                WinOrLose = -1;
            }

        }
        static void translate(ref string[] DropPosition)
        {
            int trans = Convert.ToInt16(DropPosition[0]);
            trans -= 1;
            DropPosition[0] = Convert.ToString(trans);

            switch (DropPosition[1])
            {
                case "A":
                    DropPosition[1] = "0";
                    break;

                case "B":
                    DropPosition[1] = "1";
                    break;

                case "C":
                    DropPosition[1] = "2";
                    break;

                case "D":
                    DropPosition[1] = "3";
                    break;

                case "E":
                    DropPosition[1] = "4";
                    break;

                case "F":
                    DropPosition[1] = "5";
                    break;

                case "G":
                    DropPosition[1] = "6";
                    break;

                case "H":
                    DropPosition[1] = "7";
                    break;
            }
        }
        static void PrintBoard(char symbol, string[] DropPosition, ref char[,] board, int freedomA, int freedomB)
        {
            char[,] FlipBoard = {{' ',' ',' ',' ',' ',' ',' ',' '},
                                {' ',' ',' ',' ',' ',' ',' ',' '},
                                {' ',' ',' ',' ',' ',' ',' ',' '},
                                {' ',' ',' ',' ',' ',' ',' ',' '},
                                {' ',' ',' ',' ',' ',' ',' ',' '},
                                {' ',' ',' ',' ',' ',' ',' ',' '},
                                {' ',' ',' ',' ',' ',' ',' ',' '},
                                {' ',' ',' ',' ',' ',' ',' ',' '}};
            char[,] AvailibleBoard ={{' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '}};

            Console.Clear();
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("   1   2   3   4   5   6   7   8\n");

            flip(ref board, DropPosition, ref FlipBoard, symbol);

            CountFreedom(ref freedomA, ref freedomB, board);
            char tempSymbol;
            if (symbol == '●' && freedomB != 0)
                tempSymbol = '○';
            else if (symbol == '○' && freedomA != 0)
                tempSymbol = '●';
            else
                tempSymbol = symbol;
            AvailibleSpot(board, out AvailibleBoard, tempSymbol);

            for (int j = 0; j < 8; j++)
            {
                switch (j)
                {
                    case 0:
                        Console.Write("A|");
                        break;

                    case 1:
                        Console.Write("B|");
                        break;

                    case 2:
                        Console.Write("C|");
                        break;

                    case 3:
                        Console.Write("D|");
                        break;

                    case 4:
                        Console.Write("E|");
                        break;

                    case 5:
                        Console.Write("F|");
                        break;

                    case 6:
                        Console.Write("G|");
                        break;

                    case 7:
                        Console.Write("H|");
                        break;
                }
                for (int i = 0; i < 8; i++)
                {
                    Console.SetCursorPosition((4 * (i + 1) - 1), (7 + 2 * (j + 1)));
                    if (i == Convert.ToInt16(DropPosition[0]) && j == Convert.ToInt16(DropPosition[1]))
                        Console.ForegroundColor = ConsoleColor.Red;
                    if (FlipBoard[i, j] == 'f')
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    if (AvailibleBoard[i, j] == 'a')
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write("{0}", board[i, j]);
                    Console.ResetColor();
                    Console.Write(" |");
                }
                Console.WriteLine("\n  --------------------------------");
            }
            Console.SetCursorPosition(0, 0);
        }

        static void printInitialBoard(char[,] board, char symbol)
        {
            char[,] AvailibleBoard = new char[8, 8];

            Console.Clear();
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("   1   2   3   4   5   6   7   8\n");
            AvailibleSpot(board, out AvailibleBoard, symbol);
            for (int j = 0; j < 8; j++)
            {
                switch (j)
                {
                    case 0:
                        Console.Write("A|");
                        break;

                    case 1:
                        Console.Write("B|");
                        break;

                    case 2:
                        Console.Write("C|");
                        break;

                    case 3:
                        Console.Write("D|");
                        break;

                    case 4:
                        Console.Write("E|");
                        break;

                    case 5:
                        Console.Write("F|");
                        break;

                    case 6:
                        Console.Write("G|");
                        break;

                    case 7:
                        Console.Write("H|");
                        break;
                }
                for (int i = 0; i < 8; i++)
                {
                    Console.SetCursorPosition((4 * (i + 1) - 1), (7 + 2 * (j + 1)));
                    if (AvailibleBoard[i, j] == 'a')
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write("{0}", board[i, j]);
                    Console.ResetColor();
                    Console.Write(" |");
                }
                Console.WriteLine("\n  --------------------------------");
            }
            Console.SetCursorPosition(0, 0);
        }
        static void AvailibleSpot(char[,] board, out char[,] AvailibleBoard, char symbol)
        {
            char[,] sample = {{' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '},
                            {' ',' ',' ',' ',' ',' ',' ',' '}};
            char[,] unsused = {{' ',' ',' ',' ',' ',' ',' ',' '},
                              {' ',' ',' ',' ',' ',' ',' ',' '},
                              {' ',' ',' ',' ',' ',' ',' ',' '},
                              {' ',' ',' ',' ',' ',' ',' ',' '},
                              {' ',' ',' ',' ',' ',' ',' ',' '},
                              {' ',' ',' ',' ',' ',' ',' ',' '},
                              {' ',' ',' ',' ',' ',' ',' ',' '},
                              {' ',' ',' ',' ',' ',' ',' ',' '}};
            AvailibleBoard = sample;
            string[] DropPosition = new string[2];
            char[,] TestBoard = new char[8, 8];
            Array.Copy(board, TestBoard, 64);

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (TestBoard[i, j] == ' ')
                    {
                        DropPosition[0] = Convert.ToString(i);
                        DropPosition[1] = Convert.ToString(j);
                        if (flip(ref TestBoard, DropPosition, ref unsused, symbol))
                            AvailibleBoard[i, j] = 'a';
                    }
                    Array.Copy(board, TestBoard, 64);
                }
        }
        static void CountFreedom(ref int freedomA, ref int freedomB, char[,] board)
        {
            char[,] AvailibleBoard ={{' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '},
                                    {' ',' ',' ',' ',' ',' ',' ',' '}};
            freedomA = 0;
            freedomB = 0;

            AvailibleSpot(board, out AvailibleBoard, '●');
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (AvailibleBoard[i, j] == 'a')
                        freedomA++;
                }

            AvailibleSpot(board, out AvailibleBoard, '○');
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (AvailibleBoard[i, j] == 'a')
                        freedomB++;
                }
        }
        static void count(char[,] board, out int black, out int white)
        {
            black = 0;
            white = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == '●')
                        black++;
                    if (board[i, j] == '○')
                        white++;
                }
        }
        static bool flip(ref char[,] board, string[] DropPosition, ref char[,] FlipBoard, char symbol)
        {
            bool canFlip = false;
            //for (int x = 0; x< 8; x++)
            //    for (int y = 0; y< 8; y++)
            //        FlipBoard[x, y] = ' ';
            board[Convert.ToInt16(DropPosition[0]), Convert.ToInt16(DropPosition[1])] = symbol;

            //橫向
            int end;

            int i = Convert.ToInt16(DropPosition[0]) + 1;
            if (i < 7)
                if (board[i, Convert.ToInt16(DropPosition[1])] != symbol && board[i, Convert.ToInt16(DropPosition[1])] != ' ')
                {
                    end = i;
                    while (board[end, Convert.ToInt16(DropPosition[1])] != symbol)
                    {
                        if (board[end, Convert.ToInt16(DropPosition[1])] == ' ')
                            end = 8;
                        end++;
                        if (end > 7)
                            break;
                    }
                    if (end <= 7)
                    {
                        for (int u = i; u < end; u++)
                        {
                            board[u, Convert.ToInt16(DropPosition[1])] = symbol;
                            FlipBoard[u, Convert.ToInt16(DropPosition[1])] = 'f';
                        }
                        canFlip = true;
                    }
                }

            i = Convert.ToInt16(DropPosition[0]) - 1;
            if (i > 0)
                if (board[i, Convert.ToInt16(DropPosition[1])] != symbol && board[i, Convert.ToInt16(DropPosition[1])] != ' ')
                {
                    end = i;
                    while (board[end, Convert.ToInt16(DropPosition[1])] != symbol)
                    {
                        if (board[end, Convert.ToInt16(DropPosition[1])] == ' ')
                            end = 0;
                        end--;
                        if (end < 0)
                            break;
                    }
                    if (end >= 0)
                    {
                        for (int u = i; u > end; u--)
                        {
                            board[u, Convert.ToInt16(DropPosition[1])] = symbol;
                            FlipBoard[u, Convert.ToInt16(DropPosition[1])] = 'f';
                        }
                        canFlip = true;
                    }
                }

            //縱向
            int j = Convert.ToInt16(DropPosition[1]) + 1;
            if (j < 7)
                if (board[Convert.ToInt16(DropPosition[0]), j] != symbol && board[Convert.ToInt16(DropPosition[0]), j] != ' ')
                {
                    end = j;
                    while (board[Convert.ToInt16(DropPosition[0]), end] != symbol)
                    {
                        if (board[Convert.ToInt16(DropPosition[0]), end] == ' ')
                            end = 8;
                        end++;
                        if (end > 7)
                            break;
                    }
                    if (end <= 7)
                    {
                        for (int u = j; u < end; u++)
                        {
                            board[Convert.ToInt16(DropPosition[0]), u] = symbol;
                            FlipBoard[Convert.ToInt16(DropPosition[0]), u] = 'f';
                        }
                        canFlip = true;
                    }
                }

            j = Convert.ToInt16(DropPosition[1]) - 1;
            if (j > 0)
                if (board[Convert.ToInt16(DropPosition[0]), j] != symbol && board[Convert.ToInt16(DropPosition[0]), j] != ' ')
                {
                    end = j;
                    while (board[Convert.ToInt16(DropPosition[0]), end] != symbol)
                    {
                        if (board[Convert.ToInt16(DropPosition[0]), end] == ' ')
                            end = 0;
                        end--;
                        if (end < 0)
                            break;
                    }
                    if (end >= 0)
                    {
                        for (int u = j; u > end; u--)
                        {
                            board[Convert.ToInt16(DropPosition[0]), u] = symbol;
                            FlipBoard[Convert.ToInt16(DropPosition[0]), u] = 'f';
                        }
                        canFlip = true;
                    }
                }

            //斜向
            int k1 = Convert.ToInt16(DropPosition[0]) + 1;
            int k2 = Convert.ToInt16(DropPosition[1]) + 1;
            int Xend = k1;
            int Yend = k2;
            if (k1 < 7 && k2 < 7)
                if (board[k1, k2] != symbol && board[k1, k2] != ' ')
                {
                    while (board[Xend, Yend] != symbol)
                    {
                        if (board[Xend, Yend] == ' ')
                            Xend = 7;
                        Xend++;
                        Yend++;
                        if (Xend == 8 || Yend == 8)
                            break;
                    }
                    if (Xend != 8 && Yend != 8)
                    {
                        int v = k2;
                        for (int u = k1; u < Xend; u++)
                        {
                            board[u, v] = symbol;
                            FlipBoard[u, v] = 'f';
                            v++;
                        }
                        canFlip = true;
                    }
                }

            k1 = Convert.ToInt16(DropPosition[0]) + 1;
            k2 = Convert.ToInt16(DropPosition[1]) - 1;
            Xend = k1;
            Yend = k2;
            if (k1 < 7 && k2 > 0)
                if (board[k1, k2] != symbol && board[k1, k2] != ' ')
                {
                    while (board[Xend, Yend] != symbol)
                    {
                        if (board[Xend, Yend] == ' ')
                            Xend = 7;
                        Xend++;
                        Yend--;
                        if (Xend == 8 || Yend == -1)
                            break;
                    }
                    if (Xend != 8 && Yend != -1)
                    {
                        int v = k2;
                        for (int u = k1; u < Xend; u++)
                        {
                            board[u, v] = symbol;
                            FlipBoard[u, v] = 'f';
                            v--;
                        }
                        canFlip = true;
                    }
                }


            k1 = Convert.ToInt16(DropPosition[0]) - 1;
            k2 = Convert.ToInt16(DropPosition[1]) - 1;
            Xend = k1;
            Yend = k2;
            if (k1 > 0 && k2 > 0)
                if (board[k1, k2] != symbol && board[k1, k2] != ' ')
                {
                    while (board[Xend, Yend] != symbol)
                    {
                        if (board[Xend, Yend] == ' ')
                            Xend = 0;
                        Xend--;
                        Yend--;
                        if (Xend == -1 || Yend == -1)
                            break;
                    }
                    if (Xend != -1 && Yend != -1)
                    {
                        int v = k2;
                        for (int u = k1; u > Xend; u--)
                        {
                            board[u, v] = symbol;
                            FlipBoard[u, v] = 'f';
                            v--;
                        }
                        canFlip = true;
                    }
                }

            k1 = Convert.ToInt16(DropPosition[0]) - 1;
            k2 = Convert.ToInt16(DropPosition[1]) + 1;
            Xend = k1;
            Yend = k2;
            if (k1 > 0 && k2 < 7)
                if (board[k1, k2] != symbol && board[k1, k2] != ' ')
                {
                    while (board[Xend, Yend] != symbol)
                    {
                        if (board[Xend, Yend] == ' ')
                            Xend = 0;
                        Xend--;
                        Yend++;
                        if (Xend == -1 || Yend == 8)
                            break;
                    }
                    if (Xend != -1 && Yend != 8)
                    {
                        int v = k2;
                        for (int u = k1; u > Xend; u--)
                        {
                            board[u, v] = symbol;
                            FlipBoard[u, v] = 'f';
                            v++;
                        }
                        canFlip = true;
                    }
                }
            return canFlip;
        }
    }
}
