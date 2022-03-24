using System.Windows;
using System.Windows.Controls;

namespace Xivion
{
    public static class GridHelper
    {
        public static GridLength ConvertFromString(string Value)
            => ((GridLength)(new GridLengthConverter().ConvertFromString(Value)));

        public static void SetRow(this UIElement targetElement, int Index)
            => Grid.SetRow(targetElement, Index);

        public static void SetColumn(this UIElement targetElement, int Index)
            => Grid.SetColumn(targetElement, Index);

        public static void SetRowSpan(this UIElement targetElement, int Index)
            => Grid.SetRowSpan(targetElement, Index);

        public static void SetColumnSpan(this UIElement targetElement, int Index)
            => Grid.SetColumnSpan(targetElement, Index);
    }
}
