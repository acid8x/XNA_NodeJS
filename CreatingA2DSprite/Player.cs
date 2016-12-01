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
        public string name = "";
        public long id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public int last { get; set; }
        public Vector2 Position, FontPos, FontOrigin;
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
            if (name != "")
            {
                FontOrigin = Game1.Font1.MeasureString(name) / 2;
                FontPos = new Vector2(x + (mSpriteTexture.Width / 2), y - 16);
                theSpriteBatch.DrawString(Game1.Font1, name, FontPos, Color.Black, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
        }
    }
}
