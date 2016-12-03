using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace NodeJS_XNA {

    public class Player {
        public bool active = true;
        public long id = -1;
        public float x = 100, y = 100;
        public string name = "";
        public int last = 0, r = 0, g = 0, b = 0, hp = 100, maxHp = 100;
        
        public Player(long Id = -1) {
            Random rand = new Random();
            id = Id;
            int min = Game1.vw / 4;
            int max = min * 3;
            x = rand.Next(min,max);
            min = Game1.vh / 4;
            max = min * 3;
            y = rand.Next(min,max);
            r = rand.Next(0, 255);
            g = rand.Next(0, 255);
            b = rand.Next(0, 255);
        }
        
        public void Draw(SpriteBatch theSpriteBatch, int now = 0)
        {
            if (now - last > 1000) active = false;
            if (active)
            {
                Vector2 FromPos = new Vector2(x, y);
                Vector2 FontOrigin = Game1.Font1.MeasureString(name) / 2;
                theSpriteBatch.Draw(Game1.texture, FromPos, new Color(r, g, b));
                if (name != "") theSpriteBatch.DrawString(Game1.Font1, name, FromPos, Color.Black, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
        }
    }
}