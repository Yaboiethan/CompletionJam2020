using Godot;
using System;

public class Knife : Weapon
{
    //Stats
    [Export]
    private float stabRate = 3;

    //Components
    private AudioStreamPlayer audio;


    public override void _Ready()
    {
        base._Ready();
        audio = GetNode<AudioStreamPlayer>("Audio");
        if (inUse)
        {
            anim = GetParent().GetParent().GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("Gun_Shoot") && canUse && inUse)
        {
            //Play animation
            anim.Play("Knife_Stab");
            //Play sound
            audio.Play();
            curCooldown = stabRate;

            canUse = false;
        }
    }

    //-----------Signals
    public void _on_Knife_body_entered(Node body)
    {
        if (body is EnemyAgent && body.HasMethod("Hit"))
        {
            body.Call("Hit");
        }
    }
}
