using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.InteractionLogic.Behavior;

public static class CustomBehavior
{
    public static readonly BindableProperty AnimationTimeProperty =
        BindableProperty.CreateAttached(
            "AnimationTime",
            typeof(double),
            typeof(CustomBehavior),
            defaultValue: 1.0);

    public static double GetAnimationTime(BindableObject view)
    {
        return (double) view.GetValue(AnimationTimeProperty);
    }

    public static void SetAnimationTime(BindableObject view, double value)
    {
        view.SetValue(AnimationTimeProperty, value);
    }
}