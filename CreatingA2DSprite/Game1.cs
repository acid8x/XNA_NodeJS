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

        public class Player
        {
            public int i = -1;
            public long id = -1;
            public float x = -1;
            public float y = -1;
            public int last = -1;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = true;
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
                players[0].x = 0;
                players[0].y = 0;
                players[0].last = 0;
                ready = true;
            });
            
            socket.On("update", (data) =>
            {
                long getid = -1;
                float getx = -1, gety = -1;
                bool found = false;
                JObject o = JObject.Parse(data.ToString());

                foreach (var x in o)
                {
                    string name = x.Key;
                    JToken value = x.Value;
                    if (name == "id") getid = value.ToObject<long>();
                    if (name == "x") getx = value.ToObject<float>();
                    if (name == "y") gety = value.ToObject<float>();
                }
                for (int i = 1; i < 8; i++)
                {
                    Player p = players[i];
                    if (p.id == getid)
                    {
                        sprites[i].Position.X = getx;
                        sprites[i].Position.Y = gety;
                        p.last = now;
                        found = true;
                    } else if (now - p.last > 1000)
                    {
                        sprites[i].Position.X = -200;
                        sprites[i].Position.Y = -200;
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
                sprites[i].Position = new Vector2(-200, -200);
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

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) this.Exit();

            if (newState.IsKeyDown(Keys.Left)) sprites[0].Position.X -= 3;
            if (newState.IsKeyDown(Keys.Right)) sprites[0].Position.X += 3;
            if (newState.IsKeyDown(Keys.Up)) sprites[0].Position.Y -= 3;
            if (newState.IsKeyDown(Keys.Down)) sprites[0].Position.Y += 3;

            if (sprites[0].Position.X < 0) sprites[0].Position.X = 0;
            else if ((sprites[0].Position.X + sprites[0].mSpriteTexture.Width) > graphics.GraphicsDevice.Viewport.Width) sprites[0].Position.X = graphics.GraphicsDevice.Viewport.Width - sprites[0].mSpriteTexture.Width;
            if (sprites[0].Position.Y < 0) sprites[0].Position.Y = 0;
            else if ((sprites[0].Position.Y + sprites[0].mSpriteTexture.Height) > graphics.GraphicsDevice.Viewport.Height) sprites[0].Position.Y = graphics.GraphicsDevice.Viewport.Height - sprites[0].mSpriteTexture.Height;
            
            if (now - players[0].last > 200)
            {
                socket.Emit("position", sprites[0].Position.X, sprites[0].Position.Y);
                players[0].last = now;
            }

            for (int i = 1; i < 8; i++)
            {
                Player p = players[i];
                if (now - p.last > 1000)
                {
                    sprites[i].Position.X = -200;
                    sprites[i].Position.Y = -200;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int i = 0; i < 8; i++) sprites[i].Draw(this.spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
