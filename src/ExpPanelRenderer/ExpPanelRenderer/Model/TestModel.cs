using System;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExpPanelRenderer.Model
{
    public partial class TestModel : ObservableObject
    {
        public Guid Id { get; }
        [ObservableProperty] private string _text;

        [ObservableProperty] private Color _color;

        public TestModel(string name, Color color)
        {
            Id = Guid.NewGuid();
            Text = name;
            Color = color;
        }
    }
}