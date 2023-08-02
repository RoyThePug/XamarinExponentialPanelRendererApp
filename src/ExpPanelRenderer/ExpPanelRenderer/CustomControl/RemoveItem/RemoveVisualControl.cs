using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using TouchTracking;
using TouchTracking.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace ExpPanelRenderer.CustomControl.RemoveItem;

public class RemoveVisualControl : ContentView
{
    #region Field

    private Grid? _rootGrid;

    private Path? _partImage;

    #endregion

    #region Event

    public event EventHandler? Clicked;

    #endregion

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _rootGrid = GetTemplateChild("PART_Root") as Grid;

        if (_rootGrid != null)
        {
            var touchEffect = new TouchEffect();

            touchEffect.TouchAction += TouchEffectOnTouchAction;
            _rootGrid.Effects.Add(touchEffect);
        }

        _partImage = GetTemplateChild("PART_Image") as Path;
    }

    #region Event Handler

    private void TouchEffectOnTouchAction(object sender, TouchActionEventArgs args)
    {
        if (_partImage == null)
        {
            return;
        }

        TouchEffect touchEffect = (TouchEffect) _rootGrid.Effects.FirstOrDefault(e => e is TouchEffect);

        if (touchEffect != null)
        {
            if (args.Type is TouchActionType.Pressed or TouchActionType.Entered)
            {
                touchEffect.Capture = true;
                VisualStateManager.GoToState(_partImage, "Pressed");
                //  Debug.WriteLine("Pressed");
            }

            if (args.Type is TouchActionType.Released)
            {
                Clicked?.Invoke(sender as Grid, EventArgs.Empty);
                VisualStateManager.GoToState(_partImage, "Normal");
                touchEffect.Capture = false;

                //  Debug.WriteLine("Normal");
            }
        }
    }

    #endregion

    #region Method

    #endregion
}