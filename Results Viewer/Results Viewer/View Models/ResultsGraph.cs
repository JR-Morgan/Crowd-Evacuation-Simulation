using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Results_Core;

namespace Results_Viewer.View_Models
{
    public class ResultsGraph
    {
        private const int DecimalPlaces = 3;
        
        public PlotModel Model { get; private set; }
        public Series? Series { get; private set; }
        public string? JSON { get; private set; }

        
        private SimulationResults _results;

        public SimulationResults Results
        {
            get => _results;
            set
            {
                _results = value;
                GenerateSeries();
            }
        }
        
        private int _numberOfBars;
        public int NumberOfBars
        {
            get => _numberOfBars;
            set
            {
                _numberOfBars = value;
                GenerateSeries();
            }
        }
        
      
        public ResultsGraph()
        {
            InitialiseModel();
        }

        private void InitialiseModel()
        {
            var tmp = new PlotModel {
                Title = "",
                Subtitle = "Evacuation time per agent ordered by evacuation time",
                IsLegendVisible = false,           
            };
            tmp.Legends.Add(new Legend());

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time to evacuate (seconds)",
                Minimum = 0d,
                ExtraGridlines = new double[] { 0 },
            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Agent Frequency",
                Minimum = 0d,
                ExtraGridlines = new double[] { 0 },
            };

            tmp.Axes.Add(xAxis);
            tmp.Axes.Add(yAxis);

            this.Model = tmp;
        }
        
        public void LoadResultsFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            LoadResultsFromString(json);
        }
        
        public void LoadResultsFromString(string json)
        {
            JSON = json;
            SimulationResults results = JsonConvert.DeserializeObject<SimulationResults>(json);
            Results = results;
        }

        private void GenerateSeries()
        {
            Model.Series.Clear();
            Series = GenerateSeries(Results, NumberOfBars);
            Model.Series.Add(Series);
            OnPlotInvalid.Invoke(Model);
        }

        public event Action<PlotModel> OnPlotInvalid;
        
        private static Series GenerateSeries(SimulationResults results, int numberOfBars)
        {
            AreaSeries series = new();

            if (numberOfBars <= 0) numberOfBars = results.timeData.Length;
            
            var evacuationTime = results.timeData.SelectMany(data => data.agentStates)
                .Where(a => !a.active)
                .GroupBy(a => a.id)
                .Select(g => g.OrderBy(a => a.time).First());

            float[] density = new float[numberOfBars];
            foreach (var state in evacuationTime)
            {
                int i = (int)((state.time / results.timeToEvacuate) * numberOfBars);
                density[i]++;
            }
            
            series.Points.AddRange(density.Select((a, i) => new DataPoint(Math.Round(i * results.timeToEvacuate, DecimalPlaces), a)));
            return series;
        }
    }
}