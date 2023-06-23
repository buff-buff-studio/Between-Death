using System;
using UnityEngine;

namespace Refactor.Data
{
    public class SettingsLoader : MonoBehaviour
    {
        public Settings settings;

        public void OnEnable()
        {
            settings.Load();
        }
    }
}