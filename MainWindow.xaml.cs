using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Interop;

using Microsoft.Build.Collections;

using Microsoft.Build.Evaluation;
using System.Collections;
using Microsoft.Build.Execution;
using Microsoft.Build.Shared;
using Microsoft.Build.Construction;

namespace GeneticAlgorithm
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private GeneticEvolution ge;
        double AnimStart = 0.005;
        double AnimTime = 1;
        int n;
        bool CanBeRun = true;
        List<SolidColorBrush> Color;
        Dictionary<string, List<Grid>> Population;
        List<Individual> IndividualForKilling;
        static System.Windows.Forms.Timer TimerForNewGener = new System.Windows.Forms.Timer();
        static System.Windows.Forms.Timer TimerForEnd = new System.Windows.Forms.Timer();
        public MainWindow()
        {

            InitializeComponent();
            ImageBrush Brush1 = new ImageBrush();
            var handel = Properties.Resources.background.GetHbitmap();
            Brush1.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(handel, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); ;
            this.Background = Brush1;
            ImageBrush Brush2 = new ImageBrush();
            handel = Properties.Resources.inst.GetHbitmap();
            Brush2.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(handel, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); ;
            btnHelp.Background = Brush2;
            Color = new List<SolidColorBrush>();
            Color.Add(new SolidColorBrush(Colors.Blue));
            Color.Add(new SolidColorBrush(Colors.Aqua));
            Color.Add(new SolidColorBrush(Colors.Yellow));
            Color.Add(new SolidColorBrush(Colors.Salmon));
            Color.Add(new SolidColorBrush(Colors.Chartreuse));
            Color.Add(new SolidColorBrush(Colors.DarkTurquoise));
            TimerForNewGener.Tick += new EventHandler(NewGeneration);//таймер для вызова события - генерации нового поколения
            TimerForNewGener.Interval = 7000;
            TimerForEnd.Tick += new EventHandler(EndOfAnimation);//таймер для вызова события - конца анимации алгоритма
            TimerForEnd.Interval = 3000;
            tbFunction.Focus();

        }
        private bool Initialize()
        {
            TimerForNewGener.Stop();//останавливаем предыдущий таймер, если он был
            Panel.Children.Clear();//очищаем canvas
            n = 1;
            Population = new Dictionary<string, List<Grid>>();//обновляем словарь для новой функции
            if(tbFunction.Text.Length > 50)
            {
                MessageBox.Show("Please enter limited length function", "Incorrect function", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            try
            {
                ge = new GeneticEvolution(tbFunction.Text.Trim(' '));//проверяем функцию на корректность
                return true;
            }
            catch
            {
                MessageBox.Show("Please enter a valid function", "Incorrect function", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void btStarted_Click(object sender, EventArgs e)//событие нажатия кнопки запуска алгоритма
        {
            if (!CanBeRun)
                return;
            if (!Initialize())
                return;
            CanBeRun = false;
            List<Individual> Generation = ge.Initialize();//формируем первичное поколение
            for (int i = 0; i < Generation.Count; i++)
            {
                FormNewInd(Generation[i], i, 0, AnimStart);
            }
            BlockButton(true);
            TimerForNewGener.Start();//запускаем таймер запуска формирования нового покления
        }
        private void NewGeneration(object sender, EventArgs e)
        {
            if (n > 20)
            { 
                TimerForEnd.Start();
                return;
            }
            TimerForNewGener.Interval = 5500;//делаем таймер короче, так как следующие поколения формируются быстрее чем первое 
            List<Individual> Children = new List<Individual>();
            IndividualForKilling = new List<Individual>();
            if (n <= 5)
            {
                Children = ge.Evolute();//проверка на корректность функции
                IndividualForKilling = ge.Killing();
            }
            else
            {
                for (int j = 0; j < 2; j++)
                {
                    ge.Evolute();
                    IndividualForKilling.AddRange(ge.Killing());
                }
                Children = ge.Evolute();
                IndividualForKilling.AddRange(ge.Killing());
            }
            n++;//подсчитываем номер нового поколения для смены цвета
            int i;
            for (i = 0; i < Children.Count; i++)//формируем новое поколение
            {
                bool EndOfGeneration = false;
                if (i == Children.Count - 1)
                    EndOfGeneration = true;
                FormNewInd(Children[i], i, 0, AnimStart, EndOfGeneration);
            }
            BlockButton(true);
        }
        private void FormNewInd(Individual IndFromAlgorithm, int bias, double AnimTime, double AnimStart, bool EndOfGeneration = false)//функция визуализации нововй особи
        {
            Grid NewIndividual = new Grid()//формирeем оболочку для изображения особи
            {
                Width = 50,
                Height = 50
            };
            Ellipse El = new Ellipse()//формируем эллипс для изображения особи на канвас
            {
                Style = (Style)this.Resources["Ellipse"],
                Fill = Color[n % 6],
                Stroke = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            TextBlock Number = new TextBlock()//формируем надпись на эллипсе
            {
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 8,
                FontWeight = FontWeights.Bold
            };
            NewIndividual.Children.Add(El);
            NewIndividual.Children.Add(Number);
            Canvas.SetLeft(NewIndividual, ((int)(IndFromAlgorithm.x + 1000) / 2000.0 * (int)(Panel.ActualWidth - NewIndividual.Width)));
            Canvas.SetTop(NewIndividual, (Panel.ActualHeight - NewIndividual.Height) - ((int)(IndFromAlgorithm.y + 1000) / 2000.0 * (int)(Panel.ActualHeight - NewIndividual.Height)));
            Panel.Children.Add(NewIndividual);//добавляем на канвас новую особь
            string IndivStr = IndFromAlgorithm.x.ToString() + "/" + IndFromAlgorithm.y.ToString();
            if (!Population.Keys.Contains(IndivStr))
                Population[IndivStr] = new List<Grid>();//добавляем в словарь новую особь
            Population[IndivStr].Add(NewIndividual);
            GetAnimation(IndFromAlgorithm, bias, AnimTime, AnimStart, EndOfGeneration, El, Number);
        }
        private void GetAnimation(Individual IndFromAlgorithm, int bias, double AnimTime, double AnimStart, bool EndOfGeneration, Ellipse El, TextBlock Number)
        {
            ElasticEase IFunc = new ElasticEase()
            {
                EasingMode = EasingMode.EaseInOut,
                Oscillations=25,
                Springiness=10
            };
            CircleEase IFunc1 = new CircleEase()
            {

            };
            DoubleAnimation ElWAnimation = new DoubleAnimation()//формируем анимацию для появления эллипса
            {
                From = 35,
                To = 35,
                Duration = TimeSpan.FromSeconds(AnimTime),
                BeginTime = TimeSpan.FromSeconds(AnimStart * bias),
                EasingFunction= IFunc1
            };
            El.BeginAnimation(Ellipse.WidthProperty, ElWAnimation);
            if (EndOfGeneration)// если особь крайняя крепим за ней событие отбора
            {
                ElWAnimation.Completed += Killing;
            }
            El.BeginAnimation(Ellipse.HeightProperty, ElWAnimation);

            StringAnimationUsingKeyFrames NumAnimation = new StringAnimationUsingKeyFrames()//формируем анимацию для появления текста на эллипсе
            {
                Duration = TimeSpan.FromSeconds(0),
                BeginTime = TimeSpan.FromSeconds(AnimStart * bias),
            };
            NumAnimation.KeyFrames.Add(new DiscreteStringKeyFrame()
            {
                Value = IndFromAlgorithm.x.ToString("F2") + "\n" + IndFromAlgorithm.y.ToString("F2"),
                KeyTime = KeyTime.FromPercent(1)
            });
            Number.BeginAnimation(TextBlock.TextProperty, NumAnimation);
        }
        private void Killing(object sender, EventArgs e)//функция визуализации отбора
        {
            
            BlockButton(false);
            //IndividualForKilling.Sort();
            for (int i = IndividualForKilling.Count - 1; i >= 0; i--)
            {
                string IndividualString = IndividualForKilling[i].x.ToString() + "/" + IndividualForKilling[i].y.ToString();//создаем ключ особи
                if (Population.Keys.Contains(IndividualString))
                {
                    Panel.Children.Remove(Population[IndividualString][0]);//удаляем особь с экрана
                    Population[IndividualString].RemoveAt(0);
                    if (Population[IndividualString].Count == 0)
                        Population.Remove(IndividualString);
                }
                //IndividualForKilling.RemoveAt(IndividualForKilling.Count - 1);
                if (Panel.Children.Count == 200)
                    break;
            }
            try
            {
                Best();
            }
            catch { }
            CanBeRun = true;
        }
        private void Best()
        {   while (true)
            {
                Individual Best = ge.Best();//находим индекс лучшей особи
                string BestString = Best.x.ToString() + "/" + Best.y.ToString();
                if (Population.Keys.Contains(BestString))
                {
                    Grid ch = Population[BestString][0];
                    Panel.Children.Remove(ch);
                    Panel.Children.Add(ch);//ставим эту особь на передний план
                    return;
                }
            }
        }
        private void BlockButton(bool IsBlock)//функция блокировки и разблокировки кнопок запуска алгоритма
        {
            btStarted.IsEnabled = !IsBlock;
            btStop.IsEnabled = !IsBlock;
        }
        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            TimerForNewGener.Stop();
            tbFunction.Focus();
        }//событие остановки алгоритма
        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            Reference reference = new Reference();
            reference.Owner = this;
            reference.ShowDialog();
        }//событие вызова спраки
        private void EndOfAnimation(object sender, EventArgs e)
        {
            TimerForNewGener.Stop();
            TimerForEnd.Stop();
            Grid g = (Grid)Panel.Children[Panel.Children.Count - 1];
            Ellipse El = ((Ellipse)g.Children[0]);
            El.Fill = new SolidColorBrush(Colors.Red);
            DoubleAnimation GridAnimation = new DoubleAnimation()
            {
                From = 40,
                To = 80,
                Duration = TimeSpan.FromSeconds(3),
                BeginTime = TimeSpan.FromSeconds(0)
            };
            g.BeginAnimation(Ellipse.WidthProperty, GridAnimation);
            g.BeginAnimation(Ellipse.HeightProperty, GridAnimation);
            DoubleAnimation ElWAnimation = new DoubleAnimation()
            {
                From = 35,
                To = 70,
                Duration = TimeSpan.FromSeconds(3),
                BeginTime = TimeSpan.FromSeconds(0)
            };
            El.BeginAnimation(Ellipse.WidthProperty, ElWAnimation);
            El.BeginAnimation(Ellipse.HeightProperty, ElWAnimation);
        }//событие окончания работы алгоритма
    }
}
