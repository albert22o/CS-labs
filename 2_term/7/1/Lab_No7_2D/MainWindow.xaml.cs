using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Lab_No7_2D
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Rectangle _victoriaSprite = new Rectangle();
		private readonly Polygon _polygon = new Polygon();
		//private readonly Rectangle myRect = new Rectangle();
		private readonly Path path = new Path();
		private Ellipse _ellipse;
		//private readonly Line myLine4;
		//private readonly Line myLine5;
		//private readonly Line myLine6;

		private bool _isEllipseMovable;
		private int _currentFrame = 0;
		private readonly int _frameCount = 56;
		private readonly int _frameW = 100;
		private readonly int _frameH = 100;

		private readonly DispatcherTimer _firstTimer = new DispatcherTimer();
		private readonly DispatcherTimer _secondTimer = new DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();

			_firstTimer.Interval = new TimeSpan(0, 0, 0, 0, 120);
			_firstTimer.Tick += new EventHandler(FirstTimer_Tick);

			_secondTimer.Interval = new TimeSpan(0, 0, 1);
			_secondTimer.Tick += new EventHandler(SecondTimer_Tick);
			_secondTimer.Start();

			x.Content = "X: ";
			y.Content = "Y: ";

			DrawSun();
			DrawLine();
			DrawVictoriaSprites();
			DrawEllipse();
			DrawManyrin4EVER();
			DrawInitialBefore();
			DrawClocks();
		}

		#region DrawMethods
		private void DrawSun()
		{
			Ellipse sunEllipse = new Ellipse();
			SolidColorBrush mySolidColorBrush = new SolidColorBrush
			{
				Color = Color.FromArgb(255, 215, 0, 0)
			};
			sunEllipse.Fill = mySolidColorBrush;
			sunEllipse.StrokeThickness = 2;
			sunEllipse.Stroke = Brushes.Black;
			sunEllipse.Width = 100;
			sunEllipse.Height = 100;
			sunEllipse.Margin = new Thickness(-50, 0, 0, 0);
			scene.Children.Add(sunEllipse);
		}

		private void DrawLine()
		{
			Line straightLine = new Line
			{
				Stroke = Brushes.OrangeRed,
				X1 = 500, Y1 = 300,
				X2 = 600, Y2 = 250,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				StrokeThickness = 2
			};
			scene.Children.Add(straightLine);
		}

		private void DrawVictoriaSprites()
		{
			ImageBrush ib1 = new ImageBrush();

			_victoriaSprite.Height = 100;
			_victoriaSprite.Width = 100;

			ib1.AlignmentX = AlignmentX.Left;
			ib1.AlignmentY = AlignmentY.Top;
			ib1.Stretch = Stretch.None;
			ib1.Viewbox = new Rect(0, 0, 100, 100); 
			ib1.ViewboxUnits = BrushMappingMode.Absolute;
			ib1.ImageSource = new BitmapImage(new Uri(@"C:/Users/Nekitt/Documents/Учеба/ПиОГИ/Lab_No7_2D/images/VictoriaSprites.gif", UriKind.Absolute));

			_victoriaSprite.Fill = ib1;
			_victoriaSprite.RenderTransform = new TranslateTransform(267, 206);

			scene.Children.Add(_victoriaSprite);
			_firstTimer.Start();
		}

		private void DrawEllipse()
		{
			_ellipse = new Ellipse
			{
				StrokeThickness = 2,
				Stroke = Brushes.LightSeaGreen,
				Fill = Brushes.LightSeaGreen,
				Width = 100,
				Height = 100
			};

			scene.Children.Add(_ellipse);
			_ellipse.RenderTransform = new TranslateTransform(550, 100);
			_ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
			_ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
		}

		private void DrawManyrin4EVER()
		{
			ImageBrush ib = new ImageBrush();
			path.StrokeThickness = 2;
			path.Stroke = Brushes.MediumVioletRed;
			path.Fill = ib;
			ib.ImageSource = new BitmapImage(new Uri(@"C:/Users/Nekitt/Documents/Учеба/ПиОГИ/Lab_No7_2D/images/back.jpg", UriKind.Absolute));

			BezierSegment bezierCurve1 =
				new BezierSegment(new Point(450, 0), new Point(450, 50), new Point(500, 90), true);
			BezierSegment bezierCurve2 =
				new BezierSegment(new Point(550, 50), new Point(550, 0), new Point(500, 30), true);

			PathSegmentCollection psc = new PathSegmentCollection { bezierCurve1, bezierCurve2 };
			PathFigure pf = new PathFigure { Segments = psc, StartPoint = new Point(500, 30) };
			PathFigureCollection pfc = new PathFigureCollection { pf };
			PathGeometry pg = new PathGeometry { Figures = pfc };
			GeometryGroup pathGeometryGroup = new GeometryGroup();
			pathGeometryGroup.Children.Add(pg);
			path.Data = pathGeometryGroup;

			scene.Children.Add(path);
		}

		private void DrawInitialBefore()
		{
			ImageBrush ib7 = new ImageBrush
			{
				ImageSource = new BitmapImage(new Uri(@"C:/Users/Nekitt/Documents/Учеба/ПиОГИ/Lab_No7_2D/images/before.jpg", UriKind.Absolute))
			};
			_polygon.Fill = ib7;
			_polygon.Stroke = Brushes.Yellow;
			_polygon.StrokeThickness = 2;
			_polygon.HorizontalAlignment = HorizontalAlignment.Left;
			_polygon.VerticalAlignment = VerticalAlignment.Center;

			Point Point1 = new Point(0, 0);
			Point Point2 = new Point(100, 0);
			Point Point3 = new Point(150, 75);
			Point Point4 = new Point(50, 150);
			Point Point5 = new Point(-50, 75);

			PointCollection myPointCollection = new PointCollection
			{
				Point1,
				Point2,
				Point3,
				Point4,
				Point5
			};
			_polygon.Points = myPointCollection;

			scene.Children.Add(_polygon);

			_polygon.MouseEnter += Polygon_MouseEnter;
			_polygon.MouseLeave += Polygon_MouseLeave;
			_polygon.MouseLeftButtonDown += Polygon_MouseLeftButtonDown;
			_polygon.MouseLeftButtonUp += Polygon_MouseLeftButtonUp;
			_polygon.RenderTransform = new TranslateTransform(50, 175);
		}

		private void DrawClocks()
		{
			Ellipse myEllipse3 = new Ellipse();
			ImageBrush ib3 = new ImageBrush();

			myEllipse3.StrokeThickness = 5;
			myEllipse3.Stroke = Brushes.SaddleBrown;
			myEllipse3.Fill = ib3;
			ib3.ImageSource = new BitmapImage(new Uri(@"C:/Users/Nekitt/Documents/Учеба/ПиОГИ/Lab_No7_2D/images/clocks.jpg", UriKind.Absolute));
			myEllipse3.Width = 200;
			myEllipse3.Height = 200;
			myEllipse3.Margin = new Thickness(170, 0, 0, 0);

			scene.Children.Add(myEllipse3);
		}
