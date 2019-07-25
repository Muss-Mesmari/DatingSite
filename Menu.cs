﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatingSite.Demo
{

    class Menu
    {
        private List<MenuOption> menuOptions = new List<MenuOption>();
        private ConsoleKey currentPageKey;
        public bool Quit = false;

        class MenuOption
        {
            public Action AppMethod { get; }
            public ConsoleKey Key { get; }
            public string Prompt { get; }
            public MenuOption(Action appMethod, ConsoleKey key, string prompt)
            {
                AppMethod = appMethod;
                Key = key;
                Prompt = prompt;
            }
        }
        
        public void SetupAdminMenu(App app)
        {
            currentPageKey = ConsoleKey.A;
            NewMenuOption(app.PageMainMenu, ConsoleKey.A, "a) Main menu");
            NewMenuOption(app.PageCreateQuestion, ConsoleKey.B, "b) Create a question");
            NewMenuOption(app.PageDeleteQuestion, ConsoleKey.C, "c) Delete a question");           
        }
        
            public void SetupAppMenu(App app)
        {
            currentPageKey = ConsoleKey.A;
            NewMenuOption(app.PageMainMenu, ConsoleKey.A, "a) Main menu");
            NewMenuOption(app.PageAddPerson, ConsoleKey.B, "b) Register an account");         
            NewMenuOption(app.PageDeletePerson, ConsoleKey.C, "c) Delete an account"); 
            NewMenuOption(app.PageAnswerQuestions, ConsoleKey.D, "d) Answer questions");
            NewMenuOption(app.PageCheckMatch, ConsoleKey.E, "e) View matchning");
            
        }

        private void NewMenuOption(Action menuAction, ConsoleKey hotkey, string prompt)
        {
            menuOptions.Add(new MenuOption(menuAction, hotkey, prompt));
        }
        public void SwitchPageByUserInput()
        {
            Console.WriteLine("What do you want to do?");
            menuOptions.ForEach(x => Console.WriteLine(x.Prompt));

            ConsoleKey keyInput = Console.ReadKey().Key;

            if (menuOptions.Select(x => x.Key).Contains(keyInput))
                currentPageKey = keyInput;
            else
                currentPageKey = ConsoleKey.A;
        }

        public void MainMenu()
        {
            currentPageKey = ConsoleKey.A;
        }
        public void RefreshMenu()
        {
            Action menuAction = menuOptions.Where(x => x.Key == currentPageKey).First().AppMethod;
            menuAction();
        }
    }
}
