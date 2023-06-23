using System;
using System.IO;
using System.Text;
using Refactor.Data.Variables;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Refactor.Data
{
    [CreateAssetMenu(menuName = "Refactor/Data/Save", fileName = "Save", order = 100)]
    public class Save : ScriptableObject
    {
        public class SaveSnapshot
        {
            public bool Exists;
            public DateTime CreationTime;
        }
        
        [Header("VARIABLES")]
        public Variable[] variables = Array.Empty<Variable>();

        [Header("SLOT")] public int currentSlot = 0;

        public void SaveTo(int slot)
        {
            var filePath = GetPath(slot);
            
            var builder = new StringBuilder();

            foreach(var v in variables)
            {
                builder.AppendLine(v.ToJson());
            }

            File.WriteAllText(filePath, builder.ToString());
        }

        public void ResetData()
        {
            foreach(var v in variables)
            {
                v.Reset();
                v.ForceUpdate();
            }
        }

        public void LoadFrom(int slot)
        {
            var filePath = GetPath(slot);

            if(!File.Exists(filePath))
            {
                foreach(var v in variables)
                {
                    v.Reset();
                    v.ForceUpdate();
                }

                return;
            }

            var lines = File.ReadAllLines(filePath);

            for(var i = 0; i < lines.Length; i ++)
            {
                variables[i].LoadFromJson(lines[i]);
                variables[i].ForceUpdate();
            }
        }

        public void DeleteCurrentSlot()
        {
            Delete(currentSlot);
        }
        
        public void Delete(int slot)
        {
            var path = GetPath(slot);
            if (File.Exists(path))
                File.Delete(path);
        }

        public SaveSnapshot GetSaveSnapshot(int slot)
        {
            var path = GetPath(slot);
            if (File.Exists(path))
            {
                return new SaveSnapshot() { Exists = true, CreationTime = File.GetLastWriteTime(path) };
            }
            
            return new SaveSnapshot() { Exists = false };         
        }

        public string GetPath(int slot)
        {
            return $"{Application.persistentDataPath}/save_{slot}.save";
        }

        /*
        public string path => Application.persistentDataPath + "/save.dat";

        public bool hasSave => File.Exists(path);
        
        public void SaveData()
        {
            var builder = new StringBuilder();

            foreach(var v in variables)
            {
                builder.AppendLine(v.ToJson());
            }

            File.WriteAllText(path, builder.ToString());
            Debug.Log($"[!] Saved save to '{path}'");
        }

        public void Delete()
        {
            if(File.Exists(path))
                File.Delete(path);

            LoadData();
        }
        
        public void LoadData()
        {
            if(!File.Exists(path))
            {
                foreach(var v in variables)
                {
                    v.Reset();
                    v.ForceUpdate();
                }

                return;
            }

            var lines = File.ReadAllLines(path);

            for(var i = 0; i < lines.Length; i ++)
            {
                variables[i].LoadFromJson(lines[i]);
                //JsonUtility.FromJsonOverwrite(lines[i], variables[i]);
                variables[i].ForceUpdate();
            }
            
            Debug.Log($"[!] Loaded save from '{path}'");
        }
        */
    }
}