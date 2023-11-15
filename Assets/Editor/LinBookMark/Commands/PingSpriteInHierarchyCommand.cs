using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace LinBookMark
{
    [UsedImplicitly]
    public class PingSpriteInHierarchyCommand:CommandBase
    {
        public override void Execute(params object[] targets)
        {
            base.Execute(targets);
            var images = GameObject.FindObjectsOfType<Image>();
            foreach (var img in images)
            {
                if (img.sprite == (Sprite)targets[0])
                {
                    EditorGUIUtility.PingObject(img.gameObject);
                }
            }
        }
    }

}