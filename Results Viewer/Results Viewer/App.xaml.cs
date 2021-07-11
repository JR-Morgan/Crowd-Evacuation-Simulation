using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Results_Viewer.Views;

namespace Results_Viewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
           
        private void AppStartup(object sender, StartupEventArgs e)
        {
            MainResults results = new();
            if (e.Args.Length > 0)
            {
                string fileToLoad = e.Args[0];
                results.ViewModel.LoadResultsFromFile(fileToLoad);
            }


            results.Show();
        }
        
    }
}