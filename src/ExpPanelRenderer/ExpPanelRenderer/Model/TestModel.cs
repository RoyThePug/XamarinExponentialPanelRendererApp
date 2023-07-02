using System;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExpPanelRenderer.Model
{
    public partial class TestModel : ObservableObject
    {
        public Guid Id { get; }
        [ObservableProperty] private string _title;
        
        [ObservableProperty] private string _text;

        [ObservableProperty] private Color _color;

        public TestModel(int index, string name, Color color)
        {
            Id = Guid.NewGuid();
            Title = index.ToString();
            Text = name;
            Color = color;
        }

        public TestModel DeepCopy()
        {
            var copy = (TestModel)MemberwiseClone();
            copy.Text = string.Copy(Text);
            return copy;
        }
    }
}