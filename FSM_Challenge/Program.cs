using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSM_Challenge
{

    enum EnemyState
    {
        Idle, // not moving, not doing anything! Just chilling!
        Shooting, // pew pew!!!!!
        WalkingRandomly, // walk North, South, East or West 
        WalkingInLine // Walk in the opposite direction of lastEnemyPos
        
    }
    internal class Program
    {
        // You shouldn't have to change anything here
        #region program_state
        static Random random = new Random();
        static (int, int) enemyPos = (5, 5);
        static (int, int) lastEnemyPos = (4, 5);
        
        static int mapWidth = 20;
        static int mapHeight = 20;

        static (int, int) bulletPos = (0, 0);
        static bool bulletAlive = false;
        static (int, int) bulletDirection = (0, 0);

        static EnemyState currentEnemyState = EnemyState.Idle;
        
        static readonly (int, int) UI_OFFSET = (5, 5);
        #endregion

        // You shouldn't have to change anything here either!
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            DrawMap();
            while (true)
            {
                DrawMap();
                DrawEnemy();
                DrawState();
                
                // wait for bullets to finish traveling before continuing
                while(bulletAlive)
                {
                    UpdateBulletPosition();
                    DrawBullet();
                    Thread.Sleep(30);
                }
                currentEnemyState = ProcessState(currentEnemyState);

                Thread.Sleep(100);

            }

        }

        static EnemyState ProcessState(EnemyState state)
        {
            // TODO: Use rand in the switch statement to determine transitions
            double rand = random.NextDouble();

            switch (state)
            {
                case EnemyState.Idle:
                    //TODO: transition to other states based on rand
                    //HINT: you can also return EnemyState.Idle sometimes to add more variation
                    if (rand < 0.2d)return EnemyState.Idle;
                    if (rand < 0.5d) return EnemyState.Shooting;
                    else return EnemyState.WalkingRandomly;


                        

                case EnemyState.Shooting:
                    // TODO: shoot a bullet
                    // note: there is a Shoot method ready for use! 
                    Shoot();
                    if (rand < 0.1d) return EnemyState.Shooting;
                    if (rand < 0.2d) return EnemyState.WalkingRandomly;
                    if (rand < 0.9d) return EnemyState.Idle;
                    return EnemyState.Shooting;


                case EnemyState.WalkingRandomly:
                    if (rand < 0.2d)
                    {
                        Move(1, 0);
                        
                    }
                    else if (rand < 0.4d)
                    {
                        Move(-1, 0);
                        
                    }
                    else if (rand < 0.6d)
                    {
                        Move(0, 1);
                        
                    }
                    else if (rand < 0.8d)
                    {
                        Move(0, -1);
                        
                    }
                    return EnemyState.WalkingInLine;
                    //TODO: move up, down, left or right randomly
                    


                case EnemyState.WalkingInLine:
                    //TODO: move player in direction they previously moved.
                    //HINT: there is a lastEnemyPos variable that tells you the previous position!
                    if (rand < 0.2d)
                    {
                        Move(-1, 0);

                    }
                    else if (rand < 0.4d)
                    {
                        Move(1, 0);

                    }
                    else if (rand < 0.6d)
                    {
                        Move(0, -1);

                    }
                    else if (rand < 8d)
                    {
                        Move(0, -1);

                    }

                    return EnemyState.Idle;


                default:
                    return EnemyState.Idle; // this case should never happen
            }

        }

        #region METHODS_DO_NOT_EDIT

        // move by adding x and y amount to player pos. 
        // note: diagonal movement has not been tested. 
        static void Move(int x, int y)
        {

            if(IsInBounds((enemyPos.Item1 + x, enemyPos.Item2 + y)))
            {
                lastEnemyPos = enemyPos;
                enemyPos.Item1 += x;
                enemyPos.Item2 += y;
            }
        }

        // spawn a bullet in front of the player, based on the last direction they moved
        static void Shoot()
        {
            bulletDirection = (enemyPos.Item1 - lastEnemyPos.Item1, enemyPos.Item2 - lastEnemyPos.Item2);
            bulletPos = enemyPos;
            bulletPos.Item1 += bulletDirection.Item1;
            bulletPos.Item2 += bulletDirection.Item2;
            
            if(!IsInBounds(bulletPos))
            {
                bulletAlive = false;
                return;
            }

            bulletAlive = true;
            
        }
        // check if a coordinate is within the 4 walls of the map
        static bool IsInBounds((int,int) coord)
        {
            if (coord.Item1 < 0 || coord.Item1 >= mapWidth - 1) return false;
            if (coord.Item2 < 0 || coord.Item2 >= mapHeight - 1) return false;
            return true;
        }

        // move bullet forward until it is out of bounds, then turn the bullet off
        static void UpdateBulletPosition()
        {
            bulletPos.Item1 += bulletDirection.Item1;
            bulletPos.Item2 += bulletDirection.Item2;
            if (!IsInBounds(bulletPos))
            {
                bulletAlive = false;
               
            }
        }

        static void DrawEnemy()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(enemyPos.Item1, enemyPos.Item2);
            Console.Write("O");
        }

        // Don't read this method, I wrote this program at 11pm.
        static void DrawBullet()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            if (!bulletAlive)
            {
                Console.SetCursorPosition(bulletPos.Item1 - bulletDirection.Item1, bulletPos.Item2 - bulletDirection.Item2);
                Console.Write(" ");
                return;
            }
            else
            {
                Console.SetCursorPosition(bulletPos.Item1, bulletPos.Item2);
                Console.Write("\u00b7");
            }
            
            if (bulletPos.Item1 - bulletDirection.Item1 == enemyPos.Item1 && bulletPos.Item1 - bulletDirection.Item2 == enemyPos.Item2) return;

            Console.SetCursorPosition(bulletPos.Item1 - bulletDirection.Item1, bulletPos.Item2 - bulletDirection.Item2);
            Console.Write(" ");

        }

        static void DrawState()
        {
           
            Console.ForegroundColor = enemyStateConsoleColors[currentEnemyState];
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(mapWidth + UI_OFFSET.Item1, UI_OFFSET.Item2);
            Console.Write("                     ");
            Console.SetCursorPosition(mapWidth + UI_OFFSET.Item1, UI_OFFSET.Item2);
            Console.Write(currentEnemyState);
        }

        static void DrawMap()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (j == enemyPos.Item1 && i == enemyPos.Item2) continue;
                    Console.SetCursorPosition(j, i);
                    Console.Write(" ");

                }
            }
        }

        static Dictionary<EnemyState, ConsoleColor> enemyStateConsoleColors = new Dictionary<EnemyState, ConsoleColor>
        {
            { EnemyState.Idle, ConsoleColor.White },
            {EnemyState.WalkingRandomly, ConsoleColor.Green},
            {EnemyState.WalkingInLine, ConsoleColor.DarkGreen },
            {EnemyState.Shooting, ConsoleColor.Red }
        };

        #endregion

    }
}
