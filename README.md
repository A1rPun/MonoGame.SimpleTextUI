# MonoGame.SimpleTextUI

Create simple User Interfaces with Monogame.  
Inspired by N++ (metanetsoftware)

![](img/SimpleMenu.png)

### Usage

**Creating an instance**

    var font = Content.Load<SpriteFont>("{yourfont}");
    var items = new[] { "Singleplayer", "Multiplayer", "Options", "Credits", "Exit" };
    var menu = new SimpleTextUI(this, font, items)
    {
        TextColor = Color.IndianRed,
        SelectedColor = Color.Red,
        Align = Alignment.Right
    };

**Navigating through menu's**

Call the `.Move(Direction)` to move the displayed selection a direction.  
Example in combination with [MonoGame.InputManager](https://github.com/A1rPun/MonoGame.InputManager);


    if (inputManager.JustPressed(Keys.Up))
        menu.Move(Direction.Up);
    else if (inputManager.JustPressed(Keys.Down))
        menu.Move(Direction.Down);

**Adding logic to the selection**

    if (inputManager.JustPressed(Keys.Enter))
        if(menu.GetValue() == "Exit")
            Exit();

### TODO

- Form Elements
- Form set & get
- More Customization

### Licence
MIT
