using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ExpPanelRenderer.Common;
using ExpPanelRenderer.CustomControl.Common;
using ExpPanelRenderer.CustomControl.Service;
using ExpPanelRenderer.CustomControl.VisualItem;
using ExpPanelRenderer.Model;
using ExpPanelRenderer.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ExpPanelRenderer.CustomControl.VisualItemsPanel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisualItemsPanelControl
    {
        #region Field

        private readonly IRenderService _renderService;

        private RenderMode _renderMode;

        public ObservableCollection<VisualItemControl> Items { get; private set; }

        public Dictionary<VisualItemControl, double> _hiddenItems;

        private bool _isLoaded;

        public double _currentWidth;

        private bool _scrollMode = false;

        #endregion

        #region Readonly Bindable Property

        public static readonly BindablePropertyKey CanScrollKey = BindableProperty.CreateReadOnly(
            nameof(CanScroll),
            typeof(bool),
            typeof(VisualItemsPanelControl), false);

        public static readonly BindableProperty CanScrollProperty = CanScrollKey.BindableProperty;

        public bool CanScroll
        {
            get => (bool) GetValue(CanScrollProperty);
            private set => SetValue(CanScrollKey, value);
        }

        #endregion

        #region Bindable Property

        public static readonly BindableProperty UpCommandProperty = BindableProperty.Create(
            propertyName: nameof(UpCommand),
            typeof(ICommand),
            typeof(VisualItemsPanelControl)
        );

        public ICommand UpCommand
        {
            get => (ICommand) GetValue(UpCommandProperty);
            set => SetValue(UpCommandProperty, value);
        }

        public static readonly BindableProperty DownCommandProperty = BindableProperty.Create(
            propertyName: nameof(DownCommand),
            typeof(ICommand),
            typeof(VisualItemsPanelControl));

        public ICommand DownCommand
        {
            get => (ICommand) GetValue(DownCommandProperty);
            set => SetValue(DownCommandProperty, value);
        }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
            propertyName: nameof(SelectedItem),
            typeof(object),
            typeof(VisualItemsPanelControl),
            defaultValue: null,
            propertyChanged: SelectedItemChanged
        );

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static readonly BindableProperty AnimationTimeProperty = BindableProperty.Create(
            propertyName: nameof(AnimationTime),
            typeof(double),
            typeof(VisualItemsPanelControl),
            defaultValue: 3.0);


        public double AnimationTime
        {
            get => (double) GetValue(AnimationTimeProperty);
            set => SetValue(AnimationTimeProperty, value);
        }

        public static readonly BindableProperty ItemsCountProperty = BindableProperty.Create(
            propertyName: nameof(ItemsCount),
            typeof(int),
            typeof(VisualItemsPanelControl));

        public int ItemsCount
        {
            get => (int) GetValue(ItemsCountProperty);
            set => SetValue(ItemsCountProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            propertyName: nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(VisualItemsPanelControl),
            null,
            BindingMode.OneWay,
            propertyChanged: OnItemsSourcePropertyChanged);

        public IEnumerable ItemsSource
        {
            get => (IEnumerable) GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            void ValueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (bindable is VisualItemsPanelControl control)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        var visualItem = control.CreateVisualItem(e.NewItems[0]);
                        control.Items.Add(visualItem);
                    }

                    if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        var visualItem = control.Items.FirstOrDefault(x => x.BindingContext.Equals(e.OldItems[0]));

                        if (visualItem != null)
                        {
                            control.Items.Remove(visualItem);
                        }
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        control.Items.Clear();
                        control.CanScroll = false;
                    }

                    control.ItemsCount = control.Items.Count;
                }
            }

            if (newvalue is INotifyCollectionChanged newValue)
            {
                newValue.CollectionChanged += ValueOnCollectionChanged;

                if (bindable is VisualItemsPanelControl control)
                {
                    foreach (var dataContext in (ICollection) newValue)
                    {
                        var visualItem = control.CreateVisualItem(dataContext);
                        control.Items.Add(visualItem);
                    }

                    control.ItemsCount = control.Items.Count;
                }
            }

            if (oldvalue is INotifyCollectionChanged oldValue)
            {
                oldValue.CollectionChanged -= ValueOnCollectionChanged;
            }
        }

        private static void SelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is VisualItemsPanelControl control)
            {
                var currentItem = control.Items.FirstOrDefault(i => i.BindingContext.Equals(newvalue));

                if (currentItem != null)
                {
                    foreach (var item in control.Items)
                    {
                        if (!item.BindingContext.Equals(currentItem.BindingContext))
                        {
                            item.IsSelected = false;
                        }
                    }

                    currentItem.IsSelected = true;

                    control.Test();
                }
            }
        }

        #endregion

        #region Event

        public event EventHandler RemoveItem;

        #endregion

        #region Constructor

        public VisualItemsPanelControl()
        {
            InitializeComponent();

            _renderService = new RenderService(this);

            _hiddenItems = new Dictionary<VisualItemControl, double>();

            Items = new ObservableCollection<VisualItemControl>();

            Items.CollectionChanged += ItemsOnCollectionChanged;

            PartRootStack.LayoutChanged += PART_Root_StackOnLayoutChanged;
        }

        #endregion

        #region Event Handler

        private async void PART_Root_StackOnLayoutChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isLoaded)
                {
                    return;
                }

                ((MainViewModel) BindingContext).IsBusy = true;

                CanScroll = false;

                await _renderService.RenderItems();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (!_isLoaded)
                {
                    ((MainViewModel) BindingContext).IsBusy = false;

                    if (_hiddenItems.Count > 1)
                    {
                        CanScroll = true;
                    }

                    if (Items.Any())
                    {
                        Items.Last().IsEditable = true;
                    }
                }
            }
        }

        private async void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems[0] is VisualItemControl view)
                    {
                        view.Remove += ViewOnRemove;

                        if (_renderMode == RenderMode.Move)
                        {
                            PartRootStack.Children.Insert(0, view);
                        }
                        else
                        {
                            PartRootStack.Children.Add(view);
                        }
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems[0] is VisualItemControl view)
                    {
                        view.Remove -= ViewOnRemove;

                        PartRootStack.Children.Remove(view);

                        if (_scrollMode)
                        {
                            return;
                        }

                        try
                        {
                            _scrollMode = true;

                            _isLoaded = true;

                            CanScroll = false;

                            await TestRemove(view);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                         //   if (!_isLoaded)
                         //   {
                                if (_hiddenItems.Count > 1)
                                {
                                    CanScroll = true;
                                }

                                if (Items.Any())
                                {
                                    _hiddenItems.Keys.Last().IsActive = true;
                                   // _hiddenItems.Keys.Last().IsEditable = true;
                                    
                                    // Items.Last().IsActive = true;
                                    //
                                     Items.Last().IsEditable = true;
                                }
                        //    }
                            
                            ((MainViewModel) BindingContext).IsBusy = false;
                            
                            _scrollMode = false;

                           _isLoaded = false;
                        }
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    if (PartRootStack.Children.Any())
                    {
                        PartRootStack.Children.Clear();
                    }

                    _isLoaded = false;

                    break;
                }
            }
        }

        private async void ButtonDownClicked(object sender, EventArgs e)
        {
            if (Items.Count > 1)
            {
                try
                {
                    _scrollMode = true;

                    _isLoaded = true;

                    _renderMode = RenderMode.Move;

                    CanScroll = false;

                    var item = Items.Last();

                    if (item != null)
                    {
                        await TestAnimate(item);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    _scrollMode = false;

                    _isLoaded = false;

                    _renderMode = RenderMode.Normal;

                    CanScroll = true;

                    Items.Last().IsEditable = true;
                }
            }
        }

        private void ViewOnRemove(object sender, EventArgs e)
        {
            if (sender is VisualItemControl visual)
            {
                RemoveItem?.Invoke(visual, new CustomEventArgs(visual.BindingContext));
            }
        }

        #endregion

        #region Method

        public void CalculateItemsYTranslate()
        {
            var currentParentHeight = PartRootGrid.Height;
            var currentLayoutHeight = PartRootStack.Height;
            _currentWidth = PartRootStack.Width;

            if ((currentParentHeight < 0) && !Items.Any()) return;

            var maxIndex = Items.Max(x => Items.IndexOf(x));
            var maxPowValue = currentLayoutHeight - Items.FirstOrDefault()!.Height;

            var sourcePow = Math.Pow(maxPowValue, (1.0 / (maxIndex + 1)));

            var maxItem = Items[maxIndex];

            maxItem.WidthRequest = 250;

            _hiddenItems[maxItem] = maxPowValue;

            var searchItems = Items.Where(x => !((TestModel) x.BindingContext).Id.Equals(((TestModel) maxItem.BindingContext).Id));

            foreach (var item in searchItems)
            {
                var pow = searchItems.IndexOf(item) + 1;
                var powValue = Math.Pow(sourcePow, pow);

                _hiddenItems[item] = powValue;
            }
        }

        private async void Test()
        {
            if (!Items.Any() || SelectedItem is null)
            {
                return;
            }

            var currentItem = Items.FirstOrDefault(i => i.BindingContext.Equals(SelectedItem));

            if (currentItem != null)
            {
                var itemsToRender = Items.Where(i => Items.IndexOf(i) > Items.IndexOf(currentItem));

                if (itemsToRender.Any())
                {
                    var queueToRender = new Stack<VisualItemControl>(itemsToRender);

                    try
                    {
                        CanScroll = false;

                        _scrollMode = true;

                        do
                        {
                            var itemToRender = queueToRender.Pop();
                            await TestAnimate(itemToRender);
                        } while (queueToRender.Count > 0);

                        currentItem.IsEditable = true;
                    }
                    catch (Exception e)
                    {
                    }
                    finally
                    {
                        CanScroll = true;

                        _scrollMode = false;
                    }
                }
            }
        }

        private async Task TestAnimate(VisualItemControl visualItemControl)
        {
            var tasks = new List<Task>();

            var yEnd = visualItemControl.TranslationY + visualItemControl.Height;

            tasks.Add(visualItemControl.TranslateTo(0, yEnd, (uint) TimeSpan.FromSeconds(AnimationTime).TotalMilliseconds));

            var newItem = CreateVisualItem(visualItemControl.BindingContext);

            newItem.IsSelected = visualItemControl.IsSelected;

            var yTranslateSource = _hiddenItems.OrderBy(x => x.Value).Select(x => x.Value);

            Items.Insert(0, newItem);

            _hiddenItems = Items.Where(x => !x.Equals(visualItemControl)).Zip(yTranslateSource, (visual, yTranslate) => new {visual, yTranslate})
                                .ToDictionary(x => x.visual, x => x.yTranslate);

            foreach (var visual in _hiddenItems.Keys)
            {
                tasks.Add(visual.TranslateTo(0, _hiddenItems[visual], (uint) TimeSpan.FromSeconds(AnimationTime).TotalMilliseconds));
            }

            visualItemControl.IsActive = false;
            visualItemControl.IsEditable = false;

            _hiddenItems.Keys.Last().IsActive = true;

            await Task.WhenAll(tasks);

            Items.Remove(visualItemControl);
        }

        private async Task TestRemove(VisualItemControl visualItemControl)
        {
            var tasks = new List<Task>();

            _hiddenItems.Remove(visualItemControl);
            
            CalculateItemsYTranslate();

            foreach (var visual in Items)
            {
                visual.WidthRequest = 332;
                
                tasks.Add(visual.TranslateTo(0, _hiddenItems[visual], (uint) TimeSpan.FromSeconds(AnimationTime).TotalMilliseconds));
            }
            
            _hiddenItems.Keys.Last().IsActive = true;

            await Task.WhenAll(tasks);
        }

        private VisualItemControl CreateVisualItem(object dataContext)
        {
            var animationTimeBinding = new Binding()
            {
                Path = nameof(AnimationTime),
                Source = this
            };

            var item = new VisualItemControl()
            {
                IsActive = false,
                BindingContext = dataContext,
                WidthRequest = _currentWidth
            };

            item.SetBinding(VisualItemControl.AnimationTimeProperty, animationTimeBinding);

            return item;
        }

        #endregion
    }
}