using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;


namespace Crypto_App
{
    /// <summary>
    /// Логика взаимодействия для AssetInfoWindow.xaml
    /// </summary>
    public partial class AssetInfoWindow : Window
    {
        private OxyColor PlotColour;

        public AssetInfoWindow()
        {

            Uri iconUri = new Uri(findMyDir(Environment.CurrentDirectory) + "icon.ico", UriKind.Absolute);
            this.Icon = BitmapFrame.Create(iconUri);

            DirectoryInfo DirToSave = new DirectoryInfo(findMyDir(Environment.CurrentDirectory) + @"Themes");

            FileInfo FileWithThemes = new FileInfo(DirToSave + "\\CurrentTheme.txt");

            if (!FileWithThemes.Exists)
            {
                FileWithThemes.Create().Close();
                using (StreamWriter sw = new StreamWriter(FileWithThemes.FullName))
                {
                    sw.WriteLine("1");
                }
            }

            int ThemeNum;

            using (StreamReader sr = File.OpenText(FileWithThemes.FullName))
            {
                ThemeNum = int.Parse(sr.ReadLine());
            }

            Uri uri = null;



            switch (ThemeNum)
            {
                case 1:

                    uri = new Uri("Themes\\Pink.xaml", UriKind.Relative);
                    PlotColour = OxyColor.FromRgb(240, 230, 140);
                    break;


                case 2:
                    uri = new Uri("Themes\\Gray.xaml", UriKind.Relative);
                    PlotColour = OxyColor.FromRgb(240, 248, 255);
                    break;

                case 3:
                    uri = new Uri("Themes\\BlackOrange.xaml", UriKind.Relative);
                    PlotColour = OxyColor.FromRgb(255, 165, 0);
                    break;

                case 4:
                    uri = new Uri("Themes\\Black.xaml", UriKind.Relative);
                    PlotColour = OxyColor.FromRgb(240, 248, 255);
                    break;

                case 5:
                    uri = new Uri("Themes\\White.xaml", UriKind.Relative);
                    PlotColour = OxyColor.FromRgb(0, 0, 0);
                    break;
            }

            if (uri != null)
            {
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                // очищаем коллекцию ресурсов приложения
                Application.Current.Resources.Clear();
                // добавляем загруженный словарь ресурсов
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
            }

            InitializeComponent();
        }



