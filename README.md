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

**Navigating through menus**

Call the `.Move(Direction)` to move the displayed selection a direction.  
Example in combination with [MonoGame.InputManager](https://github.com/A1rPun/MonoGame.InputManager);


    if (inputManager.JustPressed(Keys.Up))
        menu.Move(Direction.Up);
    else if (inputManager.JustPressed(Keys.Down))
        menu.Move(Direction.Down);
    else if (iM.JustPressed(Keys.Left))
        menu.Move(Direction.Left);
    else if (iM.JustPressed(Keys.Right))
        menu.Move(Direction.Right);

**Adding logic to the selection**
When the value of the menu is "Exit"

    if (inputManager.JustPressed(Keys.Enter))
        if(menu.GetCurrentCaption() == "Exit")
            Exit();

You can get all the current values like this

    if (inputManager.JustPressed(Keys.Esc))
        var options = menu.GetValues();

### TODO

- Form setValues

### Licence
MIT
