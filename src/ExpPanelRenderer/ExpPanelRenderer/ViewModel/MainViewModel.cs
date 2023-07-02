using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpPanelRenderer.Model;
using Xamarin.Essentials;

namespace ExpPanelRenderer.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddVisualItemCommand))]
    private bool isBusy;

    [ObservableProperty] ObservableCollection<TestModel> items;

    private Dictionary<int, string> _colors;

    public MainViewModel()
    {
        items = new ObservableCollection<TestModel>();

        _colors = new Dictionary<int, string>
        {
            {1, "00FF00"},
            {2, "00FFFF"},
            {3, "f0d1a0"},
            {4, "FFA500"},
            {5, "eaeaea"}
        };

        var index = new Random().Next(1, 6);
        Items.Add(new TestModel(Items.Count(), _colors[index], ColorConverters.FromHex(_colors[index])));
    }

    private bool CanAddVisualItem => !IsBusy;

    #region Command

    [RelayCommand(CanExecute = nameof(CanAddVisualItem))]
    Task AddVisualItemAsync()
    {
        try
        {
            IsBusy = true;

            Color color;

            if (items.Any())
            {
                var prevItem = items.Last();
                color = GenerateColor(prevItem.Color);
            }
            else
            {
                var index = new Random().Next(1, 5);
                color = ColorConverters.FromHex(_colors[index]);
            }

            Items.Add(new TestModel(Items.Count(), color.Name, color));
        }
        catch (Exception e)
        {
            // ignored
        }

        finally
        {
            //   IsBusy = false;
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    Task ClearItemsAsync()
    {
        Items.Clear();

        return Task.CompletedTask;
    }

    [RelayCommand]
    Task DownAsync()
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Method

    private Color GenerateColor(Color comparedColor)
    {
        try
        {
            var hex = _colors[new Random().Next(1, 6)];
            var generateColor = ColorConverters.FromHex(hex);

            return !generateColor.Equals(comparedColor) ? generateColor : GenerateColor(comparedColor);
        }
        catch (Exception e)
        {
            return Color.Aqua;
        }
    }

    #endregion
}