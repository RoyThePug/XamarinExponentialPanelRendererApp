using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpPanelRenderer.CustomControl.VisualItemsPanel;
using Xamarin.Forms;

namespace ExpPanelRenderer.CustomControl.Service;

public class RenderService : IRenderService
{
    private readonly VisualItemsPanelControl _visualItemsPanelControl;

    public RenderService(VisualItemsPanelControl visualItemsPanelControl)
    {
        _visualItemsPanelControl = visualItemsPanelControl ?? throw new ArgumentNullException(nameof(visualItemsPanelControl));
    }

    public async Task RenderItems()
    {
        try
        {
            if (_visualItemsPanelControl.Items.Any())
            {
                _visualItemsPanelControl._hiddenItems.Clear();
                
                foreach (var item in _visualItemsPanelControl.Items)
                {
                    item.IsActive = false;
                }

                _visualItemsPanelControl.CalculateItemsYTranslate();
                
                await Animate();
            }
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            
        }
    }
    
    private Task Animate()
    {
        var data = _visualItemsPanelControl._hiddenItems.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        var tasks = new List<Task>();

        foreach (var item in data)
        {
            item.Key.TranslationY = 0;
            item.Key.WidthRequest = _visualItemsPanelControl._currentWidth;
            item.Key.IsEditable = false;
            // might use Child Animation
            //var animation = new Animation(v => item.Key.TranslateTo(0, item.Value, 3000));

            tasks.Add(item.Key.TranslateTo(0, item.Value, (uint) TimeSpan.FromSeconds(_visualItemsPanelControl.AnimationTime).TotalMilliseconds));
        }

        data.Keys.Last().IsActive = true;

        return Task.WhenAll(tasks);
    }
}