using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Newtonsoft.Json;
using PlethoraModels;
using Prism.Commands;
using Prism.Mvvm;
using LineSegment = PlethoraModels.LineSegment;

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

        private Dictionary<int, Edge> _edges;
        private Dictionary<int, LineSegment> _lineSegments;
        private Dictionary<int, CircularArc> _circularArc;
        private Dictionary<int, Vertex> _vertices; 
        
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
            foreach (var edge in profile.Edges)
            {
                try
                {
                    _edges.Add(edge.id, edge);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Error. More than one edge has the same id: " + edge.id);
                }
                
            }
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
        }

        #endregion
    }
}

