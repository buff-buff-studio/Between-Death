using UnityEngine;

namespace Refactor.Data
{
    public enum Element
    {
        None = 0,
        Order = 1,
        Chaos = 2
    }

    public static class ElementExtensions
    {
        public static Color GetColor(this Element element)
        {
            return element switch
            {
                Element.Order => new Color(0.25f, 0.25f, 1f),
                Element.Chaos => new Color(1f, 1f, 0.25f),
                _ => Color.black
            };
        }

        public static string GetName(this Element element)
        {
            return element switch
            {
                Element.Order => "Order",
                Element.Chaos => "Chaos",
                _ => "?"
            };
        }

        public static bool CanDamage(this Element element, Element other)
        {
            if (element is Element.None || other is Element.None) return true;
            return element != other;
        }
    }
}