using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ImportJsonApplication.Utilities;
using Microsoft.Win32;
using Newtonsoft.Json;
using PlethoraModels;
using Prism.Commands;
using Prism.Mvvm;
using LineSegment = PlethoraModels.LineSegment;
using Point = PlethoraModels.Point;

namespace ImportJsonApplication
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            InitDelegateCommands();
            _textColor = Brushes.Gray;
            _inputFilename = "No File Chosen";
            _outputLog = "Drop Json file here...";
        }

        #region Initialization

        /// <summary>
        /// Initialize Commands
        /// </summary>
        private void InitDelegateCommands()
        {
            BrowseCommand = new DelegateCommand(BrowseCommand_Execute);
            ImportCommand = new DelegateCommand<string>(ImportCommand_Execute);
        }

        #endregion

        #region Properties

        public string InputFilename
        {
            get { return _inputFilename; }
            set { SetProperty(ref _inputFilename, value); }
        }
        private string _inputFilename;

        public string OutputLog
        {
            get { return _outputLog; }
            set { SetProperty(ref _outputLog, value); }
        }
        private string _outputLog;

        public Brush TextColor
        {
            get { return _textColor; }
            set { SetProperty(ref _textColor, value); }
        }
        private Brush _textColor;

        public string MaterialCost
        {
            get { return _materialCost; }
            set { SetProperty(ref _materialCost, value); }
        }
        private string _materialCost;

        public string MachineCost
        {
            get { return _machineCost; }
            set { SetProperty(ref _machineCost, value); }
        }
        private string _machineCost;

        public string TotalCost
        {
            get { return _totalCost; }
            set { SetProperty(ref _totalCost, value); }
        }
        private string _totalCost;

        private Dictionary<int, Edge> _edges;
        private Dictionary<int, Vertex> _vertices;
        private List<LineSegment> _lineSegments;
        private List<CircularArc> _circularArcs;
        
        #endregion

        #region Commands

        public DelegateCommand BrowseCommand { get; set; }
        public DelegateCommand<string> ImportCommand { get; set; }

        /// <summary>
        /// This is called when the user clicks the Browse button
        /// </summary>
        private void BrowseCommand_Execute()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Profile Json Files|*.json",
                Title = "Select a Json File"
            };
            openFileDialog.ShowDialog();
            InputFilename = openFileDialog.FileName;
            TextColor = Brushes.Black;
        }

        /// <summary>
        /// This is called when the import button is pressed or when the user drops a json file inside the dialog box
        /// </summary>
        /// <param name="jsonFile"></param>
        private void ImportCommand_Execute(string jsonFile)
        {
            InputFilename = Path.GetFileName(jsonFile);
            TextColor = Brushes.Black;
            OutputLog = InputFilename + " dropped";
            using (StreamReader r = new StreamReader(jsonFile))
            {
                jsonFile = r.ReadToEnd();
            }
            try
            {
                var deserializedProfile = JsonConvert.DeserializeObject<Profile>(jsonFile, new JsonEdgeConverter());
                CreateEdgesAndVerticesDictionary(deserializedProfile);
                OutputLog = InputFilename + " has been processed";
            }
            catch (Exception)
            {
                //TODO Change this to allow the user to try again
                MessageBox.Show("Error. The profile violates the Schema!");
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Creates a dictionary of edges and a dictionary of vertices. The key is the id of the edge or vertex.
        /// </summary>
        /// <param name="profile"></param>
        private void CreateEdgesAndVerticesDictionary(Profile profile)
        {
            _edges = new Dictionary<int, Edge>();
            _vertices = new Dictionary<int, Vertex>();
            _lineSegments = new List<LineSegment>();
            _circularArcs = new List<CircularArc>();
            foreach (var vertex in profile.Vertices)
            {
                try
                {
                    _vertices.Add(vertex.id, vertex);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Error. More than one vertex has the same id: " + vertex.id);
                }
            }
            foreach (var edge in profile.Edges)
            {
                try
                {
                    if (edge.GetType() == typeof (LineSegment))
                    {
                        Vertex vertex1, vertex2;
                        if (edge.Vertices.Count < 2 || !_vertices.TryGetValue(edge.Vertices[0], out vertex1) ||
                            !_vertices.TryGetValue(edge.Vertices[1], out vertex2))
                        {
                            MessageBox.Show("Error!");
                            return;
                        }
                        var lineSegment = new LineSegment
                        {
                            id = edge.id,
                            Type = edge.Type,
                            Vertices = edge.Vertices,
                            Vector2D = GeometryUtilities.ConstructVector(vertex1, vertex2)
                        };
                        lineSegment.Magnitude = GeometryUtilities.GetVectorMagnitude(lineSegment.Vector2D);
                        _edges.Add(edge.id, lineSegment);
                        //TODO probably we don't need this extra space. Revisit this
                        _lineSegments.Add(lineSegment);
                    }
                    else if (edge.GetType() == typeof (CircularArc))
                    {
                        var arc = edge as CircularArc;
                        if (arc == null)
                        {
                            return;
                        }
                        Vertex vertex1, vertex2, fromVertex, toVertex;
                        if (edge.Vertices.Count < 2 || !_vertices.TryGetValue(edge.Vertices[0], out vertex1) ||
                            !_vertices.TryGetValue(edge.Vertices[1], out vertex2) ||
                            !_vertices.TryGetValue(arc.ClockwiseFrom, out fromVertex))
                        {
                            MessageBox.Show("Error!");
                            return;
                        }
                        var circularArc = new CircularArc
                        {
                            id = edge.id,
                            Type = edge.Type,
                            Vertices = edge.Vertices,
                            Center = arc.Center,
                            ClockwiseFrom = arc.ClockwiseFrom,
                            ClockwiseTo = edge.Vertices[0] == arc.ClockwiseFrom ? edge.Vertices[1] : edge.Vertices[0],
                            Radius = GeometryUtilities.GetRadiusFromCircularArc(arc.Center, fromVertex)
                        };
                        _vertices.TryGetValue(circularArc.ClockwiseTo, out toVertex);
                        circularArc.Length = GeometryUtilities.GetPerimeterFromCircularArc(circularArc.Radius, fromVertex, toVertex);
                        _edges.Add(edge.id, circularArc);
                        //TODO probably we don't need this extra space. Revisit this
                        _circularArcs.Add(circularArc);
                    }
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Error. More than one edge has the same id: " + edge.id);
                }
            }
            CalculateCost();
        }

        #endregion


        #region Cost Calculation

        /// <summary>
        /// Function to calculate the cost
        /// </summary>
        private void CalculateCost()
        {
            var totalTime = 0.0;
            double leftX, rightX, bottomY, topY;
            if (_lineSegments.Count >= 1)
            {
                // Find left and right boundaries of the line segments
                FindBoundaries(out leftX, out rightX, out topY, out bottomY);
                foreach (var arc in _circularArcs)
                {
                    Vertex fromVertex, toVertex;
                    _vertices.TryGetValue(arc.ClockwiseFrom, out fromVertex);
                    _vertices.TryGetValue(arc.ClockwiseTo, out toVertex);
                }
                //Check if the profile has any circular Arcs
                if (_circularArcs.Count == 0)
                {
                    //There are no circular arcs so we just need to add the padding to one side of the edges
                    //Calculate the perimeter of the shape
                    var perimeter = 0.0;
                    foreach (var lineSegment in _lineSegments)
                    {
                        perimeter += lineSegment.Magnitude;
                    }

                    //Get the machine cost
                    totalTime = CostUtilities.GetRequiredTime(PlethoraDefinitions.MaxLaserSpeed, perimeter);
                }
                else if (_circularArcs.Count >= 1)
                {
                    foreach (var arc in _circularArcs)
                    {
                        //Check if the arc's from or to Vertex is at the boundaries
                        Vertex fromVertex, toVertex;
                        _vertices.TryGetValue(arc.ClockwiseFrom, out fromVertex);
                        _vertices.TryGetValue(arc.ClockwiseTo, out toVertex);
                        CheckBoundaries(ref leftX, ref rightX, ref topY, ref bottomY, fromVertex, toVertex, arc.Center, arc.Radius);
                    }
                    //Calculate the perimeter of the shape
                    var straightLength = 0.0;
                    foreach (var lineSegment in _lineSegments)
                    {
                        straightLength += lineSegment.Magnitude;
                    }
                    
                    foreach (var arc in _circularArcs)
                    {
                        var speed = CostUtilities.GetLaserSpeedForArc(arc.Radius);
                        var time = CostUtilities.GetRequiredTime(speed, arc.Length);
                        totalTime += time;
                    }
                    //Get the machine cost
                    var straightTime = CostUtilities.GetRequiredTime(PlethoraDefinitions.MaxLaserSpeed, straightLength);
                    totalTime += straightTime;
                }
                var width = rightX - leftX + PlethoraDefinitions.Padding;
                var height = topY - bottomY + PlethoraDefinitions.Padding;
                var area = GeometryUtilities.GetAreaRectangularArea(width, height);
                var materialCost = CostUtilities.GetMaterialCost(area);
                var machineCost = CostUtilities.GetMachineTimeCost(totalTime);
                MaterialCost = "$" + materialCost.ToString(CultureInfo.InvariantCulture);
                MachineCost = "$" + machineCost.ToString(CultureInfo.InvariantCulture);
                var totalCost = Math.Round(machineCost + materialCost, 2);
                TotalCost = "$" + totalCost.ToString(CultureInfo.InvariantCulture);
            }
            //TODO check if this scenario will work
            if (_lineSegments.Count == 1 && _circularArcs.Count > 0)
            {
                
            }
        }

        /// <summary>
        /// Finds the left and right boundaries of the shape
        /// </summary>
        /// <param name="leftX"></param>
        /// <param name="rightX"></param>
        /// <param name="topY"></param>
        /// <param name="bottomY"></param>
        private void FindBoundaries(out double leftX, out double rightX, out double topY, out double bottomY)
        {
            leftX = double.MaxValue;
            rightX = double.MinValue;
            topY = double.MinValue;
            bottomY = double.MaxValue;
            foreach (var vertex in _vertices)
            {
                if (vertex.Value.Position.X < leftX)
                {
                    leftX = vertex.Value.Position.X;
                }
                if (vertex.Value.Position.X > rightX)
                {
                    rightX = vertex.Value.Position.X;
                }
                if (vertex.Value.Position.Y < bottomY)
                {
                    bottomY = vertex.Value.Position.Y;
                }

                if (vertex.Value.Position.Y > topY)
                {
                    topY = vertex.Value.Position.Y;
                }
            }
        }
        
        /// <summary>
        /// Check if the boundaries should be updated according to the arcs
        /// </summary>
        /// <param name="leftX"></param>
        /// <param name="rightX"></param>
        /// <param name="topY"></param>
        /// <param name="bottomY"></param>
        /// <param name="fromVertex"></param>
        /// <param name="toVertex"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        private void CheckBoundaries(ref double leftX, ref double rightX, ref double topY, ref double bottomY, Vertex fromVertex,
            Vertex toVertex, Point center, double radius)
        {
            var intersection = GeometryUtilities.FindCircleIntersection(center, radius, fromVertex, toVertex);
            if (intersection.X < leftX)
            {
                leftX = intersection.X;
            }
            else if (intersection.X > rightX)
            {
                rightX = intersection.X;
            }
            if (intersection.Y > topY)
            {
                topY = intersection.Y;
            }
            else if (intersection.Y < bottomY)
            {
                bottomY = intersection.Y;
            }
        }

        #endregion
    }
}

