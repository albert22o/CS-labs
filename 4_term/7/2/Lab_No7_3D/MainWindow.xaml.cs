using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using Microsoft.Win32;

namespace Lab_No7_3D
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const int TERRAIN_SIZE = 256;

		private readonly ModelVisual3D _terrain;
		private readonly PerspectiveCamera _viewportCamera;

		private double _angle = default;

		public MainWindow()
		{
			_terrain = new ModelVisual3D();
			_viewportCamera = new PerspectiveCamera();

			InitializeComponent();

			Grid.Background = System.Windows.Media.Brushes.LightGray;

			GenerateTerrain();
			RenderTerrain();
		}

		private void GenerateTerrain()
		{
			OpenFileDialog dlg = new OpenFileDialog()
			{
				InitialDirectory = @"C:\Users\Nekitt\Documents\Учеба\ПиОГИ\Lab_No7_3D",
				RestoreDirectory = true
			};
			dlg.ShowDialog();
			Bitmap bitmap = new Bitmap(dlg.FileName);

			MeshGeometry3D geometry = new MeshGeometry3D();
			for (int i = default; i < TERRAIN_SIZE; ++i)
				for (int j = default; j < TERRAIN_SIZE; ++j)
				{
					double y = bitmap.GetPixel(i, j).R / 10.0;
					geometry.Positions.Add(new Point3D(i, y, j));
					double tu = i / Convert.ToDouble(TERRAIN_SIZE);
					double tv = j / Convert.ToDouble(TERRAIN_SIZE);
					geometry.TextureCoordinates.Add(new System.Windows.Point(tu, tv));
				}

			for (int i = default; i < TERRAIN_SIZE - 1; ++i)
				for (int j = default; j < TERRAIN_SIZE - 1; ++j)
				{
					int ind0 = i + j * TERRAIN_SIZE;
					int ind1 = (i + 1) + j * TERRAIN_SIZE;
					int ind2 = i + (j + 1) * TERRAIN_SIZE;
					int ind3 = (i + 1) + (j + 1) * TERRAIN_SIZE;
					geometry.TriangleIndices.Add(ind0);
					geometry.TriangleIndices.Add(ind1);
					geometry.TriangleIndices.Add(ind3);
					geometry.TriangleIndices.Add(ind0);
					geometry.TriangleIndices.Add(ind3);
					geometry.TriangleIndices.Add(ind2);
				}

			ImageBrush ib = new ImageBrush
			{
				ImageSource = new BitmapImage(new Uri(@"C:\Users\Nekitt\Documents\Учеба\ПиОГИ\Lab_No7_3D\textures\sand-texture.jpg", UriKind.Absolute)),
				Transform = new ScaleTransform(0.5, 0.5),
				TileMode = TileMode.Tile,
				Stretch = Stretch.Fill
			};

			DiffuseMaterial material = new DiffuseMaterial(ib);
			GeometryModel3D model = new GeometryModel3D(geometry, material);
			_terrain.Content = model;

			Scene.Children.Add(_terrain);
		}

		private void RenderTerrain()
		{
			_viewportCamera.Position = new Point3D(TERRAIN_SIZE / 2, TERRAIN_SIZE / 2, TERRAIN_SIZE * 1.5);
			Vector3D lookAt = new Vector3D(TERRAIN_SIZE / 2, 0, TERRAIN_SIZE / 2);
			_viewportCamera.LookDirection = Vector3D.Subtract(lookAt, new Vector3D(TERRAIN_SIZE / 2, TERRAIN_SIZE / 2, TERRAIN_SIZE * 2));
			_viewportCamera.FarPlaneDistance = 1000;
			_viewportCamera.NearPlaneDistance = 1;
			_viewportCamera.UpDirection = new Vector3D(0, 1, 0);
			_viewportCamera.FieldOfView = 75;
			Scene.Camera = _viewportCamera;
			PointLight lightPoint = new PointLight
			{
				Color = Colors.LightYellow,
				Position = new Point3D(TERRAIN_SIZE, TERRAIN_SIZE / 2, TERRAIN_SIZE / 2)
			};
			ModelVisual3D light = new ModelVisual3D { Content = lightPoint };
			Scene.Children.Add(light);
		}

		private void Window_Key_Down(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Left)
				_angle--;

			if (e.Key == Key.Right)
				_angle++;

			AxisAngleRotation3D ax3d = new AxisAngleRotation3D(new Vector3D(0, 1, 0), _angle);
			RotateTransform3D rt = new RotateTransform3D(ax3d);
			TranslateTransform3D tr1 = new TranslateTransform3D(-TERRAIN_SIZE / 2, 0, -TERRAIN_SIZE / 2);
			TranslateTransform3D tr2 = new TranslateTransform3D(TERRAIN_SIZE / 2, 0, TERRAIN_SIZE / 2);
			Transform3DGroup tg = new Transform3DGroup();

			tg.Children.Add(tr1);
			tg.Children.Add(rt);
			tg.Children.Add(tr2);

			_terrain.Transform = tg;
		}
	}
}