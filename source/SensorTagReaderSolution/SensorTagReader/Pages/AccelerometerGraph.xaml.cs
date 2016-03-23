using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Tools;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System.Threading.Tasks;
using System.Diagnostics;
using X2CodingLab.SensorTag.Sensors;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SensorTagReader.Controls
{
    public sealed partial class AccelerometerGraph : UserControl
    {
        private bool isInitialized;

        private Random _random = new Random();

        private EventThrottler _updateThrottler = new EventThrottler();

        private List<NameValueItem> _xItems = new List<NameValueItem>();
        private List<NameValueItem> _yItems = new List<NameValueItem>();
        private List<NameValueItem> _zItems = new List<NameValueItem>();

        

        //public String MyLabel
        //{
        //    get { return (String)GetValue(MyLabelProperty); }
        //    set { SetValue(MyLabelProperty, value); }
        //}

        //public static readonly DependencyProperty MyLabelProperty =
        //     DependencyProperty.Register("MyLabel", typeof(string),
        //        typeof(AccelerometerGraph), new PropertyMetadata(""));

        public MovementMeasurementValue movementMeasurementValue
        {
            set {
                    UpdateCharts(value);
                    //UpdateCharts_Test();
                }   
        }

        public static readonly DependencyProperty movementMeasurementValueProperty =
             DependencyProperty.Register("", typeof(MovementMeasurementValue),
                typeof(AccelerometerGraph), new PropertyMetadata(""));


        public AccelerometerGraph()
        {
            this.InitializeComponent();
            this.isInitialized = true;
            this.DataContext = this;

            // add the y z and lines
            //LineSeries y = new LineSeries();
            //LineSeries z = new LineSeries();
            //y.DependentValuePath = "Value";
            //y.IndependentValuePath = "Name";
            //LineChartWithAxes.Series.Add(y);
            //z.DependentValuePath = "Value";
            //z.IndependentValuePath = "Name";
            //LineChartWithAxes.Series.Add(z);
        }

        private void UpdateCharts(MovementMeasurementValue m)
        {
            if (!this.isInitialized)
            {
                return;
            }


            _xItems.Add(new NameValueItem { Name = _xItems.Count().ToString(), Value = m.Value.AccelX * 10 });
            _yItems.Add(new NameValueItem { Name = _yItems.Count().ToString(), Value = m.Value.AccelY * 10 });
            _zItems.Add(new NameValueItem { Name = _zItems.Count().ToString(), Value = m.Value.AccelZ * 10 });

            int start;
            int end;

            if (_xItems.Count() > 15)
            {
                start = _xItems.Count() - 15;
                end = _xItems.Count()-1;
            }
            else
            {
                start = 0;
                end = _xItems.Count();
            }

            List<NameValueItem> itemsX = new List<NameValueItem>();
            List<NameValueItem> itemsY = new List<NameValueItem>();
            List<NameValueItem> itemsZ = new List<NameValueItem>();
            for (int i = start; i < end; i++)
            {
                itemsX.Add(_xItems[i]);
                itemsY.Add(_yItems[i]);
                itemsZ.Add(_zItems[i]);
            }

            double maxAccel = itemsX.Max(t => t.Value);
            double minAccel = itemsX.Min(t => t.Value);
            double max = Math.Max(maxAccel, 1);
            double min = Math.Min(maxAccel, -1);

            ((LineSeries)LineChartWithAxes.Series[0]).ItemsSource = itemsX;
            ((LineSeries)LineChartWithAxes.Series[1]).ItemsSource = itemsY;
            ((LineSeries)LineChartWithAxes.Series[2]).ItemsSource = itemsZ;

            // setting up X values
            ((LineSeries)LineChartWithAxes.Series[0]).DependentRangeAxis =
                new LinearAxis
                {
                    Minimum = min,
                    Maximum = max,
                    Orientation = AxisOrientation.Y,
                    Interval = .05,
                    ShowGridLines = true
                };

            // setting up Y values
            ((LineSeries)LineChartWithAxes.Series[1]).DependentRangeAxis =
                new LinearAxis
                {
                    ,
                    Visibility = Visibility.Collapsed,
                    Minimum = min,
                    Maximum = max,
                    Orientation = AxisOrientation.Y,
                    Interval = .05,
                    ShowGridLines = true
                };

            // setting up Z values
            ((LineSeries)LineChartWithAxes.Series[2]).DependentRangeAxis =
                new LinearAxis
                {
                    Visibility = Visibility.Collapsed,
                    Minimum = min,
                    Maximum = max,
                    Orientation = AxisOrientation.Y,
                    Interval = .05,
                    ShowGridLines = true
                };
        }


        private void UpdateCharts_Test()
        {
            if (!this.isInitialized)
            {
                return;
            }

            List<NameValueItem> xItems = new List<NameValueItem>();

            for (int i = 0; i < 20; i++)
            {
                double _value = System.Convert.ToDouble(_random.Next(10, 100)) / 100;
                xItems.Add(new NameValueItem { Name = "X" + i, Value = _value});
            }


            ((LineSeries)LineChartWithAxes.Series[0]).ItemsSource = xItems;
            ((LineSeries)LineChartWithAxes.Series[0]).DependentRangeAxis =
                new LinearAxis
                {
                    Minimum = 0,
                    Maximum = 1,
                    Orientation = AxisOrientation.Y,
                    Interval = .04,
                    ShowGridLines = true
                };
        }

        public class NameValueItem
        {
            public string Name { get; set; }
            public double Value { get; set; }
        }

        public class MovementMeasurementValue
        {
            public string Name { get; set; }
            public Movement.MovementMeasurement Value { get; set; }
        }

        private void NumberOfIitemsNumericUpDown_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            this.ThrottledUpdate();
        }

        private void ChartsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ThrottledUpdate();
        }

        private void OnUpdateButtonClick(object sender, RoutedEventArgs e)
        {
            this.ThrottledUpdate();
        }

        private void ThrottledUpdate()
        {
            _updateThrottler.Run(
                async () =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    this.UpdateCharts_Test();
                    sw.Stop();
                    await Task.Delay(sw.Elapsed);
                });
        }
    }
}
