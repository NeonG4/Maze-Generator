using System.Numerics;

namespace Maze_Generation
{
    internal class MazeNode
    {
        public bool[] type;
        public bool touched = false;
        public MazeNode(bool[] type) 
        {
            if (type.Length != 4)
            {
                throw new Exception($"Expected array of length 4, got {type.Length} instead"); // tiles are squares, so 4 sides are needed.
            }
            this.type = type;
        }
        public static bool IsFinished(MazeNode[,] nodes, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (nodes[i, j].touched == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {

            // maze setup
            int width = 50, height = 25;
            MazeNode[,] maze = new MazeNode[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // maze is initialized to empty. Rather than carving out a maze, the system builds it up
                    maze[i, j] = new MazeNode([false, false, false, false]);
                }
            }
            // maze generation
            int chanceOfLoopAround = 7; // the higher, the smaller. Lowest value of 1; 5 is a good value. Higher values will take longer to run
            Random rand = new Random();
            Vector2 position = new Vector2(0, 0); // used as the "cursor" in the maze. this is the position that gets written over
            List<int> directions = new List<int>();
            while (!MazeNode.IsFinished(maze, width, height))
            {
                directions.Clear();
                // this set of if statements tracks which directions are possible to continue the maze path
                if (position.X + 1 < width)
                {
                    if (maze[(int)position.X + 1, (int)position.Y].touched == false)
                    {
                        directions.Add(0);
                    }
                    else if (rand.Next(0, chanceOfLoopAround) == 0)
                    {
                        directions.Add(0);
                    }
                }
                if (position.Y + 1 < height)
                {
                    if (maze[(int)position.X, (int)position.Y + 1].touched == false)
                    {
                        directions.Add(1);
                    }
                    else if (rand.Next(0, chanceOfLoopAround) == 0)
                    {
                        directions.Add(1);
                    }
                }
                if (position.X - 1 >= 0)
                {
                    if (maze[(int)position.X - 1, (int)position.Y].touched == false)
                    {
                        directions.Add(2);
                    }
                    else if (rand.Next(0, chanceOfLoopAround) == 0)
                    {
                        directions.Add(2);
                    }
                }
                if (position.Y - 1 >= 0)
                {
                    if (maze[(int)position.X, (int)position.Y - 1].touched == false)
                    {
                        directions.Add(3);
                    }
                    else if (rand.Next(0, chanceOfLoopAround) == 0)
                    {
                        directions.Add(3);
                    }
                }
                if (directions.Count == 0) // moves the cursor if there's no where to go
                {
                    // rather than picking a random map location, pick one that has a maze that went through nearby, and connect to that maze
                    // this connects the maze together, instead of having seperate sections
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            if (!maze[i, j].touched) // each maze node has a "touched" property. The cursor will touch tiles, but then it won't want to go back over them. 
                            {
                                List<Vector2> touchings = new List<Vector2>();
                                // this makes sure the cursor is within 1 tile of a pregenerated maze
                                if (i > 0)
                                {
                                    if (maze[i-1, j].touched)
                                    {
                                        touchings.Add(new Vector2(-1, 0));
                                    }
                                }
                                if (j > 0)
                                {
                                    if (maze[i, j- 1].touched)
                                    {
                                        touchings.Add(new Vector2(0, -1));
                                    }
                                }
                                if (i < width - 2)
                                {
                                    if (maze[i + 1, j].touched)
                                    {
                                        touchings.Add(new Vector2(1, 0));
                                    }
                                }
                                if (j < height - 2)
                                {
                                    if (maze[i, j+ 1].touched)
                                    {
                                        touchings.Add(new Vector2(0, 1));
                                    }
                                }
                                if (touchings.Count > 0) 
                                {
                                    Vector2 dir = touchings[rand.Next(0, touchings.Count)];
                                    // needs to get the current tile side, and the applied tile side
                                    int side = 0;
                                    int flipside = 3;
                                    // this switch case is used to convert the vector2 dir into an index in the boolean array
                                    switch (dir.X.ToString() + dir.Y.ToString())
                                    {
                                        case "10":
                                            {
                                                side = 0;
                                                flipside = 2;
                                                break;
                                            }
                                        case "01":
                                            {
                                                side = 1;
                                                flipside = 3;
                                                break;
                                            }
                                        case "-10":
                                            {
                                                side = 2;
                                                flipside = 0;
                                                break;
                                            }
                                        case "0-1":
                                            {
                                                side = 3;
                                                flipside = 1;
                                                break;
                                            }
                                    }
                                    maze[i, j].type[side] = true;
                                    maze[i + (int)dir.X, j + (int)dir.Y].type[flipside] = true;
                                    position = new Vector2(i, j);
                                    i = width;
                                    j = height;
                                }
                            }
                        }
                    }
                    directions.Add(0);
                    RenderMap(maze, width, height, false);
                }
                else
                {
                    int dir = directions[rand.Next(0, directions.Count)];
                    maze[(int)position.X, (int)position.Y].touched = true;
                    maze[(int)position.X, (int)position.Y].type[dir] = true;
                    // prevents one way paths
                    if (dir == 0)
                    {
                        position.X++;
                        dir = 2;
                    }
                    else if (dir == 1)
                    {
                        position.Y++;
                        dir = 3;
                    }
                    else if (dir == 2)
                    {
                        position.X--;
                        dir = 0;
                    }
                    else if (dir == 3)
                    {
                        position.Y--;
                        dir = 1;
                    }
                    maze[(int)position.X, (int)position.Y].touched = true;
                    maze[(int)position.X, (int)position.Y].type[dir] = true;
                }
            }
            RenderMap(maze, width, height, true);
        }
        static void RenderMap(MazeNode[,] maze, int width, int height, bool wait)
        {
            Console.Clear();
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (maze[i, j].touched) // if maze hasn't been touched, then don't draw anything there
                    {
                        char print = new char();
                        // switch determines which maze code to print
                        switch (maze[i, j].type)
                        {
                            case [true, true, true, true]:
                                print = '╬';
                                break;
                            case [true, false, true, true]:
                                print = '╩';
                                break;
                            case [true, true, false, true]:
                                print = '╠';
                                break;
                            case [true, true, true, false]:
                                print = '╦';
                                break;
                            case [false, true, true, true]:
                                print = '╣';
                                break;
                            case [true, true, false, false]:
                                print = '╔';
                                break;
                            case [true, false, true, false]:
                                print = '═';
                                break;
                            case [false, true, true, false]:
                                print = '╗';
                                break;
                            case [false, true, false, true]:
                                print = '║';
                                break;
                            case [true, false, false, true]:
                                print = '╚';
                                break;
                            case [false, false, false, true]:
                                print = '╨';
                                break;
                            case [false, false, true, false]:
                                print = '╡';
                                break;
                            case [false, true, false, false]:
                                print = '╥';
                                break;
                            case [true, false, false, false]:
                                print = '╞';
                                break;
                            case [false, false, true, true]:
                                print = '╝';
                                break;
                            default:
                                print = 'x';
                                break;
                        }
                        Console.Write(print);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write("\n");
            }
            if (wait)
            {
                Console.ReadKey();
            }
        }
    }
}
