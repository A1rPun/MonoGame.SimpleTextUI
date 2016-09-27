using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace A1r.SimpleTextUI
{
    public enum Alignment
    {
        Left,
        Right,
        Center
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class TextElement
    {
        public string Text;
        public TextElement(string text)
        {
            Text = text;
        }
    }

    public class SelectElement : TextElement
    {
        public string[] Options;
        public int Index = 0;
        public SelectElement(string text, string[] options) : base(text)
        {
            Options = options;
        }
    }

    public class SimpleTextUI : DrawableGameComponent
    {
        // Public
        public SpriteFont Font;
        public float Scale = 1f;
        public Color TextColor = Color.LimeGreen;
        public Color SelectedColor = Color.DarkGreen;
        public TextElement[] Items;
        public Alignment Align;
        public int SelectedIndex;
        public Vector2 Padding;
        public SpriteEffects SpriteEffect;
        // Private
        SpriteBatch spriteBatch;
        
        float layerDepth = 1f;
        // Constructor
        public SimpleTextUI(Game game) : base(game)
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            Padding = new Vector2(100);
        }
        // Get the position based on the alignment
        private Vector2 getPosition()
        {
            if (Align == Alignment.Center)
                return new Vector2(GraphicsDevice.Viewport.Width / 2, Padding.Y);
            else if (Align == Alignment.Right)
                return new Vector2(GraphicsDevice.Viewport.Width - Padding.X, Padding.Y);
            return Padding;
        }
        // Get the text origin based on alignment
        private Vector2 getOrigin(string text)
        {
            Vector2 origin = Vector2.Zero;
            if (Align == Alignment.Center)
            {
                Vector2 size = Font.MeasureString(text);
                origin.X = size.X / 2;
            }
            else if (Align == Alignment.Right)
            {
                Vector2 size = Font.MeasureString(text);
                origin.X = size.X;
            }
            return origin;
        }
        // Move index up or down
        public void Move(Direction dir)
        {
            
            switch (dir)
            {
                case Direction.Up:
                    SelectedIndex--;
                    if (SelectedIndex < 0)
                        SelectedIndex = Items.Length - 1;
                    break;
                case Direction.Down:
                    SelectedIndex++;
                    if (SelectedIndex >= Items.Length)
                        SelectedIndex = 0;
                    break;
                case Direction.Left:
                    // get item on selectedindex and move index
                    break;
                case Direction.Right:
                    break;
                default:
                    break;
            }
        }
        // Draw the SimpleTextUI in it's full glory
        public override void Draw(GameTime gameTime)
        {
            var pos = getPosition();
            spriteBatch.Begin();
            for (int i = 0; i < Items.Length; i++)
            {
                var text = Items[i].Text;
                var color = i == SelectedIndex ? SelectedColor : TextColor;
                spriteBatch.DrawString(Font, text, pos, color, 0, getOrigin(text), Scale, SpriteEffect, layerDepth);
                pos.Y += Font.MeasureString(text).Y;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
