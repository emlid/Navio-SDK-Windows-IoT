using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models.Tests;
using System;
using System.ComponentModel;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Views.Tests
{
    /// <summary>
    /// Barometer test page.
    /// </summary>
    public sealed partial class BarometerTestPage : BarometerTestPageBase
    {
        #region Constants

        /// <summary>
        /// Spacing between data points on the <see cref="Graph"/>, creating a zoom effect.
        /// </summary>
        public const int GraphZoom = 4;

        /// <summary>
        /// Padding between elements of the graph.
        /// </summary>
        public const int GraphPadding = 8;

        #endregion

        #region Lifetime

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public BarometerTestPage()
        {
            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the page model when it is displayed.
        /// </summary>
        protected override BarometerTestUIModel CreateModel(TestApplicationUIModel application)
        {
            return new BarometerTestUIModel(application);
        }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Call base class method
            base.OnNavigatedTo(arguments);

            // Hook events
            Model.PropertyChanged += OnModelChanged;

            // Update bindings
            Bindings.Update();

            // Initial layout
            UpdateLayout();
        }

        /// <summary>
        /// Updates view elements when the model changes and no automatic
        /// method is currently available.
        /// </summary>
        private void OnModelChanged(object sender, PropertyChangedEventArgs arguments)
        {
            switch (arguments.PropertyName)
            {
                case nameof(Model.Graph):
                    DrawGraph();
                    break;

                case nameof(Model.Output):
                    OutputScroller.UpdateLayout();
                    OutputScroller.ChangeView(null, OutputScroller.ScrollableHeight, null);
                    break;
            }
        }

        /// <summary>
        /// Executes the <see cref="BarometerTestUIModel.Reset"/> action when the related button is clicked.
        /// </summary>
        private void OnResetButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Reset();
        }

        /// <summary>
        /// Executes the <see cref="BarometerTestUIModel.Update"/> when the related button is clicked.
        /// </summary>
        private void OnUpdateButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Update();
        }

        /// <summary>
        /// Executes the <see cref="TestUIModel.Clear"/> when the related button is clicked.
        /// </summary>
        private void OnClearButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Clear();
        }

        /// <summary>
        /// Returns to the previous page when the close button is clicked.
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.GoBack();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Clears existing content then calculates new UI elements of the <see cref="Graph"/>
        /// for the current <see cref="BarometerTestUIModel.Graph"/>.
        /// </summary>
        private void DrawGraph()
        {
            // Initialize
            Graph.Children.Clear();

            // Calculate range and average
            var count = Model.Graph.Count;
            if (count == 0)
            {
                // Nothing to draw
                return;
            }
            var pressureMin = (double?)null;
            var pressureMax = (double?)null;
            var temperatureMin = (double?)null;
            var temperatureMax = (double?)null;
            var pressureTotal = (double?)null;
            var temperatureTotal = (double?)null;
            foreach (var point in Model.Graph)
            {
                var pressure = point.Pressure;
                if (!pressureMax.HasValue || pressure > pressureMax) pressureMax = pressure;
                if (!pressureMin.HasValue || pressure < pressureMin) pressureMin = pressure;
                pressureTotal = (pressureTotal ?? 0) + pressure;

                var temperature = point.Temperature;
                if (!temperatureMax.HasValue || temperature > temperatureMax) temperatureMax = temperature;
                if (!temperatureMin.HasValue || temperature < temperatureMin) temperatureMin = temperature;
                temperatureTotal = (temperatureTotal ?? 0) + temperature;
            }
            var pressureRange = (pressureMax ?? 0) - (pressureMin ?? 0);
            var pressureAverage = pressureTotal / count;
            var temperatureRange = (temperatureMax ?? 0) - (temperatureMin ?? 0);
            var temperatureAverage = temperatureTotal / count;

            // Calculate metrics
            Graph.UpdateLayout();
            var height = Graph.ActualHeight;
            var drawHeight = height - (GraphPadding * 2);
            var width = Graph.ActualWidth;
            var drawWidth = width - (GraphPadding * 2);
            var graphYMax = GraphPadding + drawHeight;
            Func<double, double, double, double> calculateGraphY = (double value, double minimum, double range) =>
            {
                if (range > 0)
                {
                    // Relative within range
                    return graphYMax - (drawHeight * ((value - minimum) / range));
                }
                else
                {
                    // Middle when flat line
                    return graphYMax - (drawHeight / 2);
                }
            };

            // Get resources
            var pressureBrush = (SolidColorBrush)Resources["GraphPressureBrush"];
            var temperatureBrush = (SolidColorBrush)Resources["GraphTemperatureBrush"];

            // Draw pressure average lines
            var pressureAverageY = calculateGraphY(pressureAverage.Value, pressureMin.Value, pressureRange);
            Graph.Children.Add(new Line
            {
                Stroke = pressureBrush,
                StrokeThickness = 1,
                X1 = GraphPadding,
                Y1 = pressureAverageY,
                X2 = GraphPadding + drawWidth,
                Y2 = pressureAverageY
            });

            // Draw temperature average line
            var temperatureAverageY = calculateGraphY(temperatureAverage.Value, temperatureMin.Value, temperatureRange);
            Graph.Children.Add(new Line
            {
                Stroke = temperatureBrush,
                StrokeThickness = 1,
                X1 = GraphPadding,
                Y1 = temperatureAverageY,
                X2 = GraphPadding + drawWidth,
                Y2 = temperatureAverageY
            });

            // Plot graph points
            var graphX = GraphPadding;
            var pressureLine = new Polyline
            {
                Stroke = pressureBrush,
                StrokeThickness = 1
            };
            var temperatureLine = new Polyline
            {
                Stroke = temperatureBrush,
                StrokeThickness = 1
            };
            for (var index = Model.Graph.Count - 1; index >= 0; index--)
            {
                // Iterate backwards so we start with latest measurement
                var point = Model.Graph[index];

                // Calculate relative pressure point
                var pressureY = calculateGraphY(point.Pressure, pressureMin.Value, pressureRange);
                var pressurePoint = new Point(graphX, pressureY);
                pressureLine.Points.Add(pressurePoint);

                // Calculate relative temperature point
                var temperatureY = calculateGraphY(point.Temperature, temperatureMin.Value, temperatureRange);
                var temperaturePoint = new Point(graphX, temperatureY);
                temperatureLine.Points.Add(temperaturePoint);

                // Move X for next point...
                graphX += GraphZoom;
                if (graphX > width)
                {
                    // Stop when outside view
                    break;
                }
            }
            Graph.Children.Add(pressureLine);
            Graph.Children.Add(temperatureLine);

            // Draw average text
            var pressureAverageString = string.Format(CultureInfo.CurrentCulture,
                "Pressure: {0}mbar", pressureAverage.Value);
            var pressureAverageText = new TextBlock
            {
                Foreground = pressureBrush,
                Text = pressureAverageString
            };
            Graph.Children.Add(pressureAverageText);
            var temperatureAverageString = string.Format(CultureInfo.CurrentCulture,
                "Temperature: {0}°c", temperatureAverage.Value);
            var temperatureAverageText = new TextBlock
            {
                Foreground = temperatureBrush,
                Text = temperatureAverageString
            };
            Graph.Children.Add(temperatureAverageText);

            // Position average text so that it does not overlap
            pressureAverageText.UpdateLayout();
            temperatureAverageText.UpdateLayout();
            Canvas.SetTop(pressureAverageText, pressureAverageY
                - pressureAverageText.ActualHeight);
            Canvas.SetLeft(pressureAverageText, width
                - pressureAverageText.ActualWidth - GraphPadding
                - temperatureAverageText.ActualWidth - GraphPadding);
            Canvas.SetTop(temperatureAverageText, temperatureAverageY
                - temperatureAverageText.ActualHeight);
            Canvas.SetLeft(temperatureAverageText, width
                - temperatureAverageText.ActualWidth - GraphPadding);
        }

        #endregion
    }
}