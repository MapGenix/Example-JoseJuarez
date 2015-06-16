using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace DotSpatial2
{
	public partial class Form1 : Form
	{
		string filePath;

		[Export("Shell",typeof(ContainerControl))]
		private static ContainerControl Shell;
		public Form1()
		{
			CheckForIllegalCrossThreadCalls = false;
			InitializeComponent();
			if (DesignMode)
				return;
			Shell = this;
			appManager1.LoadExtensions();
		}

		private IMapFeatureLayer LoadVectorLayer(string filePath)
		{
			IFeatureSet fs = FeatureSet.Open(filePath);
			IMapFeatureLayer myLayer = appManager1.Map.Layers.Add(fs);
			appManager1.Map.ZoomToMaxExtent();
			return myLayer;
		}

		protected override void OnShown(EventArgs e)
		{
			filePath = @"C:\Users\josej_000\Desktop\pruebas y ejemplos\DotSpatial2\mapas\rasters\O44121a1.dem";

			#region OperationsWithVectors
			//OperationsWithPoints
			//filePath = @"C:\Users\josej_000\Desktop\pruebas y ejemplos\DotSpatial2\mapas\Utah\Ejercicios\Centroids of Municipalities.shp";
			//OperationsWithPoints(myLayer);

			//OperationsWithLines
			//filePath = @"C:\Users\josej_000\Desktop\pruebas y ejemplos\DotSpatial2\mapas\Utah\UDOTRoutes_LRS\UDOTRoutes_LRS.shp";
			//OperationsWithLines(myLayer);

			//OperationsWithPolygons
			//filePath = @"C:\Users\josej_000\Desktop\pruebas y ejemplos\DotSpatial2\mapas\Utah\Counties\Counties.shp";
			//OperationsWithPolygons(myLayer);

			//OperationsWithLabels
			//filePath = @"C:\Users\josej_000\Desktop\pruebas y ejemplos\DotSpatial2\mapas\Utah\Counties\Counties.shp";
			//OperationsWithLabels(myLayer);

			
			//var myLayer = LoadVectorLayer(filePath); 

			//LocateColmena();
			#endregion
			
			//filePath = @"C:\Users\josej_000\Desktop\pruebas y ejemplos\DotSpatial2\mapas\rasters\O44121a1.dem";
			OperationsWithRasters();
			
			base.OnShown(e);
		}

		private void LocateColmena()
		{
			IFeatureSet border = FeatureSet.Open(@"C:\Users\josej_000\Desktop\pruebas y ejemplos\mapa colombia\Boundaries_20150602_230753.shp");
			border.Projection = appManager1.Map.Projection;
			IMapFeatureLayer mapLayer2 = appManager1.Map.Layers.Add(border);

			IFeatureSet buildings = FeatureSet.Open(@"C:\Users\josej_000\Desktop\pruebas y ejemplos\mapa colombia\OpenStreetMap\buildings.shp");
			buildings.Projection = appManager1.Map.Projection;
			IMapFeatureLayer mapLayer = appManager1.Map.Layers.Add(buildings);
			FilterColmena(mapLayer);
		}

		private void FilterColmena(IMapFeatureLayer mapLayer)
		{
			var featureSet = new FeatureSet(FeatureType.Point);
			featureSet.Projection = appManager1.Map.Projection;

			var myCoord = new Coordinate();
			myCoord.X = -75.57245;
			myCoord.Y = 6.20263;

			var myPoint = new DotSpatial.Topology.Point(myCoord);
			
			IFeature myFeature = featureSet.AddFeature(myPoint);
			featureSet.AddFeature(myFeature);

			var pointLayer = appManager1.Map.Layers.Add(featureSet);
			pointLayer.LegendText = "Edificio Colmena";

			DrawStars(pointLayer, Color.Yellow, DotSpatial.Symbology.PointShape.Star, 16);
			

			appManager1.Map.ResetBuffer();
		}

		#region OperationsWithRasters
		private async void OperationsWithRasters()
		{
			this.UseWaitCursor = true;
			IMapRasterLayer myLayer = await LoadRasterAsync(filePath);
			Task task2 = ApplyQuantileAsync(myLayer);
			this.UseWaitCursor = false;
			//AddLighting(myLayer);
			//GlacierColoring(myLayer);
			//ApplyQuantile(myLayer);
		}

		private async Task ApplyQuantileAsync(IMapRasterLayer myLayer)
		{
			await Task.Factory.StartNew(() =>
			{
				ApplyQuantile(myLayer);
			});
		}

		private Task<IMapRasterLayer> LoadRasterAsync(string filePath)
		{
			return Task.Factory.StartNew(() => LoadRasterLayer(filePath));
		}

		private IMapRasterLayer LoadRasterLayer(string filePath)
		{
			IRaster raster = Raster.Open(filePath);
			IMapRasterLayer myLayer = appManager1.Map.Layers.Add(raster);
			appManager1.Map.ZoomToMaxExtent();
			return myLayer;
		}

		private void AddLighting(IMapRasterLayer myLayer)
		{
			myLayer.Symbolizer.Scheme.Categories[0].Range = new Range(3200, 3450);
			myLayer.Symbolizer.Scheme.Categories[0].ApplyMinMax(myLayer.Symbolizer.EditorSettings);
			myLayer.Symbolizer.Scheme.Categories[1].Range = new Range(3440, 3700);
			myLayer.Symbolizer.Scheme.Categories[1].ApplyMinMax(myLayer.Symbolizer.EditorSettings);
			myLayer.Symbolizer.ShadedRelief.ElevationFactor = 1;
			myLayer.Symbolizer.ShadedRelief.IsUsed = true;
			myLayer.WriteBitmap();
		}

		private void ControlRange(IMapRasterLayer myLayer)
		{
			myLayer.Symbolizer.Scheme.Categories[0].Range = new Range(3200, 3450);
			myLayer.Symbolizer.Scheme.Categories[0].ApplyMinMax(myLayer.Symbolizer.EditorSettings);
			myLayer.Symbolizer.Scheme.Categories[1].Range = new Range(3440, 3700);
			myLayer.Symbolizer.Scheme.Categories[1].ApplyMinMax(myLayer.Symbolizer.EditorSettings);
			myLayer.WriteBitmap();
		}

		private void GlacierColoring(IMapRasterLayer myLayer)
		{
			myLayer.Symbolizer.Scheme.ApplyScheme(ColorSchemeType.Glaciers, myLayer.DataSet);
			myLayer.Symbolizer.Scheme.Categories[0].Range = new Range(3200, 3450);
			myLayer.Symbolizer.Scheme.Categories[0].ApplyMinMax(myLayer.Symbolizer.EditorSettings);
			myLayer.Symbolizer.Scheme.Categories[1].Range = new Range(3440, 3700);
			myLayer.Symbolizer.Scheme.Categories[1].ApplyMinMax(myLayer.Symbolizer.EditorSettings);
			myLayer.Symbolizer.ShadedRelief.ElevationFactor = 1;
			myLayer.Symbolizer.ShadedRelief.IsUsed = true;
			myLayer.WriteBitmap();
		}

		private void ApplyQuantile(IMapRasterLayer myLayer)
		{
			myLayer.Symbolizer.EditorSettings.IntervalMethod = IntervalMethod.Quantile;
			myLayer.Symbolizer.EditorSettings.NumBreaks = 5;
			myLayer.Symbolizer.Scheme.CreateCategories(myLayer.DataSet);
			myLayer.Symbolizer.ShadedRelief.ElevationFactor = 1;
			myLayer.Symbolizer.ShadedRelief.IsUsed = true;
			myLayer.WriteBitmap();
		} 
		#endregion

		#region OperationsWithVectors
		#region OperationsWithLines
		private void OperationsWithLines(IMapFeatureLayer myLayer)
		{
			//ColoringRoads(myLayer);

			//ColoringRoadsOutline(myLayer);

			//ColoringRoadsById(myLayer);

			//CustomRoadCategories(myLayer);

			//LinesMultipleStrokes(myLayer);
		}

		private static void ColoringRoads(IMapFeatureLayer mapLayer2)
		{
			mapLayer2.Symbolizer = new LineSymbolizer(Color.Brown, 1);
		}

		private static void ColoringRoadsOutline(IMapFeatureLayer mapLayer2)
		{
			LineSymbolizer road = new LineSymbolizer(Color.Yellow, 2);
			road.SetOutline(Color.Black, 1);
			mapLayer2.Symbolizer = road;
		}

		private static void ColoringRoadsById(IMapFeatureLayer mapLayer2)
		{
			LineScheme myScheme = new LineScheme();
			myScheme.EditorSettings.ClassificationType = ClassificationType.UniqueValues;
			myScheme.EditorSettings.FieldName = "CARTO";
			myScheme.CreateCategories(mapLayer2.DataSet.DataTable);
			mapLayer2.Symbology = myScheme;
		}

		private void CustomRoadCategories(IMapFeatureLayer layer)
		{
			LineScheme myScheme = new LineScheme();
			myScheme.Categories.Clear();
			LineCategory low = new LineCategory(Color.Blue, 2);
			low.FilterExpression = "[CARTO] = 3";
			low.LegendText = "Low";
			LineCategory high = new LineCategory(Color.Red, Color.Black, 6, DashStyle.Solid, LineCap.Triangle);
			high.FilterExpression = "[CARTO] = 2";
			high.LegendText = "High";
			myScheme.AppearsInLegend = true;
			myScheme.LegendText = "CARTO";
			myScheme.Categories.Add(low);
			myScheme.Categories.Add(high);
			layer.Symbology = myScheme;
		}

		private void LinesMultipleStrokes(IMapFeatureLayer mapLayer2)
		{
			LineSymbolizer mySymbolizer = new LineSymbolizer();
			mySymbolizer.Strokes.Clear();
			CartographicStroke ties = new CartographicStroke(Color.Brown);
			ties.DashPattern = new float[] { 1 / 6f, 2 / 6f };
			ties.Width = 6;
			ties.EndCap = LineCap.Flat;
			ties.StartCap = LineCap.Flat;
			CartographicStroke rails = new CartographicStroke(Color.DarkGray);
			rails.CompoundArray = new float[] { .15f, .3f, .6f, .75f };
			rails.Width = 6;
			rails.EndCap = LineCap.Flat;
			rails.StartCap = LineCap.Flat;
			mySymbolizer.Strokes.Add(ties);
			mySymbolizer.Strokes.Add(rails);
			mapLayer2.Symbolizer = mySymbolizer;
		}
		#endregion

		#region OperationsWithPoints
		private void OperationsWithPoints(IMapFeatureLayer myLayer)
		{
			//DrawStars(myLayer,Color.Yellow, DotSpatial.Symbology.PointShape.Star, 16);
			//FilterCitiesByArea(myLayer);
			//Quantile(myLayer);
		}

		private void Quantile(IMapFeatureLayer myLayer)
		{
			IMapPointLayer myPointLayer = myLayer as IMapPointLayer;
			if (myPointLayer == null)
				return;
			PointScheme myScheme = new PointScheme();
			myScheme.Categories.Clear();
			myScheme.EditorSettings.ClassificationType = ClassificationType.Quantities;
			myScheme.EditorSettings.IntervalMethod = IntervalMethod.Quantile;
			myScheme.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding;
			myScheme.EditorSettings.IntervalRoundingDigits = 5;
			myScheme.EditorSettings.TemplateSymbolizer = new PointSymbolizer(Color.Yellow, DotSpatial.Symbology.PointShape.Star, 16);
			myScheme.EditorSettings.FieldName = "Area";
			myScheme.CreateCategories(myLayer.DataSet.DataTable);
			myPointLayer.Symbology = myScheme;
		}

		private void FilterCitiesByArea(IMapFeatureLayer myLayer)
		{
			IMapPointLayer myPointLayer = myLayer as IMapPointLayer;
			if (myPointLayer != null)
			{
				PointScheme myScheme = new PointScheme();
				myScheme.Categories.Clear();
				PointCategory smallSize = new PointCategory(Color.Blue, DotSpatial.Symbology.PointShape.Rectangle, 4);
				smallSize.FilterExpression = "[Area] < 1e+08";
				smallSize.LegendText = "Small Cities";
				myScheme.AddCategory(smallSize);

				PointCategory largeSize = new PointCategory(Color.Yellow, DotSpatial.Symbology.PointShape.Star, 16);
				largeSize.FilterExpression = "[Area] >= 1e+08";
				largeSize.LegendText = "Large Cities";
				largeSize.Symbolizer.SetOutline(Color.Black, 1);
				myScheme.AddCategory(largeSize);

				myPointLayer.Symbology = myScheme;
			}
		}

		private static void DrawStars(IMapFeatureLayer myLayer, Color color, DotSpatial.Symbology.PointShape pointShape, int size)
		{
			myLayer.Symbolizer = new PointSymbolizer(color, pointShape, size);
			myLayer.Symbolizer.SetOutline(Color.Black, 1);
		}
		#endregion

		#region OperationsWithPolygons
		private void OperationsWithPolygons(IMapFeatureLayer myLayer)
		{
			//BlueOutlinedPolygons(myLayer);
			//Gradient(myLayer);
			//SpecificGradient(myLayer);
			//CoolColorsGradient(myLayer);
			//CustomPolygon(myLayer);
			//Patterns(myLayer);
		}
		private void BlueOutlinedPolygons(IMapFeatureLayer myLayer)
		{
			PolygonSymbolizer lightblue = new PolygonSymbolizer(Color.LightBlue);
			lightblue.OutlineSymbolizer = new LineSymbolizer(Color.Blue, 1);
			myLayer.Symbolizer = lightblue;
		}

		private void Gradient(IMapFeatureLayer mapLayer)
		{
			PolygonSymbolizer blueGradient = new PolygonSymbolizer(Color.LightSkyBlue, Color.DarkBlue, 45, GradientType.Linear);
			blueGradient.SetOutline(Color.Yellow, 1);
			mapLayer.Symbolizer = blueGradient;
		}

		private void SpecificGradient(IMapFeatureLayer myLayer)
		{
			PolygonSymbolizer blueGradient = new PolygonSymbolizer(Color.LightSkyBlue, Color.DarkBlue, -45, GradientType.Linear);
			blueGradient.SetOutline(Color.Yellow, 1);
			PolygonScheme myScheme = new PolygonScheme();
			myScheme.EditorSettings.TemplateSymbolizer = blueGradient;
			myScheme.EditorSettings.UseColorRange = false;
			myScheme.EditorSettings.ClassificationType = ClassificationType.UniqueValues;
			myScheme.EditorSettings.FieldName = "name";
			myScheme.CreateCategories(myLayer.DataSet.DataTable);
			myLayer.Symbology = myScheme;
		}

		private void CoolColorsGradient(IMapFeatureLayer mapLayer)
		{
			PolygonScheme myScheme = new PolygonScheme();
			myScheme.EditorSettings.StartColor = Color.LightGreen;
			myScheme.EditorSettings.EndColor = Color.LightSalmon;
			myScheme.EditorSettings.ClassificationType = ClassificationType.UniqueValues;
			myScheme.EditorSettings.FieldName = "name";
			myScheme.EditorSettings.UseGradient = true;
			myScheme.CreateCategories(mapLayer.DataSet.DataTable);
			mapLayer.Symbology = myScheme;
		}

		private void CustomPolygon(IMapFeatureLayer mapLayer)
		{
			PolygonScheme scheme = new PolygonScheme();
			PolygonCategory miranda = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
			miranda.FilterExpression = "[name] = 'Washington'";
			miranda.LegendText = "Washington";
			PolygonCategory nWords = new PolygonCategory(Color.Pink, Color.DarkRed, 1);
			nWords.FilterExpression = "[name] Like 'G*'";
			nWords.LegendText = "G Words";
			scheme.ClearCategories();
			scheme.AddCategory(miranda);
			scheme.AddCategory(nWords);
			mapLayer.ShowLabels = true;
			mapLayer.Symbology = scheme;
			appManager1.Map.Refresh();
		}

		private void Patterns(IMapFeatureLayer mapLayer)
		{
			PolygonSymbolizer mySymbolizer = new PolygonSymbolizer();
			mySymbolizer.Patterns.Add(new HatchPattern(HatchStyle.WideDownwardDiagonal, Color.Red, Color.Transparent));
			mySymbolizer.LegendText = "Zona en reclamación";

			PolygonScheme scheme = new PolygonScheme();
			PolygonCategory venezuela = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
			venezuela.FilterExpression = "[ESTADO] <> 'Zona en reclamación'";
			venezuela.LegendText = "Venezuela";

			scheme.AddCategory(venezuela);
			mapLayer.ShowLabels = true;
			mapLayer.Symbology = scheme;


			mapLayer.Symbolizer = mySymbolizer;
		}
		#endregion

		#region OperationsWithLabels
		private void OperationsWithLabels(IMapFeatureLayer mapLayer)
		{
			//FieldNameLabels(mapLayer);
			MultiLineLabels(mapLayer);
		}

		private void FieldNameLabels(IMapFeatureLayer mapLayer)
		{
			IMapLabelLayer labelLayer = new MapLabelLayer();
			ILabelCategory category = labelLayer.Symbology.Categories[0];
			category.Expression = "[NAME]";
			category.Symbolizer.Orientation = ContentAlignment.MiddleCenter;
			mapLayer.ShowLabels = true;
			mapLayer.LabelLayer = labelLayer;
		}

		private void MultiLineLabels(IMapFeatureLayer mapLayer)
		{
			IMapLabelLayer labelLayer = new MapLabelLayer();
			ILabelCategory category = labelLayer.Symbology.Categories[0];
			category.Expression = "[NAME]\nPopulation: [POP_LASTCE]";
			category.FilterExpression = "[NAME] Like 'G*'";
			category.Symbolizer.BackColorEnabled = true;
			category.Symbolizer.BorderVisible = true;
			category.Symbolizer.Orientation = ContentAlignment.MiddleCenter;
			category.Symbolizer.Alignment = StringAlignment.Center;
			mapLayer.ShowLabels = true;
			mapLayer.LabelLayer = labelLayer;
		}

		#endregion 
		#endregion
	}
}
