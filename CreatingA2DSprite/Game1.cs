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
        MouseState mouse, omouse;
        public static SpriteFont Font1;
        Texture2D texture;
        Socket socket;
        Player myPlayer = null;
        List<Player> players = null;
        Random rand = new Random();
        int now = 0, vw = 1280, vh = 720;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = vw;
            graphics.PreferredBackBufferHeight = vh;
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            socket = IO.Socket("http://robo-warz2.com:2222");
            socket.On(Socket.EVENT_CONNECT, () =>
            {

            });

            socket.On("id", (data) =>
            {
                int mix, max, miy, may;
                mix = texture.Width / 2;
                miy = texture.Height / 2;
                max = vw - mix;
                may = vh - miy;
                float x, y;
                x = rand.Next(mix, max);
                y = rand.Next(miy, may);
                int r, g, b;
                r = rand.Next(0, 255);
                g = rand.Next(0, 255);
                b = rand.Next(0, 255);
                Color c = new Color(r, g, b);
                myPlayer = new Player(texture, c, x, y, (long)data);
                socket.Emit("position", myPlayer.x, myPlayer.y, myPlayer.color.R, myPlayer.color.G, myPlayer.color.B);
            });
            
            socket.On("update", (data) =>
            {
                long getid = -1;
                float getx = -1, gety = -1;
                byte r = 0, g = 0, b = 0;
                bool found = false;
                JObject o = JObject.Parse(data.ToString());

                foreach (var x in o)
                {
                    string name = x.Key;
                    JToken value = x.Value;
                    if (name == "id") getid = value.ToObject<long>();
                    if (name == "x") getx = value.ToObject<float>();
                    if (name == "y") gety = value.ToObject<float>();
                    if (name == "r") r = value.ToObject<byte>();
                    if (name == "g") g = value.ToObject<byte>();
                    if (name == "b") b = value.ToObject<byte>();
                }

                if (getid != -1 && getx != -1 && gety != -1)
                {
                    Color c = new Color(r, g, b);
                    if (players != null)
                    {
                        foreach (Player p in players)
                        {
                            if (p.id == getid)
                            {
                                found = true;
                                p.x = getx;
                                p.y = gety;
                                p.color = c;
                                p.last = now;
                                break;
                            }
                        }
                    }
                    else players = new List<Player>();
                    if (!found) players.Add(new Player(texture, c, getx, gety, getid));
                }
            });

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("SquareGuy");
            Font1 = Content.Load<SpriteFont>("Courier New");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) this.Exit();

            if (myPlayer != null)
            {
                now = (int)gameTime.TotalGameTime.TotalMilliseconds;
                
                KeyboardState newState = Keyboard.GetState();

                Vector2 oldPosition = myPlayer.Position;

                if (newState.IsKeyDown(Keys.Left)) myPlayer.x -= 3;
                if (newState.IsKeyDown(Keys.Right)) myPlayer.x += 3;
                if (newState.IsKeyDown(Keys.Up)) myPlayer.y -= 3;
                if (newState.IsKeyDown(Keys.Down)) myPlayer.y += 3;

                if (myPlayer.x < 0) myPlayer.x = 0;
                else if ((myPlayer.x + myPlayer.mSpriteTexture.Width) > graphics.GraphicsDevice.Viewport.Width) myPlayer.x = graphics.GraphicsDevice.Viewport.Width - myPlayer.mSpriteTexture.Width;
                if (myPlayer.y < 0) myPlayer.y = 0;
                else if ((myPlayer.y + myPlayer.mSpriteTexture.Height) > graphics.GraphicsDevice.Viewport.Height) myPlayer.y = graphics.GraphicsDevice.Viewport.Height - myPlayer.mSpriteTexture.Height;

                Rectangle rect = new Rectangle((int)(myPlayer.x - myPlayer.mSpriteTexture.Width / 2), (int)(myPlayer.y - myPlayer.mSpriteTexture.Height / 2), myPlayer.mSpriteTexture.Width, myPlayer.mSpriteTexture.Height);
                if (players != null)
                {
                    foreach (Player p in players)
                    {
                        if (now - p.last > 1000) p.active = false;
                        else p.active = true;
                        Rectangle otherRect = new Rectangle((int)(p.x - p.mSpriteTexture.Width / 2), (int)(p.y - p.mSpriteTexture.Height / 2), p.mSpriteTexture.Width, p.mSpriteTexture.Height);
                        if (rect.Intersects(otherRect))
                        {
                            myPlayer.x = oldPosition.X - (myPlayer.x - oldPosition.X);
                            myPlayer.y = oldPosition.Y - (myPlayer.y - oldPosition.Y);
                        }
                    }
                }

                mouse = Mouse.GetState();
                if (mouse.LeftButton == ButtonState.Pressed && omouse.LeftButton == ButtonState.Released)
                {
                    int xpos = mouse.X;
                    int ypos = mouse.Y;
                    Rectangle mClick = new Rectangle(xpos, ypos, 1, 1);
                    if (mClick.Intersects(rect)) myPlayer.name = "myPlayer been clicked";
                }
                omouse = mouse;

                if (now - myPlayer.last > 200)
                {
                    socket.Emit("position", myPlayer.x, myPlayer.y, (int)myPlayer.color.R, (int)myPlayer.color.G, (int)myPlayer.color.B);
                    myPlayer.last = now;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (myPlayer != null) myPlayer.Draw(spriteBatch);
            if (players != null) foreach (Player p in players) if (p.active) p.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
