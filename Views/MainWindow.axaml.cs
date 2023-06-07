using System.Linq;
using Avalonia.Controls;
using GameOfLifeMvvm.ViewModels;

namespace GameOfLifeMvvm.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var vm = new MainWindowViewModel();
        DataContext = vm;
        
        // need to bind this way to use content control on all cells
        Grid.Children.AddRange(vm.All.Select(cell => new
            ContentControl
            {
                Content = cell
            })
        );
    }

    protected override void OnDataContextEndUpdate()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            // only run once the 
            vm.ToggleRun(state: true);
        }
    }
}