#endregion

		#region EventHandlers
		private void FirstTimer_Tick(object sender, EventArgs e)
		{
			_currentFrame = (_currentFrame + 1 + _frameCount) % _frameCount;
			var frameLeft = _currentFrame % 7 * _frameW;
			var frameTop = _currentFrame / 7 * _frameH;
			(_victoriaSprite.Fill as ImageBrush).Viewbox = new Rect(frameLeft, frameTop, frameLeft + _frameW, frameTop + _frameH);
		}

		private void SecondTimer_Tick(object sender, EventArgs e)
		{
			RotateSecond.Angle = 6 * (DateTime.Now.Second);
			RotateMinute.Angle = 6 * (DateTime.Now.Minute);
			RotateHour.Angle = 30 * (DateTime.Now.Hour);
		}

		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			Point position = Mouse.GetPosition(scene);
			x.Content = "X: " + position.X;
			y.Content = "Y: " + position.Y;
		}

		private void Ellipse_MouseLeftButtonDown(object sender, MouseEventArgs e)
		{
			_isEllipseMovable = true;
		}

		private void Ellipse_MouseLeftButtonUp(object sender, MouseEventArgs e)
		{
			_isEllipseMovable = false;
		}

		private void Polygon_MouseEnter(object sender, MouseEventArgs e)
		{
			ImageBrush ib = new ImageBrush
			{
				ImageSource = new BitmapImage(new Uri(@"C:/Users/Nekitt/Documents/Учеба/ПиОГИ/Lab_No7_2D/images/after.jpg", UriKind.Absolute))
			};
			_polygon.Fill = ib;
			_polygon.Stroke = Brushes.Gray;
			_polygon.StrokeThickness = 2;
		}

		private void Polygon_MouseLeave(object sender, MouseEventArgs e)
		{
			ImageBrush ib = new ImageBrush
			{
				ImageSource = new BitmapImage(new Uri(@"C:/Users/Nekitt/Documents/Учеба/ПиОГИ/Lab_No7_2D/images/before.jpg", UriKind.Absolute))
			};
			_polygon.Fill = ib;
			_polygon.Stroke = Brushes.Yellow;
			_polygon.StrokeThickness = 2;
		}

		private void Polygon_MouseLeftButtonDown(object sender, MouseEventArgs e)
		{
			_polygon.Margin = new Thickness(0, 0, 200, 200);
			_polygon.RenderTransform = new ScaleTransform(3, 3);
		}

		private void Polygon_MouseLeftButtonUp(object sender, MouseEventArgs e)
		{
			_polygon.Margin = new Thickness(50, 175, 60, 175);
			_polygon.RenderTransform = new ScaleTransform(1, 1);
		}

		private void Scene_MouseMove(object sender, MouseEventArgs e)
		{
			Point position = Mouse.GetPosition(scene);
			Rect rect = path.RenderTransform.TransformBounds(path.RenderedGeometry.Bounds);
			Rect ellipse = _ellipse.RenderTransform.TransformBounds(_ellipse.RenderedGeometry.Bounds);

			if (_isEllipseMovable == true)
				_ellipse.RenderTransform = new TranslateTransform(position.X - _ellipse.Width / 2, position.Y - _ellipse.Height / 2);

			if (rect.Contains(position) && rect.IntersectsWith(ellipse))
				MessageBox.Show("Фигуры пересекаются!");
		}
		#endregion
	}
}