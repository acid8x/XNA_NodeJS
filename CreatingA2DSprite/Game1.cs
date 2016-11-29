using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;

namespace CreatingA2DSprite
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Socket socket;
        Sprite[] sprites = new Sprite[8];
        Player[] players = new Player[8];
        int index = 1;
        bool ready = false;
        int now = 0;
        Random r = new Random();

        public class Player
        {
            public int i = -1;
            public long id = -1;
            public float x = -1;
            public float y = -1;
            public int r = 126, g = 126, b = 126;
            public int last = -1;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            socket = IO.Socket("http://robo-warz2.com:2222");
            socket.On(Socket.EVENT_CONNECT, () =>
            {

            });

            socket.On("id", (data) =>
            {
                players[0].i = 0;
                players[0].id = (long)data;
                players[0].x = r.Next(0, (graphics.PreferredBackBufferWidth - sprites[0].mSpriteTexture.Width));
                players[0].y = r.Next(0, (graphics.PreferredBackBufferHeight - sprites[0].mSpriteTexture.Height));
                players[0].last = 0;
                players[0].r = r.Next(0, 255);
                players[0].g = r.Next(0, 255);
                players[0].b = r.Next(0, 255);
                sprites[0].color = new Color(players[0].r, players[0].g, players[0].b);
                sprites[0].Position = new Vector2(players[0].x, players[0].y);
                ready = true;
            });
            
            socket.On("update", (data) =>
            {
                long getid = -1;
                float getx = -1, gety = -1;
                int r = 0, g = 0, b = 0;
                bool found = false;
                JObject o = JObject.Parse(data.ToString());

                foreach (var x in o)
                {
                    string name = x.Key;
                    JToken value = x.Value;
                    if (name == "id") getid = value.ToObject<long>();
                    if (name == "x") getx = value.ToObject<float>();
                    if (name == "y") gety = value.ToObject<float>();
                    if (name == "r") r = value.ToObject<int>();
                    if (name == "g") g = value.ToObject<int>();
                    if (name == "b") b = value.ToObject<int>();
                }
                Color c = new Color(r, g, b);
                for (int i = 1; i < 8; i++)
                {
                    Player p = players[i];
                    if (p.id == getid)
                    {
                        sprites[i].Position.X = getx;
                        sprites[i].Position.Y = gety;
                        sprites[i].color = c;
                        p.last = now;
                        found = true;
                        break;
                    }
                }
                if (!found && ready)
                {
                    if (index < 8)
                    {
                        players[index].i = index;
                        players[index].id = getid;
                        players[index].x = getx;
                        players[index].y = gety;
                        index++;
                    }
                }
            });

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            for (int i = 0; i < 8; i++)
            {
                sprites[i] = new Sprite();
                sprites[i].LoadContent(Content, "SquareGuy");
                sprites[i].Position = new Vector2(i*-100, i*-100);
                players[i] = new Player();
            }
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            now = (int)gameTime.TotalGameTime.TotalMilliseconds;

            KeyboardState newState = Keyboard.GetState();

            Vector2 oldPosition = sprites[0].Position;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) this.Exit();

            if (newState.IsKeyDown(Keys.Left)) sprites[0].Position.X -= 3;
            if (newState.IsKeyDown(Keys.Right)) sprites[0].Position.X += 3;
            if (newState.IsKeyDown(Keys.Up)) sprites[0].Position.Y -= 3;
            if (newState.IsKeyDown(Keys.Down)) sprites[0].Position.Y += 3;
            if (newState.IsKeyDown(Keys.R)) players[0].r+=3;
            if (newState.IsKeyDown(Keys.G)) players[0].g+=3;
            if (newState.IsKeyDown(Keys.B)) players[0].b+=3;

            if (players[0].r > 255) players[0].r = 0;
            if (players[0].g > 255) players[0].g = 0;
            if (players[0].b > 255) players[0].b = 0;
            Color c = new Color(players[0].r, players[0].g, players[0].b);
            sprites[0].color = c;
            
            // MY METHOD TO DETECT VIEWPORT BOUNDS
            if (sprites[0].Position.X < 0) sprites[0].Position.X = 0;
            else if ((sprites[0].Position.X + sprites[0].mSpriteTexture.Width) > graphics.GraphicsDevice.Viewport.Width) sprites[0].Position.X = graphics.GraphicsDevice.Viewport.Width - sprites[0].mSpriteTexture.Width;
            if (sprites[0].Position.Y < 0) sprites[0].Position.Y = 0;
            else if ((sprites[0].Position.Y + sprites[0].mSpriteTexture.Height) > graphics.GraphicsDevice.Viewport.Height) sprites[0].Position.Y = graphics.GraphicsDevice.Viewport.Height - sprites[0].mSpriteTexture.Height;
            
            Rectangle rect = new Rectangle((int)(sprites[0].Position.X - sprites[0].mSpriteTexture.Width / 2), (int)(sprites[0].Position.Y - sprites[0].mSpriteTexture.Height / 2), sprites[0].mSpriteTexture.Width, sprites[0].mSpriteTexture.Height);
            for (int i = 1; i < 8; i++)
            {
                Rectangle otherRect = new Rectangle((int)(sprites[i].Position.X - sprites[i].mSpriteTexture.Width / 2), (int)(sprites[i].Position.Y - sprites[i].mSpriteTexture.Height / 2), sprites[i].mSpriteTexture.Width, sprites[i].mSpriteTexture.Height);
                if (rect.Intersects(otherRect) /*|| !GraphicsDevice.Viewport.Bounds.Contains(rect)*/) // PREFERED METHOD TO DETECT BOUNDS, BUT THERE IS A SPACE OF HALF SPRITE SIZE FROM REAL BOUND
                {
                    sprites[0].Position.X = oldPosition.X - (sprites[0].Position.X - oldPosition.X);
                    sprites[0].Position.Y = oldPosition.Y - (sprites[0].Position.Y - oldPosition.Y);
                }
            }
            if (now - players[0].last > 200)
            {
                socket.Emit("position", sprites[0].Position.X, sprites[0].Position.Y, players[0].r, players[0].g, players[0].b);
                players[0].last = now;
            }
            for (int i = 1; i < 8; i++)
            {
                Player p = players[i];
                if (now - p.last > 1000)
                {
                    sprites[i].Position.X = (i*-100);
                    sprites[i].Position.Y = (i*-100);
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (ready) for (int i = 0; i < 8; i++) sprites[i].Draw(this.spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
