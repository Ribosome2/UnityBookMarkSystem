using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace LinBookMark
{
    [UsedImplicitly]
    public class PasteAssetCommand:CommandBase
    {
        public override void Execute(params object[] targets)
        {
            base.Execute(targets);
            if (targets != null && targets.Length > 0)
            {
                var assetPath = (string)targets[0];
                var folder = string.Empty;
                if (Directory.Exists(assetPath))
                {
                     folder = assetPath;
                }
                else
                {
                    folder = Path.GetDirectoryName(assetPath);
                }
                foreach (var oldPath in KyleSelections.CopyPaths)
                {
                    // Debug.Log("old: "+ oldPath+ " --- new  "+ folder);
                    if (string.IsNullOrEmpty(oldPath) == false && string.IsNullOrEmpty(folder) == false)
                    {
                        var newPath = Path.Combine(folder, Path.GetFileName(oldPath));
                        AssetDatabase.CopyAsset(oldPath, newPath);
                    }
                }
            }
        }
    }

}