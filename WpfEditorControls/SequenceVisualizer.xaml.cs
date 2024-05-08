using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfEditorControls
{
    /// <summary>
    /// Interaction logic for SequenceVisualizer.xaml
    /// </summary>
    public partial class SequenceVisualizer : UserControl
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(SequenceVisualizer), new FrameworkPropertyMetadata(Colors.Red));
        
        public Color Color
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(SequenceVisualizer), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnLabelChanged)));
        
        private static void OnLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SequenceVisualizer? sequenceVisualizer = o as SequenceVisualizer;
            if (sequenceVisualizer != null)
                sequenceVisualizer.OnLabelChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnLabelChanged(string oldValue, string newValue)
        {
            tbSequenceLabel.Text = newValue;
        }

        public string Label
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(LabelProperty);
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }
        public SequenceVisualizer()
        {
            InitializeComponent();
        }

        public void SetVerticalBounds(double lowerBound, double upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
            VerticalRange = upperBound - lowerBound;
            if (VerticalRange == 0)
                VerticalRange = 1;
        }

        public void VisualizeData<T>(List<T>? data, Func<T, double> getPosition, GraphStyle graphStyle)
        {
            cvsSequence.Children.Clear();

            if (data == null || data.Count == 0)
                return;

            double lowerBound = data.Select(x => getPosition(x)).Min();
            double upperBound = data.Select(x => getPosition(x)).Max();
            SetVerticalBounds(lowerBound, upperBound);

            double widthOfTimelineFrame = cvsSequence.ActualWidth / data.Count;
            double xPos = 0;
            foreach (T element in data)
            {
                double position = getPosition(element);
                AddToTimeline(position, xPos, widthOfTimelineFrame, graphStyle);
                xPos += widthOfTimelineFrame;
            }
            AddBoundLabels();
        }

        public void VisualizeData<T>(List<T>? data, Func<T, Color> getColor)
        {
            cvsSequence.Children.Clear();

            // Set the background to black..
            cvsSequence.Background = Brushes.Black;

            if (data == null || data.Count == 0)
                return;

            double widthOfTimelineFrame = cvsSequence.ActualWidth / data.Count;
            double xPos = 0;
            foreach (T element in data)
            {
                Color color = getColor(element);
                AddToTimeline(xPos, widthOfTimelineFrame, color);
                xPos += widthOfTimelineFrame;
            }
        }

        public enum LabelPosition
        {
            Top,
            Bottom,
        }

        void AddNumberLabel(double x, double y, double value, LabelPosition labelPosition)
        {
            TextBlock textBlock = new TextBlock
            {
                Text = value.ToString(),
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 12,
                Background = new SolidColorBrush(Colors.White) { Opacity = 0.75 },
                Padding = new Thickness(3, 0, 3, 0),
            };
            cvsSequence.Children.Add(textBlock);
            if (labelPosition == LabelPosition.Bottom)
                y -= 16;
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
        }

        void AddTopScaleLabel(double value)
        {
            AddNumberLabel(0, 0, value, LabelPosition.Top);
        }

        void AddBottomScaleLabel(double value)
        {
            AddNumberLabel(0, cvsSequence.ActualHeight, value, LabelPosition.Bottom);
        }
        void AddBoundLabels()
        {
            AddTopScaleLabel(UpperBound);
            AddBottomScaleLabel(LowerBound);
        }

        void AddBoxFromAbove(double yPosition, double xPos, double widthOfTimelineFrame)
        {
            Rectangle rectangle = AddBar(xPos, yPosition, widthOfTimelineFrame);
            rectangle.Fill = new SolidColorBrush(Color);

            Canvas.SetTop(rectangle, 0);
        }

        void AddBoxFromBelow(double yPosition, double xPos, double widthOfTimelineFrame)
        {
            Rectangle rectangle = AddBar(xPos, yPosition, widthOfTimelineFrame);
            rectangle.Fill = new SolidColorBrush(Color);

            Canvas.SetTop(rectangle, cvsSequence.ActualHeight - rectangle.Height);
        }

        private Rectangle AddBar(double xPos, double yPosition, double widthOfTimelineFrame)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = widthOfTimelineFrame;
            rectangle.Height = cvsSequence.ActualHeight * (yPosition - LowerBound) / VerticalRange;
            if (rectangle.Height < 2)
                rectangle.Height = 2;
            cvsSequence.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, xPos);
            return rectangle;
        }

        void AddSmallSquares(double position, double xPos, double widthOfTimelineFrame)
        {
            
        }

        void AddToTimeline(double xPos, double widthOfTimelineFrame, Color color)
        {
            AddBar(xPos, UpperBound, widthOfTimelineFrame).Fill = new SolidColorBrush(color);
        }

        void AddToTimeline(double position, double xPos, double widthOfTimelineFrame, GraphStyle graphStyle)
        {
            switch (graphStyle)
            {
                case GraphStyle.BoxFromAbove:
                    AddBoxFromAbove(position, xPos, widthOfTimelineFrame);
                    break;
                case GraphStyle.BoxFromBelow:
                    AddBoxFromBelow(position, xPos, widthOfTimelineFrame);
                    break;
                case GraphStyle.SmallSquares:
                    AddSmallSquares(position, xPos, widthOfTimelineFrame);
                    break;
            }
        }
        public void SetLabel(string label)
        {
            tbSequenceLabel.Text = label;
        }

        public double UpperBound { get; set; } = 1;
        public double LowerBound { get; set; } = 0;
        public double VerticalRange { get; set; } = 1;
    }
}
