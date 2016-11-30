using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CreatingA2DSprite
{
    public class Player
    {
        public bool active = true;
        public long id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public int last { get; set; }
        public Vector2 Position;
        public Texture2D mSpriteTexture;
        public Color color;

        public Player(Texture2D texture, Color c, float X, float Y, long Id)
        {
            id = Id;
            x = X;
            y = Y;
            color = c;
            mSpriteTexture = texture;
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            Position = new Vector2(x, y);
            theSpriteBatch.Draw(mSpriteTexture, Position, color);
        }

    }
}
