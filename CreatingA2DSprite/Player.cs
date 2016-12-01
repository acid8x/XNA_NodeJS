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
        public bool active = true, editName = false;
        public string name = "Enter your name";
        public long id { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public int last { get; set; }
        public Vector2 Position, FontPos, FontOrigin;
        public Texture2D mSpriteTexture;
        public Color color;
        private int alpha = 255;
        public int maxHp = 100, hp = 80;

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
            int barX = (int)x - (100 - mSpriteTexture.Width) / 2;
            int barY = (int)y - 16;
            int hpWidth = (hp * 100) / maxHp;
            theSpriteBatch.Draw(Game1.textureHP, new Rectangle(barX, barY, 100, 8), Color.Red);
            theSpriteBatch.Draw(Game1.textureHP, new Rectangle(barX, barY, hpWidth, 8), Color.Green);
            if (name != "")
            {
                FontOrigin = Game1.Font1.MeasureString(name) / 2;
                FontPos = new Vector2(x + (mSpriteTexture.Width / 2), y + 16 + mSpriteTexture.Height);
                if (!editName) theSpriteBatch.DrawString(Game1.Font1, name, FontPos, Color.Black, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                else
                {
                    alpha-=3;
                    if (alpha < 127) alpha = 255;
                    Color c2 = new Color(0, 0, 0, alpha);
                    theSpriteBatch.DrawString(Game1.Font1, name, FontPos, c2, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                }
            }
        }
    }
}
