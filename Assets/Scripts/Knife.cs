using Godot;
using System;

public class Knife : Weapon
{
    //Stats
    [Export]
    private float stabRate = 3;

    private bool inStab = false;

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
            inStab = false;
            //Play animation
            anim.Play("Knife_Stab");
            //Play sound
            audio.Play();
            curCooldown = stabRate;

            canUse = false;
        }
    }

    public override void setAnimator(Node holder)
    {
        base.setAnimator(holder);
        //Connect signals
        anim.Connect("animation_started", this, nameof(_on_AnimationPlayer_animation_started));
        anim.Connect("animation_finished", this, nameof(_on_AnimationPlayer_animation_finished));
    }

    //-----------Signals
    public void _on_Knife_body_entered(Node body)
    {
        if (body is EnemyAgent && body.HasMethod("Hit") && inStab)
        {
            body.Call("Hit");
        }
    }

    public void _on_AnimationPlayer_animation_started(string animName)
    {
        if (animName == "Knife_Stab")
        {
            inStab = true;
        }
    }

    public void _on_AnimationPlayer_animation_finished(string animName)
    {
        if (animName == "Knife_Stab")
        {
            inStab = false;
        }
    }
}
