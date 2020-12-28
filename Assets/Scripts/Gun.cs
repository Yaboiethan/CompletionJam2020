using Godot;
using System;

public class Gun : Weapon
{
    //Export Variables
    [Export]
    private PackedScene bulletInstance;

    //Stats
    [Export]
    private float fireRate = 3; //How often the gun can be fired
    [Export]
    private int ammoCapacity = 7; //How many bullets are in a clip


    private Spatial firePoint;
    private Particles smokeParticles;
    private AudioStreamPlayer audio;

    public override void _Ready()
    {
        base._Ready();
        firePoint = GetNode<Spatial>("Fire_Point");
        smokeParticles = GetNode<Particles>("Smoke_Particles");
        if (inUse)
        {
            anim = GetParent().GetParent().GetParent().GetNode<AnimationPlayer>("AnimationPlayer");
        }
        audio = GetNode<AudioStreamPlayer>("AudioPlayer");
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        //Check if fire gun
        if (@event.IsActionPressed("Gun_Shoot") && canUse && inUse)
        {
            //Spawn in bullet instance and adjust
            var b = (Bullet)bulletInstance.Instance();
            b.Start(ToGlobal(firePoint.Translation), ((Spatial)GetParent().GetParent()).Rotation);
            GetTree().Root.GetChild(0).AddChild(b);
            //Play sound
            audio.Play();
            //Play particles
            smokeParticles.Restart();
            //Play animation
            anim.Play("Gun_Recoil");
            canUse = false;
            curCooldown = fireRate;
        }
    }
}
