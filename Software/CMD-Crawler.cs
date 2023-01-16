using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

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

        private int coins; //int to store the number of coins collected
        private bool isCoin; //bool value to check if there is a coin to pick up       
        private bool mapLoad; //bool value to check if the map entered by the user exists

        private int[][] monsterPosition = new int[0][]; //int for storing the monster location in loaded map
        private int numberOfMonsters = 0; //int for counting number of monsters in loaded map

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
            string inputRead; //String for the user input
                        
            if (GameIsRunning() == GameState.RUN) //Checks if the game is running
            {                
                inputRead = Console.ReadKey(true).KeyChar.ToString(); //Storing the character entered by user
            }             
            else
            {                
                inputRead = Console.ReadLine(); //Reads the entire line of user's input
            }            
            return inputRead; //Returns users input as string
        }
        
        private int counter = -1; //Counter set to -1 before the game starts

        /// <summary>
        /// Returns the number of steps a player made on the current map. The counter only counts steps, not actions.
        /// </summary>
        public int GetStepCounter()
        {            
            return counter; //Returns the int counter
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
            string[] inputPart = input.Split(" "); //A string to split the user input per word

            if (inputPart[0] == "load") //Checking if first word is load
            {
                if (inputPart.Length > 1) //Stops crashing if nothing is inputted after load
                {
                    LoadMapFromFile(inputPart[1]); //Runs loadmapfromfile [then the map name]

                    if (LoadMapFromFile(inputPart[1]) == true) //Checking if the map file exists
                    {
                        mapLoad = true; //If the map exits change mapload to true
                        if (inputPart[1] == "advanced.map") //Can only run advanced.map with advanced mode
                        {
                            advanced = true;
                            Console.WriteLine("Advanced mode is now on!!!"); //Message for letting the user know advanced mode is enabled
                            Console.WriteLine("\n"); //Adds a new line
                            LoadMapFromFile(inputPart[1]); //Re runs the load map from file
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not a map file!"); //Outputting the error if map entered isn't a file
                        Console.WriteLine("Map choice: simple.map / simple2.map / advanced.map");
                        Console.WriteLine("\n");
                    }
                }
                else
                {
                    Console.WriteLine("Not a map file!"); //Outputting the error if map entered isn't a file
                    Console.WriteLine("Map choice: simple.map / simple2.map / advanced.map");
                    Console.WriteLine("\n");
                }
            }

            else if (inputPart[0] == "advanced") //If "advanced" is entered
            {
                if (advanced != true)
                {
                    advanced = true; //Sets advanced to true
                    Console.WriteLine("Advanced mode is now on!"); //Message for letting the user know advanced mode is enabled
                    Console.WriteLine("\n"); //Adds a new line
                    locateMonsterPositions(); //Locates monster movement
                }
                else
                {
                    Console.WriteLine("Advanced mode already enabled!");
                }
            }

            else if (inputPart[0] == "start") //If word entered is "start"
            {
                if (mapLoad == true) //Checking if the mapload (meaning map has loaded correctly)
                {
                    status = GameState.RUN; //Change GameState to RUN as game has now started                                        
                    counter = 0; //Changes movement counter to 0 (was set to -1 as game wasn't started before this)
                }
                else
                {
                    Console.WriteLine("You need to choose a map to load!"); //Output to console if user forgets to load a map
                }
            }

            if (status == GameState.RUN) //If the game has started, start reading the character input to move the player
            {
                if (input.ToLower() == "w") //For moving north
                {
                    action = PlayerActions.NORTH;
                    counter += 1;
                }
                else if (input.ToLower() == "a") //For moving west
                {
                    action = PlayerActions.WEST;
                    counter += 1;
                }
                else if (input.ToLower() == "s") //For moving south
                {
                    action = PlayerActions.SOUTH;
                    counter += 1;
                }
                else if (input.ToLower() == "d") //For moving east
                {
                    action = PlayerActions.EAST;
                    counter += 1;
                }
                else if (input.ToLower() == "z") //For picking up coins
                {
                    action = PlayerActions.PICKUP;
                }
                else if (input.ToLower() == "q") //For attacking monsters
                {
                    action = PlayerActions.ATTACK;
                }
                else
                {
                    action = PlayerActions.NOTHING; //Does nothing if any other key is pressed
                }
            }
        }
        public int monsterRandMove()
        {
            //For monster's random movement, it outputs random numbers from 1 to 4
            Random rnd = new Random();
            int move = rnd.Next(1, 5);
            return move;
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
            for (int y = 0; y < workingMap.Length; y++) //Moves through array in y (bottom to top)
            {
                for (int x = 0; x < workingMap[y].Length; x++) //Moves through array in x (left to right)
                {
                    if (workingMap[y][x] == 'P') //If it finds a "P" in working map it replaces it with an "@" symbol
                    {
                        workingMap[y][x] = '@';
                    }
                    else
                    {
                        continue; //If "P" isn't in that location it continues
                    }
                }
            }

            if (action == PlayerActions.NORTH) //For player moving north
            {
                for (int x = 0; x < numberOfMonsters; x++)
                {
                    int monsterDirection = monsterRandMove(); //Gets a random number to decide which way the monster will move

                    if (monsterDirection == 1) //Monster will move North
                    {
                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 2) //Monster will move East
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'C') //Ensure it doesn't enter the coin tile
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 3) //Monster will move South
                    {
                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 4) //Monster will move West
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        continue;
                    }
                }

                if (workingMap[y - 1][x] == 'M' || workingMap[y - 1][x] == '#')
                {
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y - 1][x] == 'D')
                {
                    workingMap[y][x] = '.';
                    y = y - 1;
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y - 1][x] != '#' && workingMap[y - 1][x] != 'M' && workingMap[y - 1][x] != 'D')
                {
                    if (isCoin == true)
                    {
                        workingMap[y - 1][x] = '@';
                        workingMap[y][x] = 'C';
                        y = y - 1;
                        isCoin = false;
                        return true;
                    }

                    if (workingMap[y - 1][x] == 'C')
                    {
                        isCoin = true;
                        workingMap[y - 1][x] = '@';
                        workingMap[y][x] = '.';
                        y = y - 1;
                    }

                    else if (isCoin == false)
                    {
                        workingMap[y - 1][x] = '@';
                        workingMap[y][x] = '.';
                        y = y - 1;
                    }
                }
            }

            else if (action == PlayerActions.EAST) //For player moving east
            {
                for (int x = 0; x < numberOfMonsters; x++)
                {
                    int monsterDirection = monsterRandMove(); //Gets a random number to decide which way the monster will move

                    if (monsterDirection == 1) //Monster will move North
                    {
                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 2) //Monster will move East
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'C') //Ensure it doesn't enter the coin tile
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 3) //Monster will move South
                    {
                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 4) //Monster will move West
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        continue;
                    }
                }

                if (workingMap[y][x + 1] == 'M' || workingMap[y][x + 1] == '#')
                {
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y][x + 1] == 'D')
                {
                    workingMap[y][x] = '.';
                    x = x + 1;
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y][x + 1] != '#' && workingMap[y][x + 1] != 'M' && workingMap[y][x + 1] != 'D')
                {
                    if (isCoin == true)
                    {
                        workingMap[y][x + 1] = '@';
                        workingMap[y][x] = 'C';
                        x = x + 1;
                        isCoin = false;
                        return true;
                    }

                    if (workingMap[y][x + 1] == 'C')
                    {
                        isCoin = true;
                        workingMap[y][x + 1] = '@';
                        workingMap[y][x] = '.';
                        x = x + 1;
                    }

                    else if (isCoin == false)
                    {
                        workingMap[y][x + 1] = '@';
                        workingMap[y][x] = '.';
                        x = x + 1;
                    }
                }
            }

            else if (action == PlayerActions.SOUTH) //For player moving South
            {
                for (int x = 0; x < numberOfMonsters; x++)
                {
                    int monsterDirection = monsterRandMove(); //Gets a random number to decide which way the monster will move

                    if (monsterDirection == 1) //Monster will move North
                    {
                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 2) //Monster will move East
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'C') //Ensure it doesn't enter the coin tile
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 3) //Monster will move South
                    {
                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 4) //Monster will move West
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        continue;                    
                    }
                }       

                if (workingMap[y + 1][x] == 'M' || workingMap[y + 1][x] == '#')
                {
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y + 1][x] == 'D')
                {
                    workingMap[y][x] = '.';
                    y = y + 1;
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y + 1][x] != '#' && workingMap[y + 1][x] != 'M' && workingMap[y + 1][x] != 'D')
                {
                    if (isCoin == true)
                    {
                        workingMap[y + 1][x] = '@';
                        workingMap[y][x] = 'C';
                        y = y + 1;
                        isCoin = false;
                        return true;
                    }

                    if (workingMap[y + 1][x] == 'C')
                    {
                        isCoin = true;
                        workingMap[y + 1][x] = '@';
                        workingMap[y][x] = '.';
                        y = y + 1;
                    }

                    else if (isCoin == false)
                    {
                        workingMap[y + 1][x] = '@';
                        workingMap[y][x] = '.';
                        y = y + 1;
                    }
                }
            }

            else if (action == PlayerActions.WEST) //For player moving west
            {
                for (int x = 0; x < numberOfMonsters; x++)
                {
                    int monsterDirection = monsterRandMove(); //Gets a random number to decide which way the monster will move

                    if (monsterDirection == 1) //Monster will move North
                    {
                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] - 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 2) //Monster will move East
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'C') //Ensure it doesn't enter the coin tile
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] + 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 3) //Monster will move South
                    {
                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0] + 1][monsterPosition[x][1]] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][0] += 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (monsterDirection == 4) //Monster will move West
                    {
                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'C')
                        {
                            if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'M') //Ensure it doesn't enter another monster
                            {
                                if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != 'D') //Ensure it doesn't enter the end tile
                                {
                                    if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '#') //Ensure it doesn't enter the walls
                                    {
                                        if (workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] != '@') //Ensure it doesn't enter the walls
                                        {
                                            if (advanced == true) //Checks advanced is enabled
                                            {
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1] - 1] = 'M'; //Removes new place with the letter M
                                                workingMap[monsterPosition[x][0]][monsterPosition[x][1]] = '.'; //Replaces the current monster position with .
                                                monsterPosition[x][1] -= 1; //Updates monster's position to the next location along
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        continue;
                    }
                }

                if (workingMap[y][x - 1] == 'M' || workingMap[y][x - 1] == '#')
                {
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y][x - 1] == 'D')
                {
                    workingMap[y][x] = '.';
                    x = x - 1;
                    this.status = GameState.STOP;
                    action = PlayerActions.NOTHING;
                }

                if (workingMap[y][x - 1] != '#' && workingMap[y][x - 1] != 'M' && workingMap[y][x - 1] != 'D')
                {
                    if (isCoin == true)
                    {
                        workingMap[y][x - 1] = '@';
                        workingMap[y][x] = 'C';
                        x = x - 1;
                        isCoin = false;
                        return true;
                    }

                    if (workingMap[y][x - 1] == 'C')
                    {
                        isCoin = true;
                        workingMap[y][x - 1] = '@';
                        workingMap[y][x] = '.';
                        x = x - 1;
                    }

                    else if (isCoin == false)
                    {
                        workingMap[y][x - 1] = '@';
                        workingMap[y][x] = '.';
                        x = x - 1;
                    }
                }
            }

            else if (action == PlayerActions.PICKUP && isCoin == true)
            {
                coins += 1;
                isCoin = false;
            }
            return false;
        }
        public bool locateMonsterPositions() //Locates all monsters in the loaded map
        {
            //This checks for all the monsters 'M' in the loaded map file and increase the numberOfMonsters counter for each 'M' found
            for (int y = 0; y < workingMap.Length; y++)
            {
                for (int x = 0; x < workingMap[y].Length; x++)
                {
                    if (workingMap[y][x] == 'M')
                    {
                        numberOfMonsters++;
                    }
                }
            }

            //Allocating each monster to the monsterPosition array
            monsterPosition = new int[numberOfMonsters][];
            for (int x = 0; x < numberOfMonsters; x++)
            {
                monsterPosition[x] = new int[2];
            }

            //Verifying the monsters in the monsterPosition array
            int monsterVerification = 0;
            while (monsterVerification != numberOfMonsters) //Loops until it verifies each monster's position
            {
                for (int y = 0; y < workingMap.Length; y++)
                {
                    for (int x = 0; x < workingMap[y].Length; x++)
                    {
                        if (workingMap[y][x] == 'M')
                        {
                            monsterPosition[monsterVerification][0] = y;
                            monsterPosition[monsterVerification][1] = x;
                            monsterVerification++;
                        }
                    }
                }
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
            //Prints the loaded map to the console
            for (int y = 0; y < workingMap.Length; y++)
            {
                for (int x = 0; x < workingMap[y].Length; x++)
                {
                    Console.Write(workingMap[y][x]);
                }
                Console.Write(Environment.NewLine);
            }

            //If player reaches the 'D' it outputs a message
            if (status == GameState.STOP)
            {
                Console.WriteLine("Congratulations! You have completed the game!!");
                Console.WriteLine("Steps taken: " + counter);
                Console.WriteLine("Coins collected: " + coins);
                Console.Write(Environment.NewLine);
                Console.ReadKey();
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
            //Prints coins collected in normal mode
            if (status == GameState.RUN && advanced == false)
            {
                Console.WriteLine("Steps taken: " + counter);
                Console.WriteLine("Coins collected: " + coins);
            }

            //Prints gold pieces collected in advanced mode
            if (status == GameState.RUN && advanced == true)
            {
                Console.WriteLine("Steps taken: " + counter);
                Console.WriteLine("Gold pieces collected: " + coins);
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

            if (!fileInfo.Exists) //Checking file exists
            {
                return false; //If it doesn't exist it returns false
            }

            var list = new List<string>(); //Creating new string
            string line; //String for adding lines
            var fileStream = new FileStream("maps/" + mapName, FileMode.Open, FileAccess.Read); //filestream is a variable for opening the file

            using (var streamReader = new StreamReader(fileStream)) //Opening file using streamreader
            {
                while ((line = streamReader.ReadLine()) != null) //Reading lines until there are no lines left to read
                {
                    if (line.Trim() != string.Empty) //If line contains nothing then it won't add it to the list
                    {
                        list.Add(line); //Adding each line to a list
                    }
                }
            }

            originalMap = new char[list.Count][]; //Creating new char for the list length

            for (int i = 0; i < list.Count; i++) //Loops until the length of list has been reached
            {
                originalMap[i] = list[i].ToCharArray(); //Adding content from list array to the original map
            }

            workingMap = new char[originalMap.Length][]; //Working map now equals content of original map in char form
            originalMap.CopyTo(workingMap, 0); //Copying the original map to working map
            status = GameState.START; //Changes gamestate to start to start game

            for (int y = 0; y < workingMap.Length; y++) //Moves up the array (y axis) until it reaches the height of array
            {
                for (int x = 0; x < workingMap[y].Length; x++) //Moves across the array (x axis) until it reaches the width of array
                {
                    if (workingMap[y][x] == 'P') //If "P" is found in the array then store the coordinates
                    {
                        this.x = x; //Store x location
                        this.y = y; //Store y location
                    }
                }
            }

            //Locates monster positions if advanced mode has been enabled
            if (advanced == true)
            {
                locateMonsterPositions();
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