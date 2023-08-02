using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpPanelRenderer.Common;
using ExpPanelRenderer.Domain.Service.TextStorage;
using ExpPanelRenderer.Model;
using Xamarin.Essentials;

namespace ExpPanelRenderer.ViewModel;

public partial class MainViewModel : ObservableObject
{
    #region Field

    private readonly ITextStorageService _textStorage;

    private Dictionary<int, string> _colors;

    #endregion

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddVisualItemCommand))]
    private bool _isBusy;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SearchTextCommand))]
    private string _currentSearchText;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AddVisualItemCommand))]
    private string _currentEnterText;

    [ObservableProperty] private double _animationTime;

    [ObservableProperty] ObservableCollection<TestModel> items;

    [ObservableProperty] private object _selectedItem;

    #region Property

    public IEnumerable<string> TextItems { get; private set; }

    public string CurrentResultText { get; private set; }

    #endregion

    public MainViewModel(ITextStorageService textStorage)
    {
        _textStorage = textStorage ?? throw new ArgumentNullException(nameof(textStorage));

        items = new ObservableCollection<TestModel>();

        // System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        AnimationTime = 0.5;

        _colors = new Dictionary<int, string>
        {
            {1, "00FF00"},
            {2, "00FFFF"},
            {3, "f0d1a0"},
            {4, "FFA500"},
            {5, "eaeaea"}
        };
    }

    #region Can Execute

    private bool CanAddVisualItem => !IsBusy;
    private bool CanGetItems => !IsBusy;

    private bool CanSearch => !string.IsNullOrEmpty(CurrentSearchText) && !IsBusy;

    #endregion

    #region Command

    [RelayCommand]
    async Task GetAllTextAsync()
    {
        try
        {
            IsBusy = true;

            TextItems = await _textStorage.GetAllText();

            var index = new Random().Next(1, 3);

            Items.Add(new TestModel(Items.Count, TextItems.ToList()[0], ColorConverters.FromHex(_colors[index])));
        }
        catch (Exception e)
        {
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSearch))]
    async Task SearchTextAsync(object parameter)
    {
        try
        {
            IsBusy = true;

            var substring = parameter.ToString();

            // if (!string.IsNullOrEmpty(CurrentResultText))
            // {
            //     if (!CurrentResultText.Equals(substring))
            //     {
            //         CurrentResultText = string.Empty;
            //     }
            // }

            var res = CurrentResultText;

            CurrentResultText = await _textStorage.SearchText(substring, res);

            var equalItem = Items.FirstOrDefault(i => !string.IsNullOrEmpty(i.Text) && i.Text.Equals(CurrentResultText));

            if (equalItem != null)
            {
                if (SelectedItem == null)
                {
                    SelectedItem = equalItem;
                }
                else
                {
                    if (!new TestEqComparer().Equals((TestModel) SelectedItem, equalItem))
                    {
                        SelectedItem = equalItem;
                    }
                }
            }
        }
        catch (Exception e)
        {
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanAddVisualItem))]
    async Task AddVisualItemAsync()
    {
        try
        {
            IsBusy = true;

            Color color;

            var textItems = TextItems.ToList();
            var index = new Random().Next(1, textItems.Count);

            if (items.Any())
            {
                var prevItem = items.Last();
                color = GenerateColor(prevItem.Color);
            }
            else
            {
                color = ColorConverters.FromHex(_colors[index]);
            }

            Items.Add(new TestModel(Items.Count(), CurrentEnterText, color));

            _textStorage.AddText(CurrentEnterText);
        }
        catch (Exception e)
        {
            // ignored
        }
        finally
        {
            CurrentEnterText = string.Empty;
        }
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

    [RelayCommand]
    Task RemoveAsync(object parameter)
    {
        // Debug.WriteLine("RemoveAsync");
        try
        {
            IsBusy = true;

            var currentItem = Items.FirstOrDefault(x => x.Equals(parameter));

            if (currentItem != null)
            {
                if (SelectedItem != null && SelectedItem.Equals(currentItem))
                {
                    SelectedItem = null;
                }

                _textStorage.RemoveText(currentItem.Text);

                Items.Remove(currentItem);
            }
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Method

    private Color GenerateColor(Color comparedColor)
    {
        try
        {
            var hex = _colors[new Random().Next(1, _colors.Count)];
            var generateColor = ColorConverters.FromHex(hex);

            return !generateColor.Equals(comparedColor) ? generateColor : GenerateColor(comparedColor);
        }
        catch (Exception)
        {
            return Color.Aqua;
        }
    }

    #endregion
}