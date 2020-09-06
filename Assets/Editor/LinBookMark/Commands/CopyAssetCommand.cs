using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace LinBookMark
{
    [UsedImplicitly]
    public class CopyAssetCommand:CommandBase
    {
        public override void Execute(params object[] targets)
        {
            base.Execute(targets);
            KyleSelections.CopyPaths.Clear();
            foreach (var path in targets)
            {
                KyleSelections.CopyPaths.Add(path.ToString());
            }
        }
    }

}