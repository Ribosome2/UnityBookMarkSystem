using UnityEngine;

namespace LinBookMark.InputHandle
{
    public class TreeViewInputHandler
    {

        public void HandleKeyBoardEvent()
        {
            if (Event.current.type == EventType.KeyUp)
            {
                var keyCode = Event.current.keyCode;
                if (keyCode == KeyCode.F2)
                {
                    
                }
            }
        }
        
       
    }
}