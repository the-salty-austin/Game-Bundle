using System;
using System.IO;
using System.Threading;
using System.Data.SQLite;

namespace GameBundle
{
    class Database
    {
        public void DB_SetUp()
        {
            CreateDB();
            string cs = @"URI=file:C:\C#\SummerProject\SP_Project3_v1\game_bundle.sqlite3";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            CreateTable(cmd);
        }
        static void CreateDB()
        {
            SQLiteConnection connection;
            connection = new SQLiteConnection("Data Source=game_bundle.sqlite3");
            if (!File.Exists("./game_bundle.sqlite3"))
            {
                SQLiteConnection.CreateFile("game_bundle.sqlite3");
                System.Console.WriteLine("Database file created");
            }
        }
        static void CreateTable(SQLiteCommand cmd)
        {

            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS User(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, username TEXT UNIQUE, password TEXT)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS HighScore(user_id INTEGER NOT NULL UNIQUE, rhm INTEGER, cn4 INTEGER, rvs INTEGER, snp INTEGER)";
            cmd.ExecuteNonQuery();
        }
        public void CreateAccount()
        {
            string cs = @"URI=file:C:\C#\SummerProject\SP_Project3_v1\game_bundle.sqlite3";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(con);
            
            string name;
            string pwd;
            string tmp_pwd;
            do {
                Console.Write("Enter New Username: ");
                name = Console.ReadLine();
                Console.Write("Enter Your Password: ");
                pwd = Console.ReadLine();
                tmp_pwd = pwd;
                Console.Clear();
                Console.Write("Verify Your Password: ");
                pwd = Console.ReadLine();
                if (pwd=="") Console.WriteLine("Password must contain valid characters.");
                else if (pwd!=tmp_pwd) Console.WriteLine("Password Verification Failed.");
                Thread.Sleep(1000);

                Console.Clear();
            } while (pwd!=tmp_pwd|pwd=="");

            cmd.CommandText = "INSERT OR IGNORE INTO User (username,password) VALUES (@name,@password)";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", pwd);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            
            // initialize highscore==0
            string stm;
            int id=0;
            stm = "SELECT id FROM User WHERE (username==@name)";
            using var cmd_retrieve = new SQLiteCommand(stm, con);
            cmd_retrieve.Parameters.AddWithValue("@name", name);
            cmd_retrieve.Prepare();
            cmd_retrieve.ExecuteNonQuery();
            using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();
            while (rdr.Read())
            {
                id = rdr.GetInt32(0);
            }

            cmd.CommandText = "INSERT OR IGNORE INTO HighScore (user_id,rhm,cn4,rvs,snp) VALUES (@id,0,0,0,0)";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            Console.WriteLine("User profile successfully established.");
            Thread.Sleep(750);
            Console.Clear();
        }
        public bool Authentication(ref int id)
        {
            string cs = @"URI=file:C:\C#\SummerProject\SP_Project3_v1\game_bundle.sqlite3";
            using var con = new SQLiteConnection(cs);
            con.Open();
            
            bool auth=false;
            Console.Write("Enter Username: ");
            string name = Console.ReadLine();
            Console.Write("Enter Password: ");
            string pwd = Console.ReadLine();
            
            string stm = "SELECT User.id,User.username,User.password FROM User";
            using var cmd_retrieve = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();
            while(rdr.Read())
            {
                string auth_name = rdr.GetString(1);
                string auth_pwd = rdr.GetString(2);

                if (name==auth_name&pwd==auth_pwd)
                {
                    Console.WriteLine("\nAuthenticated.");
                    auth = true;
                    id = rdr.GetInt32(0);
                    Thread.Sleep(1000);
                    Console.Clear();
                    return auth;
                }
            }

            Console.WriteLine("\n[ Warning ] Wrong Username or Password.");
            Thread.Sleep(1000);
            Console.Clear();
            return auth;
        }
        public void SaveHighScore(int score,int game,int id)
        {
            int compare = 0;
            int old_score = 0;
            
            string cs = @"URI=file:C:\C#\SummerProject\SP_Project3_v1\game_bundle.sqlite3";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            switch (game)
            {
                case 1: // Rush Hour Maniac
                    {
                        string stm = "SELECT user_id,rhm FROM HighScore WHERE (user_id=@id)";
                        using var cmd_retrieve = new SQLiteCommand(stm, con);
                        cmd_retrieve.Parameters.AddWithValue("@id", id);
                        cmd_retrieve.Prepare();
                        cmd_retrieve.ExecuteNonQuery();
                        using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();
                        while (rdr.Read())
                        {
                            compare = rdr.GetInt32(1);
                        }
                        if (compare<score) 
                        {
                            cmd.CommandText = "UPDATE HighScore SET rhm=@score WHERE user_id=@id";
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@score", score);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        break;
                    }
                case 2: // Connect Four
                    {   
                        string stm = "SELECT user_id,cn4 FROM HighScore WHERE (user_id=@id)";
                        using var cmd_retrieve = new SQLiteCommand(stm, con);
                        cmd_retrieve.Parameters.AddWithValue("@id", id);
                        cmd_retrieve.Prepare();
                        cmd_retrieve.ExecuteNonQuery();
                        using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();
                        while (rdr.Read())
                        {
                            old_score = rdr.GetInt32(1);
                        }
                        score = old_score+score;
                        cmd.CommandText = "UPDATE HighScore SET cn4=@score WHERE user_id==@id";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@score", score);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        break;
                    }
                case 3: // Reversi
                    {
                        string stm = "SELECT user_id,rvs FROM HighScore WHERE (user_id=@id)";
                        using var cmd_retrieve = new SQLiteCommand(stm, con);
                        cmd_retrieve.Parameters.AddWithValue("@id", id);
                        cmd_retrieve.Prepare();
                        cmd_retrieve.ExecuteNonQuery();
                        using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();
                        while (rdr.Read())
                        {
                            old_score = rdr.GetInt32(1);
                        }
                        score = old_score+score;
                        cmd.CommandText = "UPDATE HighScore SET rvs=@score WHERE user_id==@id";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@score", score);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        break;
                    }
                case 4: // Sniper
                    {
                        string stm = "SELECT user_id,snp FROM HighScore WHERE (user_id=@id)";
                        using var cmd_retrieve = new SQLiteCommand(stm, con);
                        cmd_retrieve.Parameters.AddWithValue("@id", id);
                        cmd_retrieve.Prepare();
                        cmd_retrieve.ExecuteNonQuery();
                        using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();
                        while (rdr.Read())
                        {
                            compare = rdr.GetInt32(1);
                        }
                        if (compare<score)
                        {
                            cmd.CommandText = "UPDATE HighScore SET snp=@score WHERE user_id==@id";
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@score", score);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        break;
                    }
            }

        }
        public void GetPersonalScore(int id)
        {
            string cs = @"URI=file:C:\C#\SummerProject\SP_Project3_v1\game_bundle.sqlite3";
            using var con = new SQLiteConnection(cs);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            string stm = "SELECT user_id,rhm,cn4,rvs,snp FROM HighScore WHERE (user_id=@id)";
            using var cmd_retrieve = new SQLiteCommand(stm, con);
            cmd_retrieve.Parameters.AddWithValue("@id", id);
            cmd_retrieve.Prepare();
            cmd_retrieve.ExecuteNonQuery();
            using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();

            int rhm_score = 0;
            int cn4_score = 0;
            int rvs_score = 0;
            int snp_score = 0;

            Console.WriteLine("\nYour Personal Scores: ");
            Console.WriteLine($@"{"RushHourManiac", -20} {"ConnectFour", -20} {"Reversi", -20} {"Sniper", -20}");
            while(rdr.Read())
            {
                rhm_score = rdr.GetInt32(1);
                cn4_score = rdr.GetInt32(2);
                rvs_score = rdr.GetInt32(3);
                snp_score = rdr.GetInt32(4);
                Console.WriteLine($@"{rhm_score+" (Highest)", -20} {cn4_score, -20} {rvs_score, -20} {snp_score+" (Highest)", -20}");
                Console.WriteLine("\nPress [Enter] to continue...");
                Console.ReadLine();
            }
        }
        public void GetLeaderboard(int mode)
        {
            string cs = @"URI=file:C:\C#\SummerProject\SP_Project3_v1\game_bundle.sqlite3";
            using var con = new SQLiteConnection(cs);
            con.Open();

            string stm = "";
            string gametype = "";
            
            
            switch (mode)
            {
                case 41:
                {
                    stm = "SELECT User.id,User.username,HighScore.user_id,HighScore.rhm FROM User,HighScore WHERE User.id = HighScore.user_id ORDER BY HighScore.rhm DESC";
                    gametype = "Rush Hour Maniac";
                    break;
                }
                case 42:
                {
                    stm = "SELECT User.id,User.username,HighScore.user_id,HighScore.cn4 FROM User,HighScore WHERE User.id = HighScore.user_id ORDER BY HighScore.cn4 DESC";
                    gametype = "Connect Four";
                    break;
                }
                case 43:
                {
                    stm = "SELECT User.id,User.username,HighScore.user_id,HighScore.rvs FROM User,HighScore WHERE User.id = HighScore.user_id ORDER BY HighScore.rvs DESC";
                    gametype = "Reversi";
                    break;
                }
                case 44:
                {
                    stm = "SELECT User.id,User.username,HighScore.user_id,HighScore.snp FROM User,HighScore WHERE User.id = HighScore.user_id ORDER BY HighScore.snp DESC";
                    gametype = "Sniper";
                    break;
                }
            }
            using var cmd_retrieve = new SQLiteCommand(stm, con);
            //cmd_retrieve.Prepare();
            //cmd_retrieve.ExecuteNonQuery();
            using SQLiteDataReader rdr = cmd_retrieve.ExecuteReader();

            Console.WriteLine("\n********* Leaderboard *********\n");
            Console.WriteLine($@"{"Username", -20} {gametype, -20}");
            Console.WriteLine();
            while(rdr.Read())
            {
                Console.WriteLine($@"{rdr.GetString(1), -20} {rdr.GetInt32(3), -20}");
            }
            Console.WriteLine("\nPress [Enter] to continue...");
            Console.ReadLine();
        }
    }
}