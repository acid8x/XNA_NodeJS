using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CreatingA2DSprite
{
    public class MessageBox
    {
        public bool mActive;
        public string mText;
        public Vector2 Position, FontOrigin;
        public Color color = Color.Black;

        public MessageBox(string text, Color c, float X = 0, float Y = 0, bool active = true)
        {
            mActive = active;
            mText = text;
            color = c;
            Position = new Vector2(X, Y);
            FontOrigin = Game1.Font1.MeasureString(text) / 2;
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.DrawString(Game1.Font1, mText, Position, color, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }
    }
}
