using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.VisualItem
{
    public class VisualItemControl : TemplatedView
    {
        #region BindableProperty

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(VisualItemControl),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string) base.GetValue(TextProperty);
            set
            {
                if (this.Text != value)
                    base.SetValue(TextProperty, value);
            }
        }
        
        public static readonly BindableProperty AnimationTimeProperty = BindableProperty.Create(
            propertyName: nameof(Animation),
            returnType: typeof(double),
            declaringType: typeof(VisualItemControl),
            defaultValue:1.0,
            propertyChanged:PropertyChanged);

        private static void PropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            
        }

        public double AnimationTime
        {
            get => (double) base.GetValue(AnimationTimeProperty);
            set
            {
                if (this.AnimationTime != value)
                {
                    base.SetValue(AnimationTimeProperty, value);
                }
            }
        }
        
        
        
        public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
            propertyName: nameof(IsActive),
            returnType: typeof(bool),
            declaringType: typeof(VisualItemControl),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged:IsActivePropertyChanged);

        private static void IsActivePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (!(bool) newvalue)
            {
             //   VisualStateManager.GoToState((VisualElement)bindable, "DeActive");
            }
            else
            {
               // VisualStateManager.GoToState((VisualElement)bindable, "Focused");
            }
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