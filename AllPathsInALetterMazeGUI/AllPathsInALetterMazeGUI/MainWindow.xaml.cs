using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AllPathsInALetterMazeGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Maze maze;
        private Grid MazeGrid;
        private StackPanel PathsPanel;
        private const string FileInputBoxIdleText = "Browse for file...";
        private const string RandomMazeFilePath = "random.in";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReadMazeFromFile(string filePath)
        {
            if (maze == null)
            {
                GeneralGrid.Children.Clear();
                GeneralGrid.ColumnDefinitions.Add(new ColumnDefinition());

                MazeGrid = new Grid();
                MazeGrid.ShowGridLines = true;

                GeneralGrid.Children.Add(MazeGrid);
                Grid.SetColumn(MazeGrid, 0);

                maze = new Maze(filePath);
                for (int row = 0; row < maze.maze.GetLength(0); row++)
                {
                    MazeGrid.RowDefinitions.Add(new RowDefinition());
                }
                for (int col = 0; col < maze.maze.GetLength(1); col++)
                {
                    MazeGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int row = 0; row < maze.maze.GetLength(0); row++)
                {
                    for (int col = 0; col < maze.maze.GetLength(1); col++)
                    {
                        TextBlock letter = new TextBlock();
                        letter.Text = maze.maze[row, col].Value.ToString();
                        letter.HorizontalAlignment = HorizontalAlignment.Center;
                        letter.VerticalAlignment = VerticalAlignment.Center;
                        letter.FontSize = 25;
                        MazeGrid.Children.Add(letter);
                        Grid.SetRow(letter, row);
                        Grid.SetColumn(letter, col);
                    }
                }
                Button findPathsButton = new Button();
                findPathsButton.Content = "Find minimal paths to exits";
                findPathsButton.Height = 30;
                findPathsButton.Click += new RoutedEventHandler(FindPathsButton_Click);
                GeneralGrid.Children.Add(findPathsButton);
                Grid.SetColumn(findPathsButton, 1);
            }
        }

        private void FindPathsButton_Click(object sender, RoutedEventArgs e)
        {
            if (maze != null)
            {
                GeneralGrid.Children.Remove((Button)sender);

                

                ScrollViewer scroll = new ScrollViewer();
                scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                GeneralGrid.Children.Add(scroll);
                Grid.SetColumn(scroll, 1);
                

                PathsPanel = new StackPanel();
                scroll.Content = PathsPanel;

                TextBlock description = new TextBlock();
                description.Text = "All paths to exits:";
                description.Foreground = Brushes.Gray;
                description.FontSize = 10;
                description.Padding = new Thickness(20d, 0d, 0d, 0d);
                PathsPanel.Children.Add(description);

                List<Path> paths = maze.FindAllPaths();
                description.Text += " " + paths.Count.ToString();
                for (int i = 0; i < paths.Count; i++)
                {
                    Path path = paths[i];
                    TextBlock pathToText = new TextBlock();
                    pathToText.Text = path.ToString();
                    pathToText.Padding = new Thickness(20d, 0, 0, 0);
                    pathToText.FontSize = 30;
                    pathToText.MouseEnter += new MouseEventHandler((text, events) => pathToText_MouseEnter(text, events, path));
                    pathToText.MouseLeave += new MouseEventHandler((text, events) => pathToText_MouseLeave(text, events, path));
                    PathsPanel.Children.Add(pathToText);
                }
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog =
                new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".in";
            dialog.Filter = "Specific maze files |*.in";

            Nullable<bool> result = dialog.ShowDialog();

            // Get the selected file and send it to a textbox
            string filePath = FilePathBox.Text;
            if (result == true)
            {
                filePath = dialog.FileName;
                FilePathBox.Text = filePath;
            }
            ReadMazeFromFile(filePath);
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            char[] letters = new char[26];
            for (int i = 'a'; i <= 'z'; i++)
            {
                letters[i-(int)'a'] = (char)i;
            }
            int size = rand.Next(2, 10); // note that in a 10 x 10 maze there can thousands of exits with the current letter load facotr of 65%, if I reduce the letters there won't be so many exits or reduce the size if it's slow
            StreamWriter writer = new StreamWriter(RandomMazeFilePath);
            using (writer)
            {
                List<char> charsToInsert = new List<char>();
                for (int i = 0; i < size * size - 1; i++)
                {
                    bool addLetter = rand.Next() % 2 == 0 || (rand.Next() % 2 == 0 && rand.Next() % 2 == 0); // that's actually kinda clever - gives us about 65% chance of having a letter, which is a good balance
                    if (addLetter)
                    {
                        charsToInsert.Add(letters[rand.Next(0, letters.Length)]);
                    }
                    else
                    {
                        charsToInsert.Add('#');
                    }
                }
                charsToInsert.Insert(rand.Next(0, charsToInsert.Count), '*');
                writer.WriteLine(size);
                for (int row = 0; row < size; row++)
                {
                    for (int col = 0; col < size; col++)
                    {
                        char randomChar = charsToInsert[0];
                        charsToInsert.Remove(randomChar);
                        writer.Write(randomChar);
                        if (col < size - 1)
                        {
                            writer.Write(" ");
                        }
                    }
                    writer.WriteLine();
                }
            }
            ReadMazeFromFile(RandomMazeFilePath);
        }

        private void pathToText_MouseEnter(object sender, MouseEventArgs e, Path path)
        {
            TextBlock _sender = (TextBlock)sender;
            _sender.FontSize = 40;
            _sender.Foreground = Brushes.Red;
            foreach (Cell cell in path.Cells)
            {
                TextBlock letter = MazeGrid.Children.Cast<TextBlock>()
                    .First(el => Grid.GetRow(el) == cell.Row && Grid.GetColumn(el) == cell.Column);
                letter.Foreground = Brushes.Red;
            }
        }


        private void pathToText_MouseLeave(object sender, MouseEventArgs e, Path path)
        {
            TextBlock _sender = (TextBlock)sender;
            _sender.FontSize = 30;
            _sender.Foreground = Brushes.Black;
            foreach (Cell cell in path.Cells)
            {
                TextBlock letter = MazeGrid.Children.Cast<TextBlock>()
                    .First(el => Grid.GetRow(el) == cell.Row && Grid.GetColumn(el) == cell.Column);
                letter.Foreground = Brushes.Black;
            }
        }





    }
}
