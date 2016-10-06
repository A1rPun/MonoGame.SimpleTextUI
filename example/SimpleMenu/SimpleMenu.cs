using A1r.Input;
using A1r.SimpleTextUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

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
        SpriteFont big;
        SpriteFont small;
        Texture2D cursor;

        public SimpleMenu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            setFullScreen();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            big = Content.Load<SpriteFont>("Big");
            small = Content.Load<SpriteFont>("Small");
            cursor = Content.Load<Texture2D>("img/cursor");
            menu = new SimpleTextUI(this, big, new[] { "Singleplayer", "Multiplayer", "Options", "Credits", "Exit" })
            {
                Align = Alignment.Right,
                TextColor = Color.IndianRed,
                SelectedColor = Color.Red
            };
            options = new SimpleTextUI(this, big, new TextElement[]
            {
                new SelectElement("Video", new[] { "Fullscreen", "Windowed" }),
                new SelectElement("Alignment", new[] { "Left", "Center", "Right" }, 1),
                new NumericElement("Music", 0.02f, 3, 0f, 1f, 0.01f),
                new NumericElement("SFX", 1337, 0, -1337, 13371337, 1337),
                new SelectElement("Difficulty", new[] { "Chicken", "Casual", "Ragequit PLOS" }, 1),
                new SelectElement("Numberwang", new[] { "2", "29", "42", "69", "1336.9", "1337" }, 2),
                new TextElement("Back")
            })
            {
                //Width = 600,
                Align = Alignment.Center,
                Visible = false,
            };
            currentUI = menu;
            iM = new InputManager(this);

            Components.Add(iM);
        }

        private void Back()
        {
            if (currentUI == menu)
                Exit();
            menu.Visible = true;
            options.Visible = false;
            currentUI = menu;
        }

        protected override void Update(GameTime gameTime)
        {
            // Move the menu if the cursor is on an item
            if (iM.IsMouseMoved())
                currentUI.Move(iM.GetMousePosition());
            // Move the menu with keys
            if (iM.JustPressed(Keys.Up))
                currentUI.Move(Direction.Up);
            else if (iM.JustPressed(Keys.Down))
                currentUI.Move(Direction.Down);
            else if (iM.JustPressed(Keys.Left))
            {
                currentUI.Move(Direction.Left);
                currentUI.Reflow();
            }
            else if (iM.JustPressed(Keys.Right))
            {
                currentUI.Move(Direction.Right);
                currentUI.Reflow();
            }
            // Implement logic based on the selected action
            if (iM.JustPressed(Keys.Enter) || iM.JustPressed(MouseInput.LeftButton))
            {
                if (currentUI == menu)
                {
                    if (currentUI.GetCurrentCaption() == "Exit")
                        Exit();
                    else if (currentUI.GetCurrentCaption() == "Options")
                    {
                        options.Visible = true;
                        menu.Visible = false;
                        currentUI = options;
                    }
                }
                else
                {
                    if (currentUI.GetCurrentCaption() == "Video")
                        setFullScreen(currentUI.GetCurrentValue() == "Fullscreen");
                    else if (currentUI.GetCurrentCaption() == "Alignment")
                    {
                        var alignment = Alignment.Center;
                        var width = 100;
                        if (currentUI.GetCurrentValue() == "Left")
                        {
                            width = 680;
                            alignment = Alignment.Left;
                        }
                        else if (currentUI.GetCurrentValue() == "Right")
                        {
                            width = 500;
                            alignment = Alignment.Right;
                        }
                        currentUI.Width = width;
                        currentUI.Align = alignment;
                    }
                    else if (currentUI.GetCurrentCaption() == "Back")
                        Back();
                }
            }
            if (iM.JustPressed(Keys.Escape))
                Back();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            currentUI.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.Draw(cursor, iM.GetMousePosition().ToVector2(), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        void setFullScreen(bool on = true)
        {
            if (on)
            {
                //graphics.IsFullScreen = true;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                if (menu != null)
                    menu.Font = big;
                if (options != null)
                    options.Font = big;
            }
            else
            {
                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 600;
                if (menu != null)
                    menu.Font = small;
                if (options != null)
                    options.Font = small;
            }
            graphics.ApplyChanges();
            if (menu != null)
                menu.Reflow();
            if (options != null)
                options.Reflow();
        }
    }
}
