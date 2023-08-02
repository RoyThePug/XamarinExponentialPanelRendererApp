using System;
using System.Windows.Input;
using ExpPanelRenderer.CustomControl.RemoveItem;
using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.VisualItem
{
    public class VisualItemControl : ContentView
    {
        #region Field

        private RemoveVisualControl? _removeVisual;

        #endregion

        #region Readonly Bindable Property

        public static readonly BindablePropertyKey IsProxyKey = BindableProperty.CreateReadOnly(
            nameof(IsProxy),
            typeof(bool),
            typeof(VisualItemControl), false);

        public static readonly BindableProperty IsProxyProperty = IsProxyKey.BindableProperty;

        public bool IsProxy
        {
            get => (bool) GetValue(IsProxyProperty);
            private set => SetValue(IsProxyKey, value);
        }

        #endregion

        #region Bindable Property

        public static readonly BindableProperty IsEditableProperty = BindableProperty.Create(
            propertyName: nameof(IsEditable),
            returnType: typeof(bool),
            declaringType: typeof(VisualItemControl),
            defaultValue: false);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(VisualItemControl),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(VisualItemControl),
            defaultValue: Color.Blue,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty AnimationTimeProperty = BindableProperty.Create(
            propertyName: nameof(Animation),
            returnType: typeof(double),
            declaringType: typeof(VisualItemControl),
            defaultValue: 1.0);

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            propertyName: nameof(IsSelected),
            returnType: typeof(bool),
            declaringType: typeof(VisualItemControl),
            defaultValue: false,
            propertyChanged: IsSelectedChanged);

        public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
            propertyName: nameof(IsActive),
            returnType: typeof(bool),
            declaringType: typeof(VisualItemControl),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay);

        public static readonly BindableProperty RemoveCommandProperty = BindableProperty.Create(
            propertyName: nameof(RemoveCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(VisualItemControl));

        public bool IsEditable
        {
            get => (bool) GetValue(IsEditableProperty);
            set
            {
                if (IsEditable != value)
                {
                    SetValue(IsEditableProperty, value);
                }
            }
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set
            {
                if (Text != value)
                {
                    SetValue(TextProperty, value);
                }
            }
        }

        public Color TextColor
        {
            get => (Color) GetValue(TextColorProperty);
            set
            {
                if (TextColor != value)
                {
                    SetValue(TextColorProperty, value);
                }
            }
        }

        public double AnimationTime
        {
            get => (double) GetValue(AnimationTimeProperty);
            set
            {
                if (AnimationTime != value)
                {
                    SetValue(AnimationTimeProperty, value);
                }
            }
        }

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set
            {
                if (IsSelected != value)
                {
                    SetValue(IsSelectedProperty, value);
                }
            }
        }

        public bool IsActive
        {
            get => (bool) base.GetValue(IsActiveProperty);
            set
            {
                if (IsActive != value)
                {
                    SetValue(IsActiveProperty, value);
                }
            }
        }

        public ICommand RemoveCommand
        {
            get => (ICommand) GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        #endregion

        #region Event

        public event EventHandler Remove;

        #endregion

        public VisualItemControl()
        {
        }

        protected override void OnApplyTemplate()
        {
            _removeVisual = GetTemplateChild("PartRemove") as RemoveVisualControl;

            if (_removeVisual != null)
            {
                _removeVisual.Clicked += RemoveVisualOnClicked;
            }
        }

        #region Event Handler

        private static void IsSelectedChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is VisualItemControl control)
            {
                if ((bool) newvalue)
                {
                    VisualStateManager.GoToState(control, "Test");
                }
                else
                {
                    VisualStateManager.GoToState(control, "Normal");
                }
            }
        }

        private void RemoveVisualOnClicked(object sender, EventArgs e)
        {
            Remove?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}