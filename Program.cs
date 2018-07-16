using System;
using System.Collections.Generic;
using System.IO;

namespace TurtleChallenge
{
    class Turtle
    {
        int position_x;
        int position_y;
        string moveDirection;

        public Turtle(int pos_x, int pos_y, string movDir)
        {
            position_x = pos_x;
            position_y = pos_y;
            moveDirection = movDir;
        }

        public (int, int) GetPosition()
        {
            return (position_x, position_y);
        }

        public string GetMoveDirection()
        {
            return moveDirection;
        }

        public void ChangeDirection(string newDirection)
        {
            moveDirection = newDirection;
        }

        public void ChangePosition((int, int) nextPos)
        {
            position_x = nextPos.Item1;
            position_y = nextPos.Item2;
        }

        public bool DoesCanMove((int, int) nextPos, (int, int) boardSize)
        {
            if (moveDirection == "North" || moveDirection == "South")
            {
                if (Math.Abs(position_y - nextPos.Item2) == 1 && position_x == nextPos.Item1 &&
                             nextPos.Item2 < boardSize.Item2 && nextPos.Item2 > 0)
                {
                    return true;
                }

                return false;
            }
            else if (moveDirection == "East" || moveDirection == "West")
            {
                if (Math.Abs(position_x - nextPos.Item1) == 1 && position_y == nextPos.Item2 &&
                             nextPos.Item1 < boardSize.Item1 && nextPos.Item1 > 0)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }

    class Mine
    {
        int position_x;
        int position_y;

        public Mine(int pos_x, int pos_y)
        {
            position_x = pos_x;
            position_y = pos_y;
        }

        public (int, int) GetPosition()
        {
            return (position_x, position_y);
        }
    }

    class GameSettings
    {
        int board_size_x;
        int board_size_y;
        int game_end_pos_x;
        int game_end_pos_y;

        public GameSettings(int boardSizeX, int boardSizeY, int gameEndPosX, int gameEndPosY)
        {
            board_size_x = boardSizeX;
            board_size_y = boardSizeY;
            game_end_pos_x = gameEndPosX;
            game_end_pos_y = gameEndPosY;
        }

        public (int, int) GetBoardSize()
        {
            return (board_size_x, board_size_y);
        }

        public (int, int) GetGameEndPosition()
        {
            return (game_end_pos_x, game_end_pos_y);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            (GameSettings gameSetttings, List<Mine> mines, Turtle turtle) = ReadGameSettings("game-settings");

            MainFunction("moves", gameSetttings, turtle, mines);
            Console.Read();
        }

        static (GameSettings, List<Mine>, Turtle) ReadGameSettings(string fileName)
        {
            var fileLines = File.ReadAllLines(fileName + ".txt");

            var firstLineSplited = fileLines[0].Split(" ");

            int boardSizeX = Convert.ToInt32(firstLineSplited[0]);
            int boardSizeY = Convert.ToInt32(firstLineSplited[1]);

            var thirdLineSplited = fileLines[2].Split(" ");

            int gameEndPosX = Convert.ToInt32(thirdLineSplited[0]);
            int gameEndPosY = Convert.ToInt32(thirdLineSplited[1]);

            GameSettings gameSettings = new GameSettings(boardSizeX, boardSizeY, gameEndPosX, gameEndPosY);

            var secondLineSplited = fileLines[1].Split(" ");

            int turtleStartPosX = Convert.ToInt32(secondLineSplited[0]);
            int turtleStartPosY = Convert.ToInt32(secondLineSplited[1]);
            string turtleStartDirection = secondLineSplited[2];

            Turtle turtle = new Turtle(turtleStartPosX, turtleStartPosY, turtleStartDirection);

            List<Mine> mines = new List<Mine>();

            for (int i = 3; i < fileLines.Length; i++)
            {
                var lineSplited = fileLines[i].Split(" ");

                int minePosX = Convert.ToInt32(lineSplited[0]);
                int minePosY = Convert.ToInt32(lineSplited[1]);

                Mine mine = new Mine(minePosX, minePosY);

                mines.Add(mine);
            }

            return (gameSettings, mines, turtle);
        }

        static void MainFunction(string fileName, GameSettings gameSettings, Turtle turtle, List<Mine> mines)
        {
            StreamReader file = new StreamReader(fileName + ".txt");
            string line;
            int count = 1;

            while ((line = file.ReadLine()) != null)
            {
                var values = line.Split(" ");

                if (values.Length == 1)
                {
                    turtle.ChangeDirection(values[0]);
                    Console.WriteLine("Sequence {0}: Success", count++);
                }
                else if (values.Length == 2)
                {
                    int nextPosX = Convert.ToInt32(values[0]);
                    int nextPosY = Convert.ToInt32(values[1]);

                    (int, int) nextPos = (nextPosX, nextPosY);

                    if (turtle.DoesCanMove(nextPos, gameSettings.GetBoardSize()))
                    {
                        if (DoesHitMine(nextPos, mines))
                        {
                            Console.WriteLine("Sequence {0}: Mine hit!", count++);
                        }
                        else
                        {
                            Console.WriteLine("Sequence {0}: Success", count++);
                        }

                        turtle.ChangePosition(nextPos);
                    }
                    else
                    {
                        Console.WriteLine("Sequence {0}: Turtle can not move", count++);
                    }
                }
            }


            if (turtle.GetPosition().Item1 != gameSettings.GetGameEndPosition().Item1 &&
                turtle.GetPosition().Item2 != gameSettings.GetGameEndPosition().Item2)
            {

                Console.WriteLine("Turtle not finished successfully");
            }
            else
            {

                Console.WriteLine("Turtle finished successfully");
            }
        }

        static bool DoesHitMine((int, int) nextPos, List<Mine> mines)
        {
            // go through all mines and check do any mine position is equal to next turtle movement position
            foreach (var mine in mines)
            {
                if (mine.GetPosition().Item1 == nextPos.Item1 && mine.GetPosition().Item2 == nextPos.Item2)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
