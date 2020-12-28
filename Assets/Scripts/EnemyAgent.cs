using Godot;
using System;

public class EnemyAgent : KinematicBody
{
    [Export]
    private float regSpeed = 5;
    [Export]
    private float runSpeed = 8;
    private bool isAlive = true;

    //Components
    private AnimationPlayer anim;
    private LevelManager level;
    private AudioStreamPlayer3D audio;

    public override void _Ready()
    {
        base._Ready();
        level = (LevelManager) GetParent();
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        audio = GetNode<AudioStreamPlayer3D>("Audio");
    }

    private void Hit()
    {
        if (isAlive)
        {
            anim.Play("Agent_Dead");
            audio.Play();
            isAlive = false;
        }
    }

    public bool getAlive()
    {
        return isAlive;
    }
}
