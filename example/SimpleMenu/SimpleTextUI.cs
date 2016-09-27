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
        public Vector2 Size;
        public Vector2 Position;
        public Vector2 Origin;
        public Color Color;

        public TextElement(string text)
        {
            Text = text;
        }
        public void Draw(SpriteBatch batch, SpriteFont font, Color? color = null, float scale = 1f, SpriteEffects spriteEffect = SpriteEffects.None, float layerDepth = 0f)
        {
            batch.DrawString(font, Text, Position, color ?? Color, 0, Origin, scale, spriteEffect, layerDepth);
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
        public float Scale = 1f;
        public Color TextColor = Color.LimeGreen;
        public Color SelectedColor = Color.DarkGreen;
        public SpriteEffects SpriteEffect;
        public Alignment Align
        {
            get { return align; }
            set
            {
                align = value;
                SetItems(elements);
            }
        }
        public Vector2 Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                SetItems(elements);
            }
        }
        // Private
        TextElement[] elements;
        SpriteFont uiFont;
        SpriteBatch batch;
        Alignment align;
        Vector2 padding;
        int index;

        // Constructors
        public SimpleTextUI(Game game, SpriteFont font) : base(game)
        {
            batch = new SpriteBatch(Game.GraphicsDevice);
            padding = new Vector2(100);
            uiFont = font;
        }
        public SimpleTextUI(Game game, SpriteFont font, string[] items) : this(game, font)
        {
            if (items != null)
            {
                var l = items.Length;
                var newItems = new TextElement[l];
                for (int i = 0; i < l; i++)
                    newItems[i] = new TextElement(items[i]);
                SetItems(newItems);
            }
        }
        public SimpleTextUI(Game game, SpriteFont font, TextElement[] items) : this(game, font)
        {
            if (items != null)
                SetItems(items);
        }
        // Move index up or down
        public void Move(Direction dir = Direction.Down)
        {
            switch (dir)
            {
                case Direction.Up:
                    index--;
                    if (index < 0)
                        index = elements.Length - 1;
                    break;
                case Direction.Down:
                    index++;
                    if (index >= elements.Length)
                        index = 0;
                    break;
                case Direction.Left:
                    // get item on selectedindex and move index SelectElement
                    break;
                case Direction.Right:
                    break;
                default:
                    break;
            }
        }
        public string GetValue()
        {
            return elements[index].Text;
        }
        // Set the items and update their positions
        public void SetItems(TextElement[] items)
        {
            var pos = getPosition();
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                item.Size = uiFont.MeasureString(item.Text);
                item.Origin = getOrigin(item.Size);
                item.Position = pos;
                pos.Y += item.Size.Y;
            }
            elements = items;
        }
        // Draw the SimpleTextUI in it's full glory
        public override void Draw(GameTime gameTime)
        {
            batch.Begin();
            for (int i = 0; i < elements.Length; i++)
            {
                var item = elements[i];
                var color = i == index ? SelectedColor : TextColor;
                item.Draw(batch, uiFont, color, Scale, SpriteEffect);
            }
            batch.End();
            base.Draw(gameTime);
        }
        // Get the position based on the alignment
        private Vector2 getPosition()
        {
            if (align == Alignment.Center)
                return new Vector2(GraphicsDevice.Viewport.Width / 2, padding.Y);
            else if (align == Alignment.Right)
                return new Vector2(GraphicsDevice.Viewport.Width - padding.X, padding.Y);
            return padding;
        }
        // Get the text origin based on alignment
        private Vector2 getOrigin(Vector2 size)
        {
            Vector2 origin = Vector2.Zero;
            if (align == Alignment.Center)
                origin.X = size.X / 2;
            else if (align == Alignment.Right)
                origin.X = size.X;
            return origin;
        }
    }
}
