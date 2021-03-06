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
        public static GraphicsDeviceManager graphics;
        public static SpriteFont Font1;
        public static Texture2D textureHP;
        SpriteBatch spriteBatch;
        KeyboardState newState, oldState;
        Texture2D texture;
        Socket socket;
        MessageBox exitM, help;
        Player myPlayer = null;
        List<Player> players = null;
        List<Keys> keys = new List<Keys>();
        Random rand = new Random();
        int now = 0, vw = 1280, vh = 720, connectedPlayers = 1;
        bool getKeys = true;
        Color cString = new Color(0, 0, 0, 192);
        Vector2 FontOrigin, FontOrigin2, FontPos;
        
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
                socket.Emit("position", myPlayer.hp, myPlayer.name, myPlayer.x, myPlayer.y, myPlayer.color.R, myPlayer.color.G, myPlayer.color.B);
            });

            socket.On("update", (data) =>
            {
                string getname = "";
				int gethp = -1;
                long getid = -1;
                float getx = -1, gety = -1;
                byte r = 0, g = 0, b = 0;
                bool found = false;
                JObject o = JObject.Parse(data.ToString());

                foreach (var x in o)
                {
                    string name = x.Key;
                    JToken value = x.Value;
                    if (name == "hp") gethp = value.ToObject<int>();
                    if (name == "name") getname = value.ToObject<string>();
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
						        p.hp = gethp;
                                p.name = getname;
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
            textureHP = new Texture2D(GraphicsDevice, 1, 1);
            textureHP.SetData(new Color[] { Color.White });
            Font1 = Content.Load<SpriteFont>("Courier New");
            Keys k = Keys.A;
            for (int i = 0; i < 26; i++) keys.Add(k++);
            keys.Add(Keys.Space);
            exitM = new MessageBox("Exit Game ? (Y/N)", Color.Black, vw / 2, vh / 2, false);
            help = new MessageBox("'F2' to change your player name", Color.Black, vw / 2, vh / 2, false);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape) && !exitM.mActive) exitM.mActive = true;

            if (newState.IsKeyDown(Keys.F1) && oldState.IsKeyUp(Keys.F1)) help.mActive = !help.mActive;
            if (newState.IsKeyDown(Keys.F2) && oldState.IsKeyUp(Keys.F2))
            {
                getKeys = true;
                myPlayer.name = "Enter your name";
            }

            if (exitM.mActive)
            {
                if (newState.IsKeyDown(Keys.Y) && oldState.IsKeyUp(Keys.Y)) this.Exit();
                if (newState.IsKeyDown(Keys.N) && oldState.IsKeyUp(Keys.N)) exitM.mActive = false;
            }

            if (myPlayer != null && !exitM.mActive)
            {
                now = (int)gameTime.TotalGameTime.TotalMilliseconds;
                
                Vector2 oldPosition = myPlayer.Position;

                if (newState.IsKeyDown(Keys.Left)) myPlayer.x -= 3;
                if (newState.IsKeyDown(Keys.Right)) myPlayer.x += 3;
                if (newState.IsKeyDown(Keys.Up)) myPlayer.y -= 3;
                if (newState.IsKeyDown(Keys.Down)) myPlayer.y += 3;

                if (getKeys)
                {
                    if (!myPlayer.editName) myPlayer.editName = true;
                    foreach (Keys k in keys)
                    {
                        if (newState.IsKeyDown(k) && oldState.IsKeyUp(k))
                        {
                            if (myPlayer.name == "Enter your name") myPlayer.name = k.ToString();
                            else if (myPlayer.name.Length < 16) myPlayer.name += k.ToString();
                        }
                    }
                    if (newState.IsKeyDown(Keys.Back) && oldState.IsKeyUp(Keys.Back)) if (myPlayer.name.Length > 0) myPlayer.name = myPlayer.name.Remove(myPlayer.name.Length - 1);
                    if (newState.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
                    {
                        getKeys = false;
                        if (myPlayer.name == "Enter your name") myPlayer.name = "";
                    }
                }
                else if (myPlayer.editName) myPlayer.editName = false;

                if (myPlayer.x < 0) myPlayer.x = 0;
                else if ((myPlayer.x + myPlayer.mSpriteTexture.Width) > graphics.GraphicsDevice.Viewport.Width) myPlayer.x = graphics.GraphicsDevice.Viewport.Width - myPlayer.mSpriteTexture.Width;
                if (myPlayer.y < 0) myPlayer.y = 0;
                else if ((myPlayer.y + myPlayer.mSpriteTexture.Height) > graphics.GraphicsDevice.Viewport.Height) myPlayer.y = graphics.GraphicsDevice.Viewport.Height - myPlayer.mSpriteTexture.Height;

                Rectangle rect = new Rectangle((int)(myPlayer.x - myPlayer.mSpriteTexture.Width / 2), (int)(myPlayer.y - myPlayer.mSpriteTexture.Height / 2), myPlayer.mSpriteTexture.Width, myPlayer.mSpriteTexture.Height);
                if (players != null)
                {
					connectedPlayers = 1;
                    foreach (Player p in players)
                    {
                        if (now - p.last > 1000) p.active = false;
                        else p.active = true;
                        if (p.active)
                        {
							connectedPlayers++;
                            Rectangle otherRect = new Rectangle((int)(p.x - p.mSpriteTexture.Width / 2), (int)(p.y - p.mSpriteTexture.Height / 2), p.mSpriteTexture.Width, p.mSpriteTexture.Height);
                            if (rect.Intersects(otherRect))
                            {
								myPlayer.hp -= 5;
                                if (myPlayer.hp < 0) myPlayer.hp = 0;
                                myPlayer.x = oldPosition.X - (myPlayer.x - oldPosition.X);
                                myPlayer.y = oldPosition.Y - (myPlayer.y - oldPosition.Y);
                            }
                        }
                    }
                }

                if (now - myPlayer.last > 200)
                {
                    socket.Emit("position", myPlayer.hp, myPlayer.name, myPlayer.x, myPlayer.y, (int)myPlayer.color.R, (int)myPlayer.color.G, (int)myPlayer.color.B);
                    myPlayer.last = now;                    
                }

                if (now - myPlayer.regen > 2000)
                {
                    if (myPlayer.hp < myPlayer.maxHp) myPlayer.hp++;
                    myPlayer.regen = now;
                }
            }

            oldState = newState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (myPlayer != null) myPlayer.Draw(spriteBatch);
            if (players != null) foreach (Player p in players) if (p.active) p.Draw(spriteBatch);
            if (exitM.mActive) exitM.Draw(spriteBatch);
            if (help.mActive) help.Draw(spriteBatch);
            string connectedPlayersString = "Press 'F1' for help\nPlayers: " + connectedPlayers;
            spriteBatch.DrawString(Font1, connectedPlayersString, new Vector2(10, 5), cString, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
