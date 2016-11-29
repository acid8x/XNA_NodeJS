using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CreatingA2DSprite
{
    class Sprite
    {
        public Vector2 Position = new Vector2(0,0);
        public Texture2D mSpriteTexture;
        public Color color = Color.White;

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
        }
        
        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(mSpriteTexture, Position, color);
        }

    }
}
