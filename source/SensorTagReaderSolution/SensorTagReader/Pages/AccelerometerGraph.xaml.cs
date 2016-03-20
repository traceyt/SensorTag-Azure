using System;
using System.Collections.Generic;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SensorTagReader.Pages
{
    public sealed partial class AccelerometerGraph : UserControl
    {
        private bool isInitialized;

        private Random _random = new Random();

        private EventThrottler _updateThrottler = new EventThrottler();

        private List<NameValueItem> items;

        public AccelerometerGraph()
        {
            this.InitializeComponent();
            this.isInitialized = true;

        }

        private void UpdateCharts()
        {
            if (!this.isInitialized)
            {
                return;
            }


            items = new List<NameValueItem>();

            for (int i = 0; i < NumberOfIitemsNumericUpDown.Value; i++)
            {
                double _value = System.Convert.ToDouble(_random.Next(10, 100)) / 100;
                items.Add(new NameValueItem { Name = "X" + i, Value = _value});
            }


            ((LineSeries)LineChartWithAxes.Series[0]).ItemsSource = items;
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
                    this.UpdateCharts();
                    sw.Stop();
                    await Task.Delay(sw.Elapsed);
                });
        }
    }
}
