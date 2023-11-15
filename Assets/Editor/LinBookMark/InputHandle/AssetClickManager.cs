using System.Collections.Generic;

namespace LinBookMark
{
    public class AssetClickManager:IAssetClickHandler
    {
        private List<IAssetClickHandler> handlers;
        public bool HandleClickAsset(string assetPath)
        {
            if (handlers == null)
            {
                InitHandlers();
            }

            foreach (var assetClickHandler in handlers)
            {
                if (assetClickHandler.HandleClickAsset(assetPath))
                {
                    return true;
                }
            }

            return false;
        }

        void InitHandlers()
        {
            handlers = new List<IAssetClickHandler>();
            handlers.Add(new SpriteAssetClickHandler());
            handlers.Add(new TextureAssetClickHandler());
        }
        
        
    }
}