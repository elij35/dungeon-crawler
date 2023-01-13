using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace GameDev
{
    /**
     * The main class of the Dungeon Game Application
     * 
     * You may add to your project other classes which are referenced.
     * Complete the templated methods and fill in your code where it says "Your code here".
     * Do not rename methods or variables which already eDist or change the method parameters.
     * You can do some checks if your project still aligns with the spec by running the tests in UnitTest1
     * 
     * For Questions do contact us!
     */
    public class Game
    {
        /**
         * use the following to store and control the movement 3
         */
        public enum PlayerActions { NOTHING, NORTH, EAST, SOUTH, WEST, PICKUP, ATTACK, DROP, QUIT };
        private PlayerActions action = PlayerActions.NOTHING;
        public enum GameState { UNKOWN, STOP, RUN, START, INIT };
        private GameState status = GameState.INIT;

        // maps 
        private char[][] originalMap = new char[0][];
        private char[][] workingMap = new char[0][];



        /**
        * tracks if the game is running
        */
        private bool advanced = false;

        private string currentMap;

        //Assigned ints for storing the players movement
        private int x;
        private int y;

        private int coins;

        private bool iscoin;

        /**
         * Reads user input from the Console
         * 
         * Please use and implement this method to read the user input.
         * 
         * Return the input as string to be further processed
         *  
         */
        private string ReadUserInput()
        {            
            string inputRead;

            if (GameIsRunning() == GameState.RUN)
            {
                inputRead = Console.ReadKey(true).KeyChar.ToString();
            } 
            
            else
            {
                inputRead = Console.ReadLine();
            }

            return inputRead;
        }

        private int counter = -1;

        /// <summary>
        /// Returns the number of steps a player made on the current map. The counter only counts steps, not actions.
        /// </summary>
        public int GetStepCounter()
        {
            return counter;
 
        }

        /**
         * Processed the user input string
         * 
         * takes apart the user input and does control the information flow
         *  * initializes the map ( you must call InitializeMap)
         *  * starts the game when user types in Play
         *  * sets the correct playeraction which you will use in the Update
         *  
         *  DO NOT read any information from command line in here but only act upon what the method receives.
         */
        public void ProcessUserInput(string input)
        {
            string[] inputPart = input.Split(" ");

            if (inputPart[0] == "load")
            {
                LoadMapFromFile(inputPart[1]);
            }

            else if (inputPart[0] == "start" && currentMap != string.Empty)
            {
                status = GameState.RUN;
                counter = 0;
            }

            if (status == GameState.RUN)
            {
                if (input.ToLower() == "w")
                {                    
                    action = PlayerActions.NORTH;
                    counter += 1;
                }
                else if (input.ToLower() == "a")
                {                    
                    action = PlayerActions.WEST;
                    counter += 1;
                }
                else if (input.ToLower() == "s")
                {
                    action = PlayerActions.SOUTH;
                    counter += 1;
                }
                else if (input.ToLower() == "d")
                {
                    action = PlayerActions.EAST;
                    counter += 1;
                }
                else if (input.ToLower() == "z")
                {
                    action = PlayerActions.PICKUP;
                }
                else if (input.ToLower() == "q")
                {
                    action = PlayerActions.ATTACK;
                }
                else
                {
                    action = PlayerActions.NOTHING;
                }
            }  
        }

        /**
         * The Main Game Loop. 
         * It updates the game state.
         * 
         * This is the method where you implement your game logic and alter the state of the map/game
         * use playeraction to determine how the character should move/act
         * the input should tell the loop if the game is active and the state should advance
         * 
         * Returns true if the game could be updated and is ongoing
         */
        public bool Update(GameState status)
        {
            for (int y = 0; y < workingMap.Length; y++)
            {
                for (int x = 0; x < workingMap[y].Length; x++)
                {
                    if (workingMap[y][x] == 'P')
                    {
                        workingMap[y][x] = '@';
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            if (action == PlayerActions.NORTH)
            {
                //check if the next tile is a coin before moving, set the bool to true, set it to false if the coin was picked up, and when leaving the tile check if its true and put a 'C' instead of a '.'
                //then set it to false after moving

                if (workingMap[y - 1][x] == 'M' || workingMap[y - 1][x] == '#')
                {
                    //Is a monster so nothing happens
                    action = PlayerActions.NOTHING;
                }                

                if (workingMap[y - 1][x] == 'D')
                {
                    //Is a coin that needs to either be picked up or moved over
                    workingMap[y][x] = '.';
                    y = y - 1;
                    
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y - 1][x] != '#' && workingMap[y - 1][x] != 'M' && workingMap[y - 1][x] != 'D')
                {
                    if (iscoin == true)
                    {
                        workingMap[y - 1][x] = '@';
                        workingMap[y][x] = 'C';
                        y = y - 1;
                        iscoin = false;
                        return true;
                    }                    

                    if (workingMap[y - 1][x] == 'C')
                    {
                        iscoin = true;
                        workingMap[y - 1][x] = '@';
                        workingMap[y][x] = '.';
                        y = y - 1;
                    }

                    else if (iscoin == false)
                    {
                        workingMap[y - 1][x] = '@';
                        workingMap[y][x] = '.';
                        y = y - 1;
                    }                    
                }
            }

            else if (action == PlayerActions.EAST)
            {           
                if (workingMap[y][x + 1] == 'M' || workingMap[y][x + 1] == '#')
                {
                    //Is a monster so nothing happens
                    action = PlayerActions.NOTHING;
                }               

                if (workingMap[y][x + 1] == 'D')
                {
                    //Is a monster so nothing happens
                    workingMap[y][x] = '.';
                    x = x + 1;                    
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y][x + 1] != '#' && workingMap[y][x + 1] != 'M' && workingMap[y][x + 1] != 'D')
                {
                    workingMap[y][x + 1] = '@';
                    workingMap[y][x] = '.';
                    x = x + 1;
                }
            }

            else if (action == PlayerActions.SOUTH)
            {               
                if (workingMap[y + 1][x] == 'M' || workingMap[y + 1][x] == '#')
                {
                    //Is a monster so nothing happens
                    action = PlayerActions.NOTHING;
                }                

                if (workingMap[y + 1][x] == 'D')
                {
                    //Is a coin that needs to either be picked up or moved over
                    workingMap[y][x] = '.';
                    y = y + 1;                    
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y + 1][x] != '#' && workingMap[y + 1][x] != 'M' && workingMap[y + 1][x] != 'D')
                {
                    workingMap[y + 1][x] = '@';
                    workingMap[y][x] = '.';
                    y = y + 1;
                }
            }            

            else if (action == PlayerActions.WEST) 
            {                
                if (workingMap[y][x - 1] == 'M' || workingMap[y][x - 1] == '#')
                {
                    //Is a monster so nothing happens
                    action = PlayerActions.NOTHING;
                }                

                if (workingMap[y][x - 1] == 'D')
                {
                    //Ends game
                    workingMap[y][x] = '.';
                    x = x - 1;                    
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y][x - 1] != '#' && workingMap[y][x - 1] != 'M' && workingMap[y][x - 1] != 'D')
                {
                    workingMap[y][x - 1] = '@';
                    workingMap[y][x] = '.';
                    x = x - 1;
                }
            }

            //Change the below:
            else if (action == PlayerActions.PICKUP && iscoin == true)
            {              
                coins += 1;
                iscoin = false;
                Console.WriteLine("Pressed z");                
            }

            return false;
        }

        /**
         * The Main Visual Output element. 
         * It draws the new map after the player did something onto the screen.
         * 
         * This is the method where you implement your the code to draw the map ontop the screen
         * and show the move to the user. 
         * 
         * The method returns true if the game is running and it can draw something, false otherwise.
        */
        public bool PrintMapToConsole()
        {
            for (int y = 0; y < workingMap.Length; y++)
            {
                for (int x = 0; x < workingMap[y].Length; x++)
                {
                    Console.Write(workingMap[y][x]);
                }
                Console.Write(Environment.NewLine);
            }

            if (status == GameState.STOP)
            {
                Console.WriteLine("Congratulations! You have completed the game!!");
                Console.WriteLine("Total number of moves: " + counter);
                Console.WriteLine("Total coins collected: " + coins);
                Console.Write(Environment.NewLine);
            }            

            return true;
        }
        /**
         * Additional Visual Output element. 
         * It draws the flavour texts and additional information after the map has been printed.
         * 
         * This is the method does not need to be used unless you want to output somethign else after the map onto the screen.
         * 
         */
        public bool PrintExtraInfo()
        {
            if (status == GameState.RUN)
            {
                Console.WriteLine("Number of moves: " + counter);
                Console.WriteLine("Coins collected: " + coins);
            }           

            return false;
        }

        /**
        * Map and GameState get initialized
        * mapName references a file name 
        * Do not use abosolute paths but use the files which are relative to the eDecutable.
        * 
        * Create a private object variable for storing the map in Game and using it in the game.
        */
        public bool LoadMapFromFile(String mapName)
        {
            FileInfo fileInfo = new FileInfo("maps/" + mapName);

            //Checking file exists
            if (!fileInfo.Exists)
            {
                return false;
            }

            //Creating new string
            var list = new List<string>();

            string line;

            //filestream is a variable for opening the file
            var fileStream = new FileStream("maps/" + mapName, FileMode.Open, FileAccess.Read);

            using (var streamReader = new StreamReader(fileStream))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.Trim() != string.Empty)
                    {
                        list.Add(line);
                    }
                }
            }

            originalMap = new char[list.Count][];

            for (int i = 0; i < list.Count; i++)
            {
                originalMap[i] = list[i].ToCharArray();
            }

            workingMap = new char[originalMap.Length][];

            originalMap.CopyTo(workingMap, 0);

            status = GameState.START;

            for (int y = 0; y < workingMap.Length; y++)
            {
                for (int x = 0; x < workingMap[y].Length; x++)
                {
                    if (workingMap[y][x] == 'P')
                    {
                        this.x = x;
                        this.y = y;
                    }
                }
            }

            return true;
        }
                
        /**
         * Returns a representation of the currently loaded map
         * before any move was made.
         * This map should not change when the player moves
         */
        public char[][] GetOriginalMap()
        {
            return originalMap;
        }

        /**
         * Returns the current map state and contains the player's move
         * without altering it 
         */
        public char[][] GetCurrentMapState()
        {
            return workingMap;
        }

        /**
         * Returns the current position of the player on the map
         * 
         * The first value is the y coordinate and the second is the x coordinate on the map
         */ 
        public int[] GetPlayerPosition()
        {
            int[] swap = { y, x };                       

            return swap;
        }
        
        /**
        * Returns the next player action
        * 
        * This method does not alter any internal state
        */
        public int GetPlayerAction()
        {
            //returns player action
            return (int) action;
        }

        public GameState GameIsRunning()
        {
            //returns status of game                        
            return status;
        }

        /**
         * Main method and Entry point to the program
         * ####
         * Do not change! 
        */
        static void Main(string[] args)
        {
            Game crawler = new Game();

            string input = string.Empty;
            Console.WriteLine("Welcome to the Commandline Dungeon!" + Environment.NewLine +
                "May your Quest be filled with riches!" + Environment.NewLine);

            // Loops through the input and determines when the game should quit
            while (crawler.GameIsRunning() != GameState.STOP && crawler.GameIsRunning() != GameState.UNKOWN)
            {
                Console.Write("Your Command: ");
                input = crawler.ReadUserInput();
                Console.WriteLine(Environment.NewLine);
                crawler.ProcessUserInput(input);
                crawler.Update(crawler.GameIsRunning());
                crawler.PrintMapToConsole();
                crawler.PrintExtraInfo();
            }

            Console.WriteLine("See you again" + Environment.NewLine +
                "In the CMD Dungeon! ");
        }
    }
}