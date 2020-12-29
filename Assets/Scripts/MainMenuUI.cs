using Godot;
using System;

public class MainMenuUI : Control
{
    //------------Signals
    public void _on_PlayButton_button_down()
    {
        GetTree().ChangeScene("res://Assets/Game_Scenes/Prison Level.tscn");
    }

    public void _on_QuitButton_button_down()
    {
        GetTree().Quit();
    }
}
