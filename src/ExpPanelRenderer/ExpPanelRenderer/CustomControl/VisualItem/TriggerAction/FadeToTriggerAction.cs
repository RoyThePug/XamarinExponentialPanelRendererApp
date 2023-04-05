using System;
using ExpPanelRenderer.CustomControl.VisualItem;
using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.VisualItem.TriggerAction;

public static class MyBehavior
{
    public static readonly BindableProperty AnimationTimeProperty =
        BindableProperty.CreateAttached(
            "AnimationTime",
            typeof(double),
            typeof(MyBehavior),
            defaultValue: 0.0);

    public static double GetAnimationTime(BindableObject view)
    {
        return (double) view.GetValue(AnimationTimeProperty);
    }

    public static void SetAnimationTime(BindableObject view, double value)
    {
        view.SetValue(AnimationTimeProperty, value);
    }
}

public class FadeToTriggerAction : TriggerAction<VisualElement>
{
    public double Opacity { get; set; }
    
    protected override void Invoke(VisualElement item)
    {
        var animationTime = (uint) TimeSpan.FromSeconds(MyBehavior.GetAnimationTime(item)).TotalMilliseconds;
        item.FadeTo(Opacity,  animationTime);
    }
}