using System;
using System.Collections.Generic;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GameOfLifeMvvm.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Timer timer = new(200)
    {
        Enabled = false,
    };

    [ObservableProperty, NotifyPropertyChangedFor(nameof(GenerationsTitle))]
    private int generations;
    
    public string GenerationsTitle => $"Game of Life ({Rows}, {Columns}) - Generations : {Generations}";
    
    [ObservableProperty] 
    string toggleText;
    
    public int Rows { get; }
    public int Columns { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public Cell[][] Cells { get; set; }
    private readonly OptimizedCell[][] nextGeneration;

    public MainWindowViewModel()
        : this(150, 150)
    {
    }

    public MainWindowViewModel(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Cells = new Cell[rows][];
        nextGeneration = new OptimizedCell[rows][];

        for (var row = 0; row < rows; row++)
        {
            Cells[row] = new Cell[columns];
            nextGeneration[row] = new OptimizedCell[columns];

            for (var column = 0; column < columns; column++)
            {
                Cells[row][column] = new Cell(row, column, 0, isAlive: Random.Shared.NextDouble() > 0.8);
            }
        }
        
        timer.Elapsed += (_, _) => Update();
    }

    [RelayCommand]
    private void Randomize()
    {
        var timerEnabled = timer.Enabled;
        timer.Enabled = false;
        Generations = 0;
        
        var rows = Rows;
        var columns = Columns;
        
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                Cells[row][column].IsAlive = Random.Shared.NextDouble() > 0.8;
                Cells[row][column].Age = 0;
            }
        }

        timer.Enabled = timerEnabled;
    }

    public void Update()
    {
        Generations++;
        
        var alive = false;
        var age = 0;
        
        var rows = Rows;
        var columns = Columns;

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                CalculateNextGeneration(i, j, ref alive, ref age);
                nextGeneration[i][j].IsAlive = alive;
                nextGeneration[i][j].Age = age;
            }
        }

        UpdateToNextGeneration();
    }
    
    private void CalculateNextGeneration(int row, int column, ref bool isAlive, ref int age)
    {
        isAlive = Cells[row][column].IsAlive;
        age = Cells[row][column].Age;

        var count = CountNeighbors(row, column);

        if (isAlive && count < 2)
        {
            isAlive = false;
            age = 0;
        }

        if (isAlive && count is 2 or 3)
        {
            Cells[row][column].Age++;
            isAlive = true;
            age = Cells[row][column].Age;
        }

        if (isAlive && count > 3)
        {
            isAlive = false;
            age = 0;
        }

        if (!isAlive && count == 3)
        {
            isAlive = true;
            age = 0;
        }
    }
    
    private void UpdateToNextGeneration()
    {
        var rows = Rows;
        var columns = Columns;
        
        for (var i = 0; i < rows; i++)
        for (var j = 0; j < columns; j++)
        {
            Cells[i][j].IsAlive = nextGeneration[i][j].IsAlive;
            Cells[i][j].Age = nextGeneration[i][j].Age;
        }
    }
    
    private int CountNeighbors(int i, int j)
    {
        var count = 0;

        if (i != Rows - 1 && Cells[i + 1][j].IsAlive) count++;
        if (i != Rows - 1 && j != Columns - 1 && Cells[i + 1][j + 1].IsAlive) count++;
        if (j != Columns - 1 && Cells[i][j + 1].IsAlive) count++;
        if (i != 0 && j != Columns - 1 && Cells[i - 1][j + 1].IsAlive) count++;
        if (i != 0 && Cells[i - 1][j].IsAlive) count++;
        if (i != 0 && j != 0 && Cells[i - 1][j - 1].IsAlive) count++;
        if (j != 0 && Cells[i][j - 1].IsAlive) count++;
        if (i != Rows - 1 && j != 0 && Cells[i + 1][j - 1].IsAlive) count++;

        return count;
    }

    public IEnumerable<Cell> All
    {
        get
        {
            var rows = Rows;
            var columns = Columns;
            
            for (var r = 0; r < rows - 1; r++)
            {
                for (var c = 0; c < columns - 1; c++)
                {
                    yield return Cells[r][c];
                }
            }
        }
    }

    [RelayCommand]
    public void ToggleRun(bool? state = null)
    {
        timer.Enabled = state ?? !timer.Enabled;
        ToggleText = timer.Enabled ? "⏸ Pause" : "▶ Run";
    }

}


