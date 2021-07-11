using System.Windows;
using System.Windows.Controls;
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
            ViewModel.OnPlotInvalid += _ => pltPlot.InvalidatePlot();;
            DataContext = ViewModel;
        }

        private void cboXDataGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void cboYDataGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void btnExportJSON_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void btnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        
    }
}