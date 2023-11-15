using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;
using UnityEngine;
using UnityObject = UnityEngine.Object;
namespace LinBookMark
{
    public interface IDragDropHandler
    {
        void SetupDragAndDrop(IList<int> sortedDraggedIDs);
    }
}