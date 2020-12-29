using Godot;
using System;

public class MainMenuUI : Control
{
    //Menu References
    private Control mainMenu; //We want to hide/show this depending on what sub menu is up
    //Sub-Menus
    private Control controlsMenu;
    private Control creditsMenu;

    public override void _Ready()
    {
        base._Ready();
        //Find menus
        mainMenu = GetNode<Control>("Buttons_Container");
        controlsMenu = GetNode<Control>("Controls_Container");
        creditsMenu = GetNode<Control>("Credits_Container");
        //Hide submenus
        controlsMenu.Hide();
        creditsMenu.Hide();
    }

    //------------Signals
    public void _on_PlayButton_button_down()
    {
        GetTree().ChangeScene("res://Assets/Game_Scenes/Prison Level.tscn");
    }

    public void _on_ControlsButton_button_down()
    {
        controlsMenu.Show();
        mainMenu.Hide();
    }

    public void _on_CreditsButton_button_down()
    {
        creditsMenu.Show();
        mainMenu.Hide();
    }


    public void _on_ReturnButton_button_down()
    {
        creditsMenu.Hide();
        controlsMenu.Hide();
        mainMenu.Show();
    }

    public void _on_QuitButton_button_down()
    {
        GetTree().Quit();
    }
}
