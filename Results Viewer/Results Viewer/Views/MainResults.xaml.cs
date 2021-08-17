using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using Results_Viewer.View_Models;

namespace Results_Viewer.Views
{
    public partial class MainResults : Window
    {
        public ResultsGraph ViewModel { get; }
        public MainResults()
        {
            InitializeComponent();
            ViewModel = new ResultsGraph();
            ViewModel.OnPlotInvalid += UpdateResults;
            DataContext = ViewModel;
        }

        private void UpdateResults(PlotModel plotModel)
        {
            pltPlot.InvalidatePlot();
            lblResults.Content = ViewModel.Results;
        }

        private void btnViewFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.FilePath)) return;
            var filePath = System.IO.Path.GetFullPath(ViewModel.FilePath);
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");

        }
    }
}