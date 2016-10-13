using System;
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
        public Vector2 Size;
        public Color Color;
        public TextElement(string caption, Color? color = null)
        {
            Caption = caption;
            Color = color ?? default(Color);
        }
        public virtual void Draw(SpriteBatch batch, SpriteFont font, Color? color = null)
        {
            batch.DrawString(font, Caption, Position, color ?? Color);
        }
        public virtual string GetValue() { return Caption; }
        public virtual void SetValue(string text) { Caption = text; }
    }

    public class MultiTextElement : TextElement
    {
        public string Text = "";
        public Vector2 TextPosition;
        public Vector2 TextSize;
        public MultiTextElement(string caption) : base(caption) { }
        public MultiTextElement(string caption, string text, Color? color = null) : base(caption, color)
        {
            Text = text;
        }
        public override void Draw(SpriteBatch batch, SpriteFont font, Color? color = null)
        {
            batch.DrawString(font, Caption, Position, color ?? Color);
            batch.DrawString(font, Text, TextPosition, color ?? Color);
        }
        public virtual void Update(bool left = false) { }
        public override string GetValue() { return Text; }
        public override void SetValue(string text) { Text = text; }
    }

    public class SelectElement : MultiTextElement
    {
        public string[] Options;
        public int Index;
        public SelectElement(string caption, string[] options, string value = "") : base(caption)
        {
            Options = options;
            SetValue(value);
        }
        public override void Update(bool left = false)
        {
            setIndex(left ? Index - 1 : Index + 1);
        }
        private void setIndex(int index)
        {
            var length = Options.Length;
            if (length > 0)
            {
                if (index < 0)
                    index = 0;
                else if (index >= length)
                    index = length - 1;
                Index = index;
                Text = Options[Index];
            }
            else
            {
                Index = 0;
                Text = "";
            }
        }
        public override void SetValue(string text)
        {
            setIndex(text == string.Empty ? 0 : Array.IndexOf(Options, text));
        }
    }

    public class NumericElement : MultiTextElement
    {
        public float Value;
        public float Max;
        public float Min;
        public float Step;
        public int Decimals;
        public NumericElement(string caption, float value = 0f, int decimals = 0,
            float min = float.MinValue, float max = float.MaxValue, float step = 1f) : base(caption)
        {
            Decimals = decimals;
            Min = min;
            Max = max;
            Step = step;
            setValue(value);
        }
        public override void Update(bool left = false)
        {
            var val = left ? Value - Step : Value + Step;
            if (val < Min)
                val = Min;
            if (val >= Max)
                val = Max;
            setValue(val);
        }
        private void setValue(float val)
        {
            Value = (float)decimal.Round((decimal)val, Decimals);
            Text = Value.ToString();
        }
        public override void SetValue(string text)
        {
            float val;
            if (float.TryParse(text, out val))
                setValue(val);
        }
    }

    public class SimpleTextUI : DrawableGameComponent
    {
        public Color TextColor = Color.LightGreen;
        public int Width = 100;
        public Alignment Align
        {
            get { return align; }
            set
            {
                align = value;
                Reflow();
            }
        }
        public Vector2 Padding
        {
            get { return padding; }
            set
            {
                padding = value;
                Reflow();
            }
        }
        public TextElement SelectedElement
        {
            get { return selectedElement; }
            set
            {
                selectedElement = value;
                Reflow();
            }
        }
        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;
                Reflow();
            }
        }
        SpriteFont _font;
        TextElement[] elements;
        SpriteBatch batch;
        Alignment align;
        Vector2 padding;
        int index;
        TextElement selectedElement;

        // Constructors
        public SimpleTextUI(Game game, SpriteFont font) : base(game)
        {
            batch = new SpriteBatch(Game.GraphicsDevice);
            padding = new Vector2(100);
            _font = font;
            selectedElement = new TextElement("", Color.LimeGreen);
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
                    Reflow();
                    break;
                case Direction.Right:
                    el = elements[index] as MultiTextElement;
                    if (el != null)
                        el.Update();
                    Reflow();
                    break;
                default:
                    break;
            }
        }

        public void Move(Point point)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                var el = elements[i];
                var mtext = el as MultiTextElement;
                var size = el.Size;
                if (mtext != null)
                    size.X += mtext.TextSize.X + Width;
                var rectangle = new Rectangle((int)el.Position.X, (int)el.Position.Y, (int)size.X, (int)size.Y);
                if (rectangle.Contains(point))
                {
                    index = i;
                    break;
                }
            }
        }
        // Get the current caption
        public string GetCurrentCaption()
        {
            return elements[index].Caption;
        }
        public string GetCurrentValue()
        {
            return elements[index].GetValue();
        }
        // Get all the current captions or values
        public string[] GetValues()
        {
            var length = elements.Length;
            var result = new string[length];
            for (int i = 0; i < length; i++)
                result[i] = elements[i].GetValue();
            return result;
        }
        public void SetValues(string[] values)
        {
            var length = values.Length;
            for (int i = 0; i < length; i++)
                elements[i].SetValue(values[i]);
        }

        public void Reflow()
        {
            if (selectedElement != null)
            {
                var selel = selectedElement as MultiTextElement;
                if (selel != null)
                    selel.TextSize = _font.MeasureString(selel.Text);
                selectedElement.Size = _font.MeasureString(selectedElement.Caption);
            }
            SetItems(elements);
        }
        // Set the items and update their positions
        public void SetItems(TextElement[] items)
        {
            var pos = getPosition();
            var halfWidth = Width / 2;
            var selsize = selectedElement == null ? 0 : selectedElement.Size.X;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                item.Position = pos;
                item.Size = _font.MeasureString(item.Caption);
                var mtext = item as MultiTextElement;
                if (mtext != null)
                {
                    mtext.TextSize = _font.MeasureString(mtext.Text);
                    if (align == Alignment.Center)
                    {
                        item.Position.X -= item.Size.X + halfWidth;
                        mtext.TextPosition = new Vector2(pos.X + halfWidth, pos.Y);
                    }
                    else if (align == Alignment.Right)
                    {
                        item.Position.X -= item.Size.X + Width;
                        mtext.TextPosition = new Vector2(pos.X - mtext.TextSize.X, pos.Y);
                    }
                    else
                        mtext.TextPosition = new Vector2(pos.X + Width, pos.Y);
                }
                else
                {
                    if (align == Alignment.Center)
                        item.Position.X -= item.Size.X / 2;
                    else if (align == Alignment.Right)
                        item.Position.X -= item.Size.X + selsize;
                    else
                        item.Position.X += selsize;
                }
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
                var color = TextColor;
                if (selectedElement != null && i == index)
                {
                    color = selectedElement.Color;
                    selectedElement.Position = item.Position;
                    selectedElement.Position.X -= selectedElement.Size.X;
                    var selel = selectedElement as MultiTextElement;
                    if (selel != null)
                    {
                        selel.TextPosition = item.Position;
                        selel.TextPosition.X += item.Size.X;
                    }
                    selectedElement.Draw(batch, _font);
                }
                item.Draw(batch, _font, color);
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
