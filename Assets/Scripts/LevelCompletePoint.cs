using Godot;
using System;

public class LevelCompletePoint : Area
{
    private UI uiManager;
    public bool pointActive = false;
    public override void _Ready()
    {
        base._Ready();
        uiManager = GetTree().Root.GetChild(0).GetNode<UI>("UI");
        Connect("body_entered", this, nameof(_on_Level_Complete_body_entered));
    }

    public void _on_Level_Complete_body_entered(Node body)
    {
        if(body.Name == "Player" && pointActive)
        {
            //Disable movement
            body.Set("canMove", false);
            //Turn on ui
            uiManager.levelComplete();
        }
    }
}
