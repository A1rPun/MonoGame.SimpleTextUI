using A1r.Input;
using A1r.SimpleTextUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimpleMenu
{
    public class SimpleMenu : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SimpleTextUI menu;
        SimpleTextUI options;
        SimpleTextUI currentUI;
        InputManager iM;

        public SimpleMenu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            setFullScreen();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            var font = Content.Load<SpriteFont>("Menu");
            menu = new SimpleTextUI(this, font, new[] { "Singleplayer", "Multiplayer", "Options", "Credits", "Exit" })
            {
                Align = Alignment.Right,
                TextColor = Color.IndianRed,
                SelectedColor = Color.Red
            };
            options = new SimpleTextUI(this, font, new[]
            {
                new SelectElement("Audio", new[] { "5.1", "7" }),
                new SelectElement("Video", new[] { "Onboard", "GPU", "Both" }),
                new SelectElement("Numberwang", new[] { "2", "29", "42", "69", "1336.9", "1337" })
            })
            {
                Align = Alignment.Center,
                Visible = false
            };
            currentUI = menu;
            iM = new InputManager(this);

            Components.Add(menu);
            Components.Add(options);
            Components.Add(iM);
        }

        protected override void Update(GameTime gameTime)
        {
            if (iM.JustPressed(Keys.Up))
                currentUI.Move(Direction.Up);
            else if (iM.JustPressed(Keys.Down))
                currentUI.Move(Direction.Down);
            else if (iM.JustPressed(Keys.Left))
                currentUI.Move(Direction.Left);
            else if (iM.JustPressed(Keys.Right))
                currentUI.Move(Direction.Right);

            if (iM.JustPressed(Keys.Enter))
            {
                if (menu.GetValue() == "Options")
                {
                    options.Visible = true;
                    menu.Visible = false;
                    currentUI = options;
                }

                if (menu.GetValue() == "Exit")
                    Exit();
            }
            if (iM.JustPressed(Keys.Escape))
            {
                menu.Visible = true;
                options.Visible = false;
                currentUI = menu;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        void setFullScreen()
        {
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
    }
}
