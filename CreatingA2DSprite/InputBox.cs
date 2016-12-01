using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CreatingA2DSprite
{
    public class InputBox
    {
        public bool show = false;
        public string name = "";
        public Texture2D rect { get; set; }
        public Vector2 Position, FontPos, FontOrigin, coor;
        public int x, y, w, h;

        public InputBox()
        {
            w = Game1.vw / 2;
            h = Game1.vh / 7;
            x = w / 2;
            y = h * 3;

            rect = new Texture2D(Game1.graphics.GraphicsDevice, w, h);

            Color[] data = new Color[w * h];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Chocolate;
            rect.SetData(data);

            coor = new Vector2(x, y);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            Position = new Vector2(x, y);
            theSpriteBatch.Draw(rect, coor, Color.White);
            if (name != "")
            {
                FontOrigin = Game1.Font1.MeasureString(name) / 2;
                FontPos = new Vector2(x + (rect.Width / 2), y + (h/2));
                theSpriteBatch.DrawString(Game1.Font1, name, FontPos, Color.LightGreen, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
        }
    }
}
