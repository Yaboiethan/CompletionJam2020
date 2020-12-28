using Godot;
using System;

public class Bullet : KinematicBody
{
    [Export]
    private float speed = 15;

    private AudioStreamPlayer3D audio;

    public override void _Ready()
    {
        base._Ready();
        audio = GetNode<AudioStreamPlayer3D>("Audio");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        //If exists, move in forward direction
        var collision = MoveAndCollide(-Transform.basis.z * speed);
        if (collision != null)
        {
            audio.Play();
            if (collision.Collider.HasMethod("Hit"))
            {
                collision.Collider.Call("Hit");
            }
            QueueFree();
        }
    }

    public void Start(Vector3 pos, Vector3 rot)
    {
        Translation = pos;
        Rotation = rot;
    }

    //---------Signals
    public void _on_Timer_timeout() //Emergency Destroy
    {
        QueueFree();
    }

    public void _on_Audio_finished()
    {
        QueueFree();
    }
}
