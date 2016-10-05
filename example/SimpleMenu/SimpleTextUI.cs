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
        public string Caption;
        public Vector2 Position;
        public Color Color;

        public TextElement(string caption)
        {
            Caption = caption;
        }
        public virtual void Draw(SpriteBatch batch, SpriteFont font, Color? color = null)
        {
            batch.DrawString(font, Caption, Position, color ?? Color);
        }

    }

    public class MultiTextElement : TextElement
    {
        public string Text = "";
        public Vector2 TextPosition;
        public MultiTextElement(string caption) : base(caption) { }
        public MultiTextElement(string caption, string text) : base(caption)
        {
            Text = text;
        }
        public override void Draw(SpriteBatch batch, SpriteFont font, Color? color = null)
        {
            batch.DrawString(font, Caption, Position, color ?? Color);
            batch.DrawString(font, Text, TextPosition, color ?? Color);
        }
        public virtual void Update(bool left = false) { }
    }

    public class SelectElement : MultiTextElement
    {
        public string[] Options;
        public int Index = 0;
        public SelectElement(string caption, string[] options) : base(caption)
        {
            Options = options;
            Text = options.Length > 0 ? options[0] : "";
        }
        public override void Update(bool left = false)
        {
            var length = Options.Length;
            if (length == 0) return;
            Index = left ? Index - 1 : Index + 1;
            if (Index < 0)
                Index = 0;
            if (Index >= length)
                Index = length - 1;
            Text = Options[Index];
        }
    }

    public class NumericElement : MultiTextElement
    {
        public float Value;
        public float Max = float.MaxValue;
        public float Min = float.MinValue;
        public float Step;
        public NumericElement(string caption, float value = 0f, float step = 1f) : base(caption)
        {
            Value = value;
            Step = step;
            Text = Value.ToString();
        }
        public override void Update(bool left = false)
        {
            Value = left ? Value - Step : Value + Step;
            if (Value < Min)
                Value = Min;
            if (Value >= Max)
                Value = Max;
            Text = Value.ToString();
        }
    }

    public class SimpleTextUI : DrawableGameComponent
    {
        // Public
        public Color TextColor = Color.LimeGreen;
        public Color SelectedColor = Color.DarkGreen;
        public int Width = 100;
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
            MultiTextElement el;
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
                    el = elements[index] as MultiTextElement;
                    if (el != null)
                        el.Update(true);
                    break;
                case Direction.Right:
                    el = elements[index] as MultiTextElement;
                    if (el != null)
                        el.Update();
                    break;
                default:
                    break;
            }
        }
        public string GetValue()
        {
            return elements[index].Caption;
        }
        // Set the items and update their positions
        public void SetItems(TextElement[] items)
        {
            var pos = getPosition();
            var halfWidth = Width / 2;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                item.Position = pos;
                var size = uiFont.MeasureString(item.Caption);
                var mtext = item as MultiTextElement;
                if (mtext != null)
                {
                    if (align == Alignment.Center)
                    {
                        item.Position.X -= size.X + halfWidth;
                        mtext.TextPosition = new Vector2(pos.X + halfWidth, pos.Y);
                    }
                    else if (align == Alignment.Right)
                    {
                        item.Position.X -= size.X + Width;
                        mtext.TextPosition = new Vector2(pos.X - uiFont.MeasureString(mtext.Text).X, pos.Y);
                    }
                    else
                        mtext.TextPosition = new Vector2(pos.X + Width, pos.Y);
                }
                else
                {
                    if (align == Alignment.Center)
                        item.Position.X += size.X / 2;
                    else if (align == Alignment.Right)
                        item.Position.X += size.X;
                }
                pos.Y += size.Y;
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
                item.Draw(batch, uiFont, color);
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
    }
}
