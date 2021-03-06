using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;

namespace NodeJS_XNA {

    public class Game1 : Microsoft.Xna.Framework.Game {

        public static int vw = 800, vh = 450;
        public static SpriteFont Font1, Font2;
        public static Texture2D texture;
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Socket socket;
        public KeyboardManager km = new KeyboardManager();
        public Player myPlayer = null;
        public List<Player> players = new List<Player>();
        public int now = 0, last = 0;
        public bool editName = false, connected = false, debug = true;
        public Vector2 stringPosition, stringOrigin;
        public string connection = "Connecting ";

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = vw;
            graphics.PreferredBackBufferHeight = vh;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            socket = IO.Socket("http://robo-warz2.com:2222/");
            socket.On(Socket.EVENT_CONNECT, () => { connected = true; });
            socket.On("id", (data) => {
                myPlayer = new Player((long)data);
            });
            socket.On("update", (data) => {
                JObject jb = JObject.Parse(data.ToString());
                Player player = jb.ToObject<Player>();
                bool found = false;
                for (int i = 0; i < players.Count; i++) {
                    if (players[i].id == player.id) {
                        player.last = now;
                        players[i] = player;
                        found = true;
                        break;
                    }
                }
                if (!found) players.Add(player);
            });
            stringPosition = new Vector2(vw / 2, vh / 2);
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>("Motorwerk");
            Font2 = Content.Load<SpriteFont>("Debug");
            texture = Content.Load<Texture2D>("SquareGuy");
        }
        
        protected override void Update(GameTime gameTime) {
            now = (int)gameTime.TotalGameTime.TotalMilliseconds;
            km.Update();
            if (km.press(Keys.Escape)) this.Exit();
            if (myPlayer != null)
            {
                if (!editName)
                {
                    float curTime = gameTime.ElapsedGameTime.Milliseconds/150f;
                    if (km.down(Keys.A)) myPlayer.facingRight = false;
                    if (km.down(Keys.D)) myPlayer.facingRight = true;
                    if (km.hold(Keys.A) && myPlayer.vel.X > -40f) myPlayer.vel.X -= myPlayer.acc.X * curTime;
                    if (km.hold(Keys.D) && myPlayer.vel.X < 40f) myPlayer.vel.X += myPlayer.acc.X * curTime;
                    if (km.down(Keys.Space) && myPlayer.state == 0) {
                        myPlayer.state = 1;                         
                        myPlayer.pos = new Vector2(myPlayer.x,myPlayer.y);
                        myPlayer.vel.Y = 0;
                    } else if (myPlayer.state != 0) {
                        if (myPlayer.state == 1) myPlayer.vel.Y -= (myPlayer.acc.Y * curTime * 3);
                        else if (myPlayer.state == 2) myPlayer.vel.Y += (myPlayer.acc.Y * curTime * 2);
                        myPlayer.y += myPlayer.vel.Y * curTime;
                        if (myPlayer.y < myPlayer.pos.Y - 100f) myPlayer.state = 2;
                        if (myPlayer.y > myPlayer.pos.Y)
                        {
                            myPlayer.y = myPlayer.pos.Y;
                            myPlayer.state = 0;
                        }
                    }                       
                    if (km.press(Keys.F1)) debug = !debug;
                    if (!km.press(Keys.A) && !km.press(Keys.D)) {
                        if (myPlayer.vel.X > 0) myPlayer.vel.X--;
                        else if (myPlayer.vel.X < 0) myPlayer.vel.X++;
                    }
                    myPlayer.x += (int)(myPlayer.vel.X * curTime);
                }
                if (km.press(Keys.Enter))
                {
                    if (!editName) myPlayer.name = "";
                    editName = !editName;
                }
                if (editName) for (int i = 65; i < 65 + 26; i++) if (km.press((Keys)i)) myPlayer.name += (char)(Keys)i;
                if (now - myPlayer.last > 200)
                {
                    socket.Emit("update", JObject.FromObject(myPlayer));
                    myPlayer.last = now;
                }
                if (myPlayer.x < 0) myPlayer.x = 0;
                else if ((myPlayer.x + texture.Width) > graphics.GraphicsDevice.Viewport.Width) myPlayer.x = graphics.GraphicsDevice.Viewport.Width - texture.Width;
                if (myPlayer.y < 0) myPlayer.y = 0;
                else if ((myPlayer.y + texture.Height) > graphics.GraphicsDevice.Viewport.Height) myPlayer.y = graphics.GraphicsDevice.Viewport.Height - texture.Height;

                Rectangle rect = new Rectangle((int)(myPlayer.x - texture.Width / 2), (int)(myPlayer.y - texture.Height / 2), texture.Width, texture.Height);
                if (players != null)
                {
                    foreach (Player p in players)
                    {
                        if (p.active)
                        {
                            Rectangle otherRect = new Rectangle((int)(p.x - texture.Width / 2), (int)(p.y - texture.Height / 2), texture.Width, texture.Height);
                            if (rect.Intersects(otherRect)) myPlayer.vel *= -1.05f;
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            if (connected)
            {
                if (myPlayer != null) myPlayer.Draw(spriteBatch, now);
                foreach (Player p in players) p.Draw(spriteBatch, now);
            }
            else
            {
                if (now - last > 1000)
                {
                    last = now;
                    connection += ".";
                }
                stringOrigin = Font1.MeasureString(connection) / 2;
                spriteBatch.DrawString(Font1, connection, stringPosition, Color.Black, 0, stringOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
            if (debug && myPlayer != null)
            {
                string debugString = JObject.FromObject(myPlayer).ToString().Replace("\"", "").Replace("{", "").Replace("}", "").Replace(",", "").Replace("\"", "");
                spriteBatch.DrawString(Font2, debugString, new Vector2(0, 0), Color.Black, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
