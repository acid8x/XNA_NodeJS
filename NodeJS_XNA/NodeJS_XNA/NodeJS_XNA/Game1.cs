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
        public static SpriteFont Font1;
        public static Texture2D texture;
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Socket socket;
        public KeyboardManager km = new KeyboardManager();
        public Player myPlayer = null;
        public List<Player> players = new List<Player>();
        public int now = 0;
        public bool editName = false;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = vw;
            graphics.PreferredBackBufferHeight = vh;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            socket = IO.Socket("http://localhost");
            socket.On(Socket.EVENT_CONNECT, () => { });
            socket.On("id", (data) => { myPlayer = new Player((long)data); });
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
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Font1 = Content.Load<SpriteFont>("Motorwerk");
            texture = Content.Load<Texture2D>("SquareGuy");
        }
        
        protected override void Update(GameTime gameTime) {
            now = (int)gameTime.TotalGameTime.TotalMilliseconds;
            km.Update();
            if (km.press(Keys.Escape)) this.Exit();
            if (myPlayer != null)
            {
                Vector2 oldPosition = new Vector2(myPlayer.x, myPlayer.y);
                if (!editName)
                {
                    if (km.hold(Keys.A)) myPlayer.x -= 5;
                    if (km.hold(Keys.D)) myPlayer.x += 5;
                    if (km.hold(Keys.W)) myPlayer.y -= 5;
                    if (km.hold(Keys.S)) myPlayer.y += 5;
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
                        if (now - p.last > 1000) p.active = false;
                        else p.active = true;
                        if (p.active)
                        {
                            Rectangle otherRect = new Rectangle((int)(p.x - (texture.Width-14) / 2), (int)(p.y - texture.Height / 2)+10, texture.Width-14, texture.Height-20);
                            if (rect.Intersects(otherRect))
                            {
                                myPlayer.x = oldPosition.X - (myPlayer.x - oldPosition.X);
                                myPlayer.y = oldPosition.Y - (myPlayer.y - oldPosition.Y);
                            }
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
            if (myPlayer != null) myPlayer.Draw(spriteBatch,now);
            foreach (Player p in players) if (p.active) p.Draw(spriteBatch,now);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
