using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public ObservableCollection<VisualItemControl> Items { get; }

        private Dictionary<VisualItemControl, double> _hiddenItems;

        private bool _isLoaded;

        private double _currentWidth;

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
                    if (e.NewItems != null)
                    {
                        var visualItem = control.CreateVisualItem(e.NewItems[0]);
                        control.Items.Add(visualItem);
                    }
                    else
                    {
                        control.Items.Clear();
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

        #endregion

        #region Constructor

        public VisualItemsPanelControl()
        {
            InitializeComponent();

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

                var count = Items.Count;

                if (count > 0)
                {
                    _hiddenItems.Clear();

                    foreach (var item in Items)
                    {
                        item.IsActive = false;
                    }

                    CalculateItemsYTranslate();
                }

                ((MainViewModel) BindingContext).IsBusy = true;

                await Animate();
            }
            catch (Exception exception)
            {
            }
            finally
            {
                ((MainViewModel) BindingContext).IsBusy = false;
            }
        }

        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    var view = e.NewItems[0];

                    PartRootStack.Children.Add((VisualItemControl) view);
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                    break;
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
            //Add Is Busy Flag

            if (Items.Any())
            {
                var item = Items.Last();

                if (item != null)
                {
                    await TestAnimate(item);
                }
            }
        }

        #endregion

        #region Method

        private Task Animate()
        {
            var data = _hiddenItems.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            var tasks = new List<Task>();

            foreach (var item in data)
            {
                item.Key.TranslationY = 0;
                item.Key.WidthRequest = _currentWidth;

                // might use Child Animation
                //var animation = new Animation(v => item.Key.TranslateTo(0, item.Value, 3000));

                tasks.Add(item.Key.TranslateTo(0, item.Value, (uint) TimeSpan.FromSeconds(AnimationTime).TotalMilliseconds));
            }

            data.Keys.Last().IsActive = true;

            return Task.WhenAll(tasks);
        }

        private void CalculateItemsYTranslate()
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
                // item.WidthRequest = currentWidth;
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

            var itemsToRender = Items.Where(i => Items.IndexOf(i) > Items.IndexOf(currentItem));

            if (itemsToRender.Any())
            {
                var queueToRender = new Stack<VisualItemControl>(itemsToRender);

                do
                {
                    var itemToRender = queueToRender.Pop();
                    await TestAnimate(itemToRender);
                } while (queueToRender.Count > 0);
            }
        }

        private async Task TestAnimate(VisualItemControl visualItemControl)
        {
            var tasks = new List<Task>();

            var yEnd = visualItemControl.TranslationY + visualItemControl.Height;

            tasks.Add(visualItemControl.TranslateTo(0, yEnd, (uint) TimeSpan.FromSeconds(AnimationTime).TotalMilliseconds));

            var copyData = ((TestModel) visualItemControl.BindingContext).DeepCopy();

            var newItem = CreateVisualItem(visualItemControl.BindingContext);
            newItem.IsSelected = visualItemControl.IsSelected;

            _isLoaded = true;

            newItem.TranslationY = -newItem.Height;

            var yTranslateSource = _hiddenItems.OrderBy(x => x.Value).Select(x => x.Value);

            Items.Insert(0, newItem);

            _hiddenItems = Items.Where(x => !x.Equals(visualItemControl)).Zip(yTranslateSource, (visual, yTranslate) => new {visual, yTranslate})
                                .ToDictionary(x => x.visual, x => x.yTranslate);
            Items.Clear();

            foreach (var visual in _hiddenItems.Keys)
            {
                Items.Add(visual);
                tasks.Add(visual.TranslateTo(0, _hiddenItems[visual], (uint) TimeSpan.FromSeconds(AnimationTime).TotalMilliseconds));
            }

            visualItemControl.IsActive = false;

            Items.Add(visualItemControl);

            _hiddenItems.Keys.Last().IsActive = true;

            await Task.WhenAll(tasks);

            Items.Remove(visualItemControl);
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