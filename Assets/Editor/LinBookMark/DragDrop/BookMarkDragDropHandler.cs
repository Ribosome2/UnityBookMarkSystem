using System.Collections.Generic;
using UnityEditor;
using UnityObject = UnityEngine.Object;
namespace LinBookMark
{
    public class BookMarkDragDropHandler:IDragDropHandler
    {
        // public void SetupDragAndDrop(DragAndDropArgs args)
        // {
        //     DragAndDrop.PrepareStartDrag ();
        //
        //     var sortedDraggedIDs = SortItemIDsInRowOrder (args.draggedItemIDs);
        //
        //     List<UnityObject> objList = new List<UnityEngine.Object> (sortedDraggedIDs.Count);
        //     foreach (var id in sortedDraggedIDs)
        //     {
        //         UnityObject obj = EditorUtility.InstanceIDToObject (id);
        //         if (obj != null)
        //             objList.Add (obj);
        //     }
        //
        //     DragAndDrop.objectReferences = objList.ToArray ();
        //         
        //     string title = objList.Count > 1 ? "<Multiple>" : objList[0].name;
        //     DragAndDrop.StartDrag (title);
        // }
        public void SetupDragAndDrop()
        {
            throw new System.NotImplementedException();
        }
    }
}