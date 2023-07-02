using System;
using ExpPanelRenderer.CustomControl.InteractionLogic.Behavior;
using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.InteractionLogic.TriggerAction;

public class FadeToTriggerAction : TriggerAction<VisualElement>
{
    public double Opacity { get; set; }

    protected override void Invoke(VisualElement item)
    {
        var animationTime = (uint) TimeSpan.FromSeconds(CustomBehavior.GetAnimationTime(item)).TotalMilliseconds;
        item.FadeTo(Opacity, animationTime);
    }
}