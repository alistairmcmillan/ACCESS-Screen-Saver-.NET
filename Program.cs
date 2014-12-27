using System;
using System.Windows.Forms;
using System.Globalization;

namespace AlistairMcMillan.ACCESSScreenSaver
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                // Get the 2 character command line argument
                string arg = args[0].ToUpperInvariant().Trim().Substring(0, 2);
                switch (arg)
                {
                    case "/C":
                        // Show the options dialog
                        ShowOptions();
                        break;
                    case "/P":
                        // Don't do anything for preview
                        break;
                    case "/S":
                        // Show screensaver form
                        ShowScreenSaver();
                        break;
                    case "/D":
                        // Show screensver in debug mode
                        ShowScreenSaver(true);
                        break;
                    default:
                        MessageBox.Show("Invalid command line argument: " + arg, "Invalid Command Line Argument", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                // If no arguments were passed in, show the screensaver
                ShowScreenSaver();
            }
        }
        
        static void ShowOptions()
        {
            OptionsForm optionsForm = new OptionsForm();
            Application.Run(optionsForm);
        }

        static void ShowScreenSaver()
        {
            ShowScreenSaver(false);
        }
        
        static void ShowScreenSaver(bool debugmode)
        {
            ScreenSaverForm screenSaver = new ScreenSaverForm(debugmode);
            Application.Run(screenSaver);
        }
    }
}