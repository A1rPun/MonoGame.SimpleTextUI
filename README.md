# MonoGame.SimpleTextUI

Create simple User Interfaces with Monogame.  
Inspired by N++

![](img/SimpleMenu.png)

### Usage

**Creating an instance**

    var menu = new SimpleTextUI(this)
    {
        Font = Content.Load<SpriteFont>("{yourfont}"),
        TextColor = Color.IndianRed,
        SelectedColor = Color.Red,
        Items = new[]
        {
            new TextElement("Singleplayer"),
            new TextElement("Multiplayer"),
            new TextElement("Options"),
            new TextElement("Credits"),
            new TextElement("Exit")
        },
        Align = Alignment.Right
    };

**Navigating through menu's**

Call the `.Move(bool up)` to

    if (inputManager.JustPressed(Keys.Up))
        menu.Move(true);
    else if (inputManager.JustPressed(Keys.Down))
        menu.Move();

**Adding logic to the selection**

    if (inputManager.JustPressed(Keys.Enter))
        if(menu.SelectedIndex == 4)
            Exit();

### TODO

- Optimize code
- Form Elements
- Form set & get
- More Customization

### Licence
MIT
