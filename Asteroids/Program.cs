using System;
using System.Collections.Generic;
using System.Threading;

class Asteroids
{

    public class Position
    {
        protected int row;
        protected int col;

        public Position() { }

        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public int Row
        {
            get
            {
                return this.row;
            }
            set
            {
                this.row = value;
            }
        }

        public int Col
        {
            get
            {
                return this.col;
            }
            set
            {
                this.col = value;
            }
        }
    }

    public class Rock : Position
    {
        private char charSign;
        private ConsoleColor color;

        public Rock(int row, int col, char charSign, ConsoleColor color)
            : base(row, col)
        {
            this.charSign = charSign;
            this.color = color;
        }

        public char CharSign
        {
            get
            {
                return this.charSign;
            }
            set
            {
                this.charSign = value;
            }
        }

        public ConsoleColor Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }
    }

    public class Projectile : Rock
    {
        public Projectile(int row, int col, char charSign, ConsoleColor color)
            : base(row, col, charSign, color) { }
    }

    static void Main(string[] args)
    {
        int score = 0;
        char[] symbols = new char[] { '^', '@', '*', '&', '+', '%', '$', '#', '!', '.', ';' };
        ConsoleColor[] colors = new ConsoleColor[]
        {
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Magenta,
            ConsoleColor.Red,
            ConsoleColor.White,
        };
        int sleepTime = 70;
        Console.Title = "Asteroids";
        Console.BufferHeight = Console.WindowHeight;
        Console.CursorVisible = false;
        Position ship = new Position(Console.WindowHeight - 1, (Console.WindowWidth / 2) - 1);
        List<Rock> rocks = new List<Rock>();
        List<Projectile> projectiles = new List<Projectile>();
        Random rand = new Random();
        bool isDead = false;

        while (true)
        {
            Console.Clear();
            Console.SetCursorPosition(ship.Col, ship.Row);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("(^)");

            Console.SetCursorPosition(Console.WindowWidth - 15, 0);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("SCORE: {0}", score);

            Rock newRock = new Rock(0, rand.Next(Console.WindowWidth-1),
                symbols[rand.Next(symbols.Length)], colors[rand.Next(colors.Length)]);
            rocks.Add(newRock);

            IntersectRocksAndProjectiles(ref rocks, ref projectiles, ref score);

            PrintRocks(ref rocks, ref ship, ref isDead, ref score);
            if (isDead)
            { 
                isDead = false;
                projectiles.Clear();
                rocks.Clear();
                ship.Col = (Console.WindowWidth / 2) - 1;
                score = 0;
                Console.ReadKey(); 
                continue; 
            }
            AdvanceRocks(ref rocks, ref score);
            PrintProjectiles(ref projectiles);
            AdvanceProjectiles(ref projectiles, ref rocks);

            Thread.Sleep(sleepTime);

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.LeftArrow && ship.Col > 0)
                {
                    ship.Col -= 1;
                }
                else if (key.Key == ConsoleKey.RightArrow && ship.Col < Console.WindowWidth - 5)
                {
                    ship.Col += 1;
                }
                else if (key.Key == ConsoleKey.Add)
                {
                    if (sleepTime > 20)
                    {
                        sleepTime -= 20;
                    }
                }
                else if (key.Key == ConsoleKey.Subtract)
                {
                    if (sleepTime < 200)
                    {
                        sleepTime += 20;
                    }
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    projectiles.Add(new Projectile(Console.WindowHeight - 1,
                        ship.Col + 1, '^', ConsoleColor.Cyan));
                }
                
            }
        }

    }

    public static void IntersectRocksAndProjectiles(ref List <Rock> rocks, ref List <Projectile> projectiles, ref int score)
    {
        List<Rock> rocksToRemove = new List<Rock>();
        List<Projectile> projectilesToRemove = new List<Projectile>();

        foreach (Projectile projectile in projectiles)
        {
            foreach (Rock rock in rocks)
            {
                if (projectile.Col == rock.Col && (projectile.Row - rock.Row <= 1))
                {
                    score += 50;
                    Console.SetCursorPosition(projectile.Col, projectile.Row);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('*');
                    projectilesToRemove.Add(projectile);
                    rocksToRemove.Add(rock);
                }
            }
        }

        foreach (Projectile projectile in projectilesToRemove)
        {
            projectiles.Remove(projectile);
        }

        foreach (Rock rock in rocksToRemove)
        {
            rocks.Remove(rock);
        }
    }


    public static void AdvanceProjectiles(ref List<Projectile> projectiles, ref List<Rock> rocks)
    {
        List<Projectile> projectilesToRemove = new List<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            projectile.Row--;
            if (projectile.Row == -1)
            {
                projectilesToRemove.Add(projectile);
            }
        }

        foreach (Projectile projectile in projectilesToRemove)
        {
            projectiles.Remove(projectile);
        }
    }

    public static void PrintProjectiles(ref List<Projectile> projectiles)
    {
        foreach (Projectile projectile in projectiles)
        {
            Console.ForegroundColor = projectile.Color;
            Console.SetCursorPosition(projectile.Col, projectile.Row);
            Console.Write(projectile.CharSign);
        }
    }

    public static void AdvanceRocks(ref List<Rock> rocks, ref int score)
    {
        // moving the rocks
        List<Rock> rocksToRemove = new List<Rock>();
        foreach (Rock rock in rocks)
        {
            rock.Row++;
            if (rock.Row == Console.WindowHeight)
            {
                rocksToRemove.Add(rock);
            }
        }

        foreach (Rock rock in rocksToRemove)
        {
            rocks.Remove(rock);
        }
    }

    public static void PrintRocks(ref List<Rock> rocks, ref Position ship, ref bool isDead, ref int score)
    {
        foreach (Rock rock in rocks)
        {
            if (rock.Row == Console.WindowHeight - 1
                && (rock.Col >= ship.Col && rock.Col < ship.Col + 3))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition((Console.WindowWidth / 2) - 7, Console.WindowHeight / 2 - 2);
                Console.Write("GAME OVER.");
                Console.SetCursorPosition((Console.WindowWidth / 2) - 6, (Console.WindowHeight / 2));
                Console.Write("SCORE: {0}", score);
                Console.SetCursorPosition((Console.WindowWidth / 2) - 15, (Console.WindowHeight / 2) + 2);
                Console.Write("PRESS ANY KEY TO RESTART...");
                isDead = true;
                Thread.Sleep(1100);
                return;
            }
            Console.ForegroundColor = rock.Color;
            Console.SetCursorPosition(rock.Col, rock.Row);
            Console.Write(rock.CharSign);
        }
    }
}