        public AssetInfoWindow(string DirToSavePath, string timingToShow) : this()
        {


            DirectoryInfo DirToSave = new DirectoryInfo(DirToSavePath);


            DateTime time = DateTime.Now;

            double? oneMinuteChange;
            double? fiveMinuteChange;
            double? oneHourChange;


            FileInfo BeforeFileWithData = new FileInfo(DirToSave.FullName + $"\\{time.Year}_{time.Month}_{time.Day}_{time.Hour - 1}.txt");
            FileInfo CurrentFileWithData = new FileInfo(DirToSave.FullName + $"\\{time.Year}_{time.Month}_{time.Day}_{time.Hour}.txt");

            //1min
            if (!CurrentFileWithData.Exists)
                oneMinuteChange = null;
            else
            if (time.Minute > 0)
            {
                double? beforeVal = null;
                double? currentVal = null;

                Dictionary<DateTime, decimal> DateValuePair;
                using (StreamReader sr = File.OpenText(CurrentFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == time.Minute - 1)
                    {
                        beforeVal = (double)item.Value;
                    }
                    if (item.Key.Minute == time.Minute)
                    {
                        currentVal = (double)item.Value;
                        break;
                    }
                }
                if (beforeVal == null || currentVal == null)
                    oneMinuteChange = null;
                else
                    oneMinuteChange = currentVal - beforeVal;
            }
            else
            if (!BeforeFileWithData.Exists)
                oneMinuteChange = null;
            else
            {
                double? beforeVal = null;
                double? currentVal = null;

                Dictionary<DateTime, decimal> DateValuePair;
                using (StreamReader sr = File.OpenText(BeforeFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == 60 + (time.Minute - 1))
                    {
                        beforeVal = (double)item.Value;
                        break;
                    }
                }
                using (StreamReader sr = File.OpenText(CurrentFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == time.Minute)
                    {
                        currentVal = (double)item.Value;
                        break;
                    }
                }
                if (beforeVal == null || currentVal == null)
                    oneMinuteChange = null;
                else
                    oneMinuteChange = currentVal - beforeVal;
            }
            //5min
            if (!CurrentFileWithData.Exists)
                fiveMinuteChange = null;
            else
            if (time.Minute > 4)
            {
                double? beforeVal = null;
                double? currentVal = null;

                Dictionary<DateTime, decimal> DateValuePair;
                using (StreamReader sr = File.OpenText(CurrentFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == time.Minute - 5)
                    {
                        beforeVal = (double)item.Value;
                    }
                    if (item.Key.Minute == time.Minute)
                    {
                        currentVal = (double)item.Value;
                        break;
                    }
                }
                if (beforeVal == null || currentVal == null)
                    fiveMinuteChange = null;
                else
                    fiveMinuteChange = currentVal - beforeVal;
                //if (fiveMinuteChange != null)
                //{
                //    if (fiveMinuteChange < 0 && fiveMinuteChange > -(beforeVal * 0.01))
                //        MessageBox.Show(DirToSave.Name + " falls");
                //    if (fiveMinuteChange > 0 && fiveMinuteChange > (beforeVal * 0.01))
                //        MessageBox.Show(DirToSave.Name + " rises");
                //}

            }
            else
            if (!BeforeFileWithData.Exists)
                fiveMinuteChange = null;
            else
            {
                double? beforeVal = null;
                double? currentVal = null;

                Dictionary<DateTime, decimal> DateValuePair;
                using (StreamReader sr = File.OpenText(BeforeFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == 60 + (time.Minute - 5))
                    {
                        beforeVal = (double)item.Value;
                        break;
                    }
                }
                using (StreamReader sr = File.OpenText(CurrentFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == time.Minute)
                    {
                        currentVal = (double)item.Value;
                        break;
                    }
                }
                if (beforeVal == null || currentVal == null)
                    fiveMinuteChange = null;
                else
                    fiveMinuteChange = currentVal - beforeVal;

                //if (fiveMinuteChange != null)
                //{
                //    if (fiveMinuteChange < 0 && fiveMinuteChange > -(beforeVal * 0.01))
                //        MessageBox.Show(DirToSave.Name + " falls");
                //    if (fiveMinuteChange > 0 && fiveMinuteChange > (beforeVal * 0.01))
                //        MessageBox.Show(DirToSave.Name + " rises");
                //}
            }
            //1h
            if (!CurrentFileWithData.Exists)
                oneHourChange = null;
            else
            if (!BeforeFileWithData.Exists)
                oneHourChange = null;
            else
            {
                double? beforeVal = null;
                double? currentVal = null;

                Dictionary<DateTime, decimal> DateValuePair;
                using (StreamReader sr = File.OpenText(BeforeFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == time.Minute)
                    {
                        beforeVal = (double)item.Value;
                        break;
                    }
                }
                using (StreamReader sr = File.OpenText(CurrentFileWithData.FullName))
                {
                    DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                }
                foreach (var item in DateValuePair)
                {
                    if (item.Key.Minute == time.Minute)
                    {
                        currentVal = (double)item.Value;
                        break;
                    }
                }
                if (beforeVal == null || currentVal == null)
                    oneHourChange = null;
                else
                    oneHourChange = currentVal - beforeVal;
            }



            PlotModel MyModel;
            MyModel = new PlotModel { Title = DirToSave.Name };

            List<FileInfo> fileInfos = new List<FileInfo>(DirToSave.GetFiles());

            DateTime todayDate = DateTime.Now.AddDays(1);
            int todayHourMinus = -1;
            int todayHour = -1;

            bool ImWeek = false;
            bool ImMonth = false;

            DateTime yersterday1 = DateTime.Now.AddDays(1);
            DateTime yersterday2 = DateTime.Now.AddDays(1);
            DateTime yersterday3 = DateTime.Now.AddDays(1);
            DateTime yersterday4 = DateTime.Now.AddDays(1);
            DateTime yersterday5 = DateTime.Now.AddDays(1);
            DateTime yersterday6 = DateTime.Now.AddDays(1);

            int currentMonth = -1;

            switch (timingToShow)
            {
                case "hour":
                    todayHourMinus = DateTime.Now.Hour - 1;
                    todayHour = DateTime.Now.Hour;
                    break;
                case "day":
                    todayDate = DateTime.Now.Date;
                    break;
                case "week":
                    todayDate = DateTime.Now.Date;
                    yersterday1 = DateTime.Now.Date.AddDays(-1);
                    yersterday2 = DateTime.Now.Date.AddDays(-2);
                    yersterday3 = DateTime.Now.Date.AddDays(-3);
                    yersterday4 = DateTime.Now.Date.AddDays(-4);
                    yersterday5 = DateTime.Now.Date.AddDays(-5);
                    yersterday6 = DateTime.Now.Date.AddDays(-6);
                    ImWeek = true;
                    break;
                case "month":
                    currentMonth = DateTime.Now.Month;
                    ImMonth = true;
                    break;
            }



            foreach (var file in fileInfos)
            {
                if ((((file.CreationTime.Hour == todayHourMinus & file.CreationTime.Date == DateTime.Now.Date) | (file.CreationTime.Hour == todayHour & file.CreationTime.Date == DateTime.Now.Date)) | file.CreationTime.Date == todayDate | (file.CreationTime.Date == yersterday1 | file.CreationTime.Date == yersterday2 | file.CreationTime.Date == yersterday3 | file.CreationTime.Date == yersterday4 | file.CreationTime.Date == yersterday5 | file.CreationTime.Date == yersterday6) | file.CreationTime.Month == currentMonth) && file.Name.EndsWith(".txt") && !file.Name.EndsWith("WindowParams.txt"))
                {
                    Dictionary<DateTime, decimal> DateValuePair;


                    using (StreamReader sr = File.OpenText(file.FullName))
                    {
                        DateValuePair = JsonConvert.DeserializeObject<Dictionary<DateTime, decimal>>(sr.ReadLine());
                    }

                    int StepCount = 1;

                    double LastPointVal = 0;
                    double LastTimeVal = 0;



                    foreach (var dvp in DateValuePair)
                    {
                        if ((double)dvp.Value == -1)
                        {

                        }
                        else if (StepCount == 1)
                        {
                            if (ImMonth | ImWeek)
                            {
                                LastTimeVal = 0.02 + dvp.Key.Day + dvp.Key.Hour * 0.04 + dvp.Key.Minute * 0.00066667;
                            }
                            else
                            {
                                LastTimeVal = dvp.Key.Hour + dvp.Key.Minute * 0.01666667;
                            }
                            LastPointVal = (double)dvp.Value;
                            LineSeries ls = new LineSeries();

                            ls.MarkerType = MarkerType.Circle;
                            ls.Color = OxyColor.FromRgb(255, 165, 0);
                            ls.MarkerSize = 2;
                            ls.MarkerStrokeThickness = 3;
                            ls.Points.Add(new DataPoint(LastTimeVal, LastPointVal));



                            MyModel.Series.Add(ls);
                        }
                        else
                        {
                            double TimeVal;

                            if (ImMonth | ImWeek)
                            {
                                TimeVal = 0.02 + dvp.Key.Day + dvp.Key.Hour * 0.04 + dvp.Key.Minute * 0.00066667;
                            }
                            else
                            {
                                TimeVal = dvp.Key.Hour + dvp.Key.Minute * 0.01666667;
                            }
                            double PointVal = (double)dvp.Value;

                            if ((TimeVal - LastTimeVal) > 0.02)
                            {
                                LineSeries ls = new LineSeries();

                                ls.MarkerType = MarkerType.Circle;
                                ls.Color = OxyColor.FromRgb(128, 128, 128);
                                ls.MarkerSize = 1;
                                ls.MarkerStrokeThickness = 3;
                                ls.Points.Add(new DataPoint(LastTimeVal, LastPointVal));

                                MyModel.Series.Add(ls);

                                ls = new LineSeries();

                                ls.MarkerType = MarkerType.Circle;
                                ls.Color = OxyColor.FromRgb(128, 128, 128);
                                ls.MarkerSize = 1;
                                ls.MarkerStrokeThickness = 3;
                                ls.Points.Add(new DataPoint(TimeVal, PointVal));

                                MyModel.Series.Add(ls);
                            }
                            else
                            {
                                var fs = new FunctionSeries();
                                fs.Points.Add(new DataPoint(LastTimeVal, LastPointVal));
                                fs.Points.Add(new DataPoint(TimeVal, PointVal));

                                if (LastPointVal > PointVal)
                                {
                                    fs.Color = OxyColor.FromRgb(255, 0, 0);
                                }
                                else if (LastPointVal == PointVal)
                                {
                                    fs.Color = OxyColor.FromRgb(128, 128, 128);
                                }
                                else
                                {
                                    fs.Color = OxyColor.FromRgb(0, 214, 120);
                                }
                                MyModel.Series.Add(fs);

                            }
                            LastPointVal = PointVal;
                            LastTimeVal = TimeVal;
                            if (StepCount == DateValuePair.Count)
                            {

                                LineSeries ls = new LineSeries();

                                ls.MarkerType = MarkerType.Circle;
                                ls.Color = OxyColor.FromRgb(255, 165, 0);
                                ls.MarkerSize = 2;
                                ls.MarkerStrokeThickness = 3;
                                ls.Points.Add(new DataPoint(LastTimeVal, LastPointVal));


                                MyModel.Series.Add(ls);
                            }

                        }
                        StepCount++;
                    }


                }


            }

            StackPanel stackWithShanges = new StackPanel();
            stackWithShanges.Orientation = Orientation.Vertical;
            stackWithShanges.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            stackWithShanges.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;


            Style style = this.FindResource("TextBlockStyle") as Style;


            TextBlock LastMinChange = new TextBlock();

            LastMinChange.Style = style;
            if (oneMinuteChange == null)
                LastMinChange.Text = "1 min:  ?";
            else
                LastMinChange.Text = "1 min:  " + (float)oneMinuteChange;
            stackWithShanges.Children.Add(LastMinChange);

            TextBlock LastFiveMinChange = new TextBlock();
            LastFiveMinChange.Style = style;

            if (fiveMinuteChange == null)
                LastFiveMinChange.Text = "5 min:  ?";
            else
                LastFiveMinChange.Text = "5 min:  " + (float)fiveMinuteChange;
            stackWithShanges.Children.Add(LastFiveMinChange);

            TextBlock LastHourChange = new TextBlock();
            LastHourChange.Style = style;

            if (oneHourChange == null)
                LastHourChange.Text = "1 hour: ?";
            else
                LastHourChange.Text = "1 hour: " + (float)oneHourChange;
            stackWithShanges.Children.Add(LastHourChange);

            changeGrid.Children.Add(stackWithShanges);



            PlotView plotView = new PlotView();


            style = this.FindResource("PlotViewStyle") as Style;
            plotView.Style = style;
            style = this.FindResource("WindowStyle") as Style;
            this.Style = style;


            LinearAxis linearAxis1 = new LinearAxis();
            linearAxis1.AxislineColor = PlotColour;
            linearAxis1.ExtraGridlineColor = PlotColour;
            linearAxis1.MajorGridlineColor = PlotColour;
            linearAxis1.MinorGridlineColor = PlotColour;
            linearAxis1.MinorTicklineColor = PlotColour;
            linearAxis1.TextColor = PlotColour;
            linearAxis1.TicklineColor = PlotColour;
            linearAxis1.TitleColor = PlotColour;

            linearAxis1.TitlePosition = 0.5;

            //linearAxis1.Title = "prise";

            linearAxis1.Position = AxisPosition.Left;

            LinearAxis linearAxis2 = new LinearAxis();
            linearAxis2.AxislineColor = PlotColour;
            linearAxis2.ExtraGridlineColor = PlotColour;
            linearAxis2.MajorGridlineColor = PlotColour;
            linearAxis2.MinorGridlineColor = PlotColour;
            linearAxis2.MinorTicklineColor = PlotColour;
            linearAxis2.TextColor = PlotColour;
            linearAxis2.TicklineColor = PlotColour;
            linearAxis2.TitleColor = PlotColour;
            linearAxis2.TitlePosition = 0.5;

            //linearAxis2.Title = "time";

            linearAxis2.Position = AxisPosition.Bottom;

            MyModel.Axes.Add(linearAxis1);
            MyModel.Axes.Add(linearAxis2);

            MyModel.TitleColor = PlotColour;
            MyModel.PlotAreaBorderColor = PlotColour;

            plotView.Model = MyModel;

            mainGrid.Children.Add(plotView);

        }
        string findMyDir(string path)
        {
            if (path.EndsWith("Crypto_App\\"))
            {
                return path;
            }
            else
                return findMyDir(path.Remove(path.Length - 1));
        }
    }
}
