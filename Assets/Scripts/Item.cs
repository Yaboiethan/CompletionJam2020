using Godot;
using System;

public class Item : RigidBody
{
    /*
     * Items are collectables that have some sort of impact in the levels and cause an effect
     * examples: keys, switches, etc
     */

    [Export]
    public string name;
    [Export]
    private bool destoryOnUse = true;
    [Export]
    public bool canUse = true;

    private bool playerInRange = false;
    private LevelManager level;
    private Area area;
    private AudioStreamPlayer3D audio;

    public override void _Ready()
    {
        base._Ready();
        area = GetNode<Area>("Area");
        level = (LevelManager) GetTree().Root.GetChild(0);
        audio = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        //Connect enter and exit signals
        area.Connect("body_entered", this, nameof(_on_Area_body_entered));
        area.Connect("body_exited", this, nameof(_on_Area_body_exited));
        audio.Connect("finished", this, nameof(_on_AudioStreamPlayer3D_finished));
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("Interact") && playerInRange && canUse)
        {
            //Call levelmanager
            level.onItemCollect(this);
            //Play audio
            audio.Play();
            //Adjust
            canUse = false;
            if (destoryOnUse)
            {
                Visible = false;
            }
        }
    }

    public void _on_Area_body_entered(Node body)
    {
        if (body.Name == "Player")
        {
            playerInRange = true;
        }
    }

    public void _on_Area_body_exited(Node body)
    {
        if (body.Name == "Player")
        {
            playerInRange = false;
        }
    }

    public void _on_AudioStreamPlayer3D_finished()
    {
        if(destoryOnUse)
        {
            QueueFree();
        }
    }
}
