using CommunityToolkit.Mvvm.ComponentModel;

namespace GameOfLifeMvvm.ViewModels;

public partial class Cell : ViewModelBase
{
    public int X { get; }
    public int Y { get; }

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsYoung), nameof(IsOld))]
    private int age;

    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsYoung), nameof(IsOld), nameof(IsDead))]
    private bool isAlive;

    public bool IsYoung => IsAlive && Age < 2;
    public bool IsOld => IsAlive && !IsYoung;
    public bool IsDead => !IsAlive;

    public Cell(int x, int y, int age, bool isAlive)
    {
        X = x;
        Y = y;
        Age = age;
        IsAlive = isAlive;
    }
}

internal struct OptimizedCell
{
    public bool IsAlive;
    public int Age;
}