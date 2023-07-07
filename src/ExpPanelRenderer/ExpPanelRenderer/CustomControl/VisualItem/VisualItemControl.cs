using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.VisualItem
{
    public class VisualItemControl : ContentView
    {
        #region Bindable Property

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(VisualItemControl),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay);

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

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(VisualItemControl),
            defaultValue: Color.Blue,
            defaultBindingMode: BindingMode.OneWay);

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


        public static readonly BindableProperty AnimationTimeProperty = BindableProperty.Create(
            propertyName: nameof(Animation),
            returnType: typeof(double),
            declaringType: typeof(VisualItemControl),
            defaultValue: 1.0,
            propertyChanged: PropertyChanged);

        private static void PropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
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

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            propertyName: nameof(IsSelected),
            returnType: typeof(bool),
            declaringType: typeof(VisualItemControl),
            defaultValue: false,
            propertyChanged: IsSelectedChanged);

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

        public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
            propertyName: nameof(IsActive),
            returnType: typeof(bool),
            declaringType: typeof(VisualItemControl),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsActivePropertyChanged);

        private static void IsActivePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            // if (!(bool) newvalue)
            // {
            //      VisualStateManager.GoToState((VisualElement)bindable, "Normal");
            // }
            // else
            // {
            //     VisualStateManager.GoToState((VisualElement)bindable, "Test");
            // }
        }

        public bool IsActive
        {
            get => (bool) base.GetValue(IsActiveProperty);
            set
            {
                if (this.IsActive != value)
                {
                    base.SetValue(IsActiveProperty, value);
                }
            }
        }

        #endregion

        public VisualItemControl()
        {
        }

        protected override void OnApplyTemplate()
        {
        }
    }
}