using System;
using System.Windows.Forms;

namespace SimpleWinceGuiAutomation.AppTest
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [MTAThread]
        private static void Main()
        {
            Application.Run(new Form1());
        }
    }
}