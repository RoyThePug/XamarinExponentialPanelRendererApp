using System.Collections.Generic;
using ExpPanelRenderer.Model;

namespace ExpPanelRenderer.Common;

public class TestEqComparer : IEqualityComparer<TestModel>
{
    public bool Equals(TestModel x, TestModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
            
        return x.Color.Equals(y.Color) && x.Text.Equals(y.Text);
    }

    public int GetHashCode(TestModel obj)
    {
        return obj.Text.GetHashCode() ^
               obj.Color.GetHashCode();
    }
}