using Unity.VisualScripting;
using UnityEngine;

namespace Refactor.Data
{
    [CreateAssetMenu(fileName = "BindingDisplayPalette", menuName = "Refactor/BindingDisplayPallete", order = 100)]
    public class BindingDisplayPalette : ScriptableObject
    {
        public Sprite[] sprites;

        public Sprite ResolveSprite(string name)
        {
            foreach (var v in sprites)
            {
                if (v.name.ToLower().EndsWith(name.ToLower()))
                    return v;
            }

            return null;
        }
    }
}