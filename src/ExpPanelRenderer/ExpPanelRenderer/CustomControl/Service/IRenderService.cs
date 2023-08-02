using System;
using System.Threading.Tasks;

namespace ExpPanelRenderer.CustomControl.Service;

public interface IRenderService
{
    Task RenderItems();
}