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
         * use the following to store and control the movement 
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
            string inputRead = string.Empty;
            //modify
            return inputRead;
        }

        private int counter = -1;

        /// <summary>
        /// Returns the number of steps a player made on the current map. The counter only counts steps, not actions.
        /// </summary>
        public int GetStepCounter()
        {
            //modify
            return -1;
 
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
            //modify

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
            //modify
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
            //modify
            return false;
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
            //modify
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
            //modify

            return false;
        }

        /**
         * Returns a representation of the currently loaded map
         * before any move was made.
         * This map should not change when the player moves
         */
        public char[][] GetOriginalMap()
        {
            //modify
            return new char[2][];
        }

        /*
         * Returns the current map state and contains the player's move
         * without altering it 
         */
        public char[][] GetCurrentMapState()
        {
            //modify
            return new char[1][];
        }

        /**
         * Returns the current position of the player on the map
         * 
         * The first value is the y coordinate and the second is the x coordinate on the map
         */
        public int[] GetPlayerPosition()
        {
            //modify
            return new int[2];
        }

        /**
        * Returns the next player action
        * 
        * This method does not alter any internal state
        */
        public int GetPlayerAction()
        {
            //modify
            return -1;
        }

        public GameState GameIsRunning()
        {
            //modify
            return GameState.UNKOWN;
        }

        /**
         * Main method and Dntry point to the program
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