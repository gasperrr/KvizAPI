using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Shapes;

namespace Kviz;

public partial class PoisciBesedePage : ContentPage
{
    public int Age { get; set; }
    Dictionary<(int, int), Label> cellLabels = new();
    Dictionary<string, List<(int row, int col)>> wordPositions = new();
    HashSet<string> foundWords = new();

    int GridSize = 25;

    public char[,] grid = new char[1,1];

    List<string> words = new() { "GASILEC", "POZAR", "CEV", "KRVAVITEV", "CELADA","POVELJNIK","NAPAD","ROCNIK" };
    Random rnd = new();

    int cellSize;

    public PoisciBesedePage()
    {
        InitializeComponent();
        
    }
    protected override void OnAppearing()
    {
        InitializeGrid();
        PlaceWords();
        FillEmptyCells();
        DrawGrid();
        WordListLabel.Text = $"Find words: {string.Join(", ", words)}";

        this.SizeChanged += OnPageSizeChanged;
        base.OnAppearing();
    }

    private void OnPageSizeChanged(object? sender, EventArgs e)
    {
        if (ZoomPanContainer == null)
            return;

        double sideLength = Math.Min(this.Width, this.Height);

        // Apply square size to ZoomPanContainer
        ZoomPanContainer.WidthRequest = sideLength;
        ZoomPanContainer.HeightRequest = sideLength;
        // Apply clipping so the content doesn't go outside
        ZoomPanContainer.Clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, sideLength, sideLength)
        };

        // Unsubscribe so this runs only once
        this.SizeChanged -= OnPageSizeChanged;
    }

    void InitializeGrid()
    {
        if (Age == 1)
        {
            GridSize = 10;
        }
        else if (Age == 2)
        {
            GridSize = 17;
        }
        else if (Age == 3)
        {
            GridSize = 25;
        }
        grid = new char[GridSize, GridSize];

        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        cellSize = (int)screenWidth / GridSize;

        PuzzleGrid.RowDefinitions.Clear();
        PuzzleGrid.ColumnDefinitions.Clear();
        PuzzleGrid.Children.Clear();

        for (int i = 0; i < GridSize; i++)
        {
            PuzzleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize) });
            PuzzleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
        }
        PuzzleGrid.HorizontalOptions = LayoutOptions.Center;
        PuzzleGrid.VerticalOptions = LayoutOptions.Center;
    }

    void DrawGrid()
    {
        PuzzleGrid.Children.Clear();
        cellLabels.Clear();

        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                var label = new Label
                {
                    Padding = 0,
                    Text = grid[row, col].ToString(),
                    FontSize = 14,
                    BackgroundColor = Colors.Transparent,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                var border = new Border
                {
                    Padding = 0,
                    Margin = 0,
                    Stroke = Colors.Black,
                    StrokeThickness = 2,
                    BackgroundColor = Colors.DarkGrey,
                    Content = label
                };

                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);
                PuzzleGrid.Children.Add(border);

                cellLabels[(row, col)] = label;
            }
        }
        PuzzleGrid.WidthRequest = GridSize * cellSize;
        PuzzleGrid.HeightRequest = GridSize * cellSize;
    }

    void FillEmptyCells()
    {
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (grid[row, col] == '\0')
                {
                    grid[row, col] = (char)('A' + rnd.Next(0, 26));
                }
            }
        }
    }

    void PlaceWords()
    {
        foreach (var word in words)
        {
            bool placed = false;
            for (int attempts = 0; attempts < 100 && !placed; attempts++)
            {
                int row = rnd.Next(GridSize);
                int col = rnd.Next(GridSize);
                var direction = GetRandomDirection();

                if (CanPlaceWord(word, row, col, direction))
                {
                    PlaceWord(word, row, col, direction);
                    placed = true;
                }
            }
        }
    }
    readonly (int dx, int dy)[] directionsAge1 = new (int, int)[]
    {
    (0, 1),   // right
    (1, 0),   // down
    (0, -1),  // left
    (-1, 0),  // up
    };

    readonly (int dx, int dy)[] directionsAge2 = new (int, int)[]
    {
    (0, 1),    // right
    (1, 0),    // down
    (0, -1),   // left
    (-1, 0),   // up
    (1, 1),    // down-right
    (-1, -1),  // up-left
    };

    readonly (int dx, int dy)[] directionsAge3 = new (int, int)[]
    {
    (0, 1),    // right
    (1, 0),    // down
    (0, -1),   // left
    (-1, 0),   // up
    (1, 1),    // down-right
    (-1, -1),  // up-left
    (1, -1),   // down-left
    (-1, 1),   // up-right
    };
    (int dx, int dy) GetRandomDirection()
    {
        return Age switch
        {
            1 => directionsAge1[rnd.Next(directionsAge1.Length)],
            2 => directionsAge2[rnd.Next(directionsAge2.Length)],
            3 => directionsAge3[rnd.Next(directionsAge3.Length)],
            _ => directionsAge1[0],
        };
    }
   // (int dx, int dy) GetRandomDirection() => directions[rnd.Next(directions.Length)];

    bool CanPlaceWord(string word, int row, int col, (int dx, int dy) dir)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int r = row + i * dir.dy;
            int c = col + i * dir.dx;

            if (r < 0 || r >= GridSize || c < 0 || c >= GridSize)
                return false;

            if (grid[r, c] != '\0' && grid[r, c] != word[i])
                return false;
        }
        return true;
    }

    void PlaceWord(string word, int row, int col, (int dx, int dy) dir)
    {
        var positions = new List<(int, int)>();

        for (int i = 0; i < word.Length; i++)
        {
            int r = row + i * dir.dy;
            int c = col + i * dir.dx;
            grid[r, c] = word[i];
            positions.Add((r, c));
        }

        wordPositions[word.ToUpper()] = positions;
    }

    void OnCheckWordClicked(object sender, EventArgs e)
    {
        string input = WordEntry.Text?.ToUpper().Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input))
            return;

        if (foundWords.Contains(input))
        {
            WordEntry.Text = string.Empty;
            return;
        }

        if (wordPositions.TryGetValue(input, out var positions))
        {
            foreach (var (r, c) in positions)
            {
                cellLabels[(r, c)].BackgroundColor = Colors.LightGreen;
            }

            foundWords.Add(input);
            WordListLabel.Text = $"Words: {string.Join(", ", words.Select(w => foundWords.Contains(w) ? $" {w}" : w))}";
        }


        WordEntry.Text = string.Empty;
    }
}
