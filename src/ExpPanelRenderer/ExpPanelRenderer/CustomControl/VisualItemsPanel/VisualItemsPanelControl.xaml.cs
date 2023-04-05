using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpPanelRenderer.Common;
using ExpPanelRenderer.CustomControl.VisualItem;
using ExpPanelRenderer.Model;
using ExpPanelRenderer.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ExpPanelRenderer.CustomControl.VisualItemsPanel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisualItemsPanelControl : ContentView
    {
        #region Field

        public ObservableCollection<VisualItemControl> items { get; set; }

        private Dictionary<VisualItemControl, double> hiddenItems;

        private double currentWidth;

        #endregion

        #region BindableProperty

        public static readonly BindableProperty AnimationTimeProperty = BindableProperty.Create(
            propertyName: nameof(AnimationTime),
            typeof(double),
            typeof(VisualItemsPanelControl),
            defaultValue: 1.0);

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
                        control.AddVisualItem(e.NewItems[0]);
                    }
                    else
                    {
                        control.items.Clear();
                    }

                    control.ItemsCount = control.items.Count;
                }
            }

            if (newvalue is INotifyCollectionChanged newValue)
            {
                newValue.CollectionChanged += ValueOnCollectionChanged;

                if (bindable is VisualItemsPanelControl control)
                {
                    foreach (var item in (ICollection) newValue)
                    {
                        control.AddVisualItem(item);
                    }

                    control.ItemsCount = control.items.Count;
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

            hiddenItems = new Dictionary<VisualItemControl, double>();

            items = new ObservableCollection<VisualItemControl>();

            items.CollectionChanged += ItemsOnCollectionChanged;

            PART_Root_Stack.LayoutChanged += PART_Root_StackOnLayoutChanged;
        }

        #endregion

        #region Method

        private void AddVisualItem(object dataContext)
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
                WidthRequest = currentWidth
            };

            item.SetBinding(VisualItemControl.AnimationTimeProperty, animationTimeBinding);

            items.Add(item);
        }

        private void HiddenItems()
        {
            hiddenItems.Clear();

            foreach (var item in items)
            {
                item.IsActive = false;
            }

            var currentParentHeight = PART_Root_Grid.Height;
            var currentLayoutHeight = PART_Root_Stack.Height;
            currentWidth = PART_Root_Stack.Width;

            if ((currentParentHeight < 0) && !items.Any()) return;

            var maxIndex = items.Max(x => items.IndexOf(x));
            var maxPowValue = currentLayoutHeight - items.FirstOrDefault()!.Height;

            var sourcePow = Math.Pow(maxPowValue, (1.0 / (maxIndex + 1)));

            var maxItem = items[maxIndex];

            maxItem.WidthRequest = 250;

            hiddenItems[maxItem] = maxPowValue;

            var searchItems = items.Where(x => !((TestModel) x.BindingContext).Id.Equals(((TestModel) maxItem.BindingContext).Id));

            foreach (var item in searchItems)
            {
                var pow = searchItems.IndexOf(item) + 1;
                var powValue = Math.Pow(sourcePow, pow);
                // item.WidthRequest = currentWidth;
                hiddenItems[item] = powValue;
            }
        }

        private Task Animate()
        {
            var data = hiddenItems.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            var tasks = new List<Task>();

            var last = data.Last();

            var maxIndex = data.Max(x => data.IndexOf(x));

            foreach (var item in data)
            {
                item.Key.TranslationY = 0;
                item.Key.WidthRequest = currentWidth;
                // item.Key.IsActive = false;

                // might use Child Animation
                //var animation = new Animation(v => item.Key.TranslateTo(0, item.Value, 3000));

                tasks.Add(item.Key.TranslateTo(0, item.Value, (uint) TimeSpan.FromSeconds(AnimationTime).TotalMilliseconds));

                if (((TestModel) item.Key.BindingContext).Id.Equals(((TestModel) last.Key.BindingContext).Id))
                {
                    item.Key.IsActive = true;
                }
            }

            return Task.WhenAll(tasks);
        }

        #endregion

        #region Event Handler

        private async void PART_Root_StackOnLayoutChanged(object sender, EventArgs e)
        {
            try
            {
                var count = items.Count;

                if (count > 0)
                {
                    HiddenItems();
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

                    PART_Root_Stack.Children.Add((VisualItemControl) view);
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Reset:
                {
                    if (PART_Root_Stack.Children.Any())
                    {
                        PART_Root_Stack.Children.Clear();
                    }

                    break;
                }
            }
        }

        #endregion
    }
}