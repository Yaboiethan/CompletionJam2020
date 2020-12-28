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
    [Export]
    private int ammoMax = 21; //Max ammo available

    private int curAmmo;
    private int curClip;


    private Spatial firePoint;
    private Particles smokeParticles;
    private AudioStreamPlayer audio;

    [Export]
    private AudioStream[] audioClips; //0 == fire sound, 1 == reload sound, 2 == jam sound

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
        //Set ammo vars
        curAmmo = ammoMax;
        curClip = ammoCapacity;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        //Check if fire gun
        if (@event.IsActionPressed("Gun_Shoot") && canUse && inUse)
        {
            //Check if have bullets
            if(curClip == 0)
            {
                //Check if have more ammo
                if(curAmmo > 0)
                {
                    //Reload
                    canUse = false;
                    audio.Stream = audioClips[1];
                    audio.Play();
                    anim.Play("Gun_Reload");
                    return;
                }
                else
                {
                    //Play jammed sound
                    audio.Stream = audioClips[2];
                    audio.Play();
                    anim.Play("Gun_Recoil");
                    return;
                }
            }
            //Spawn in bullet instance and adjust
            var b = (Bullet)bulletInstance.Instance();
            b.Start(ToGlobal(firePoint.Translation), ((Spatial)GetParent().GetParent()).Rotation);
            GetTree().Root.GetChild(0).AddChild(b);
            //Play sound
            audio.Stream = audioClips[0];
            audio.Play();
            //Play particles
            smokeParticles.Restart();
            //Play animation
            anim.Play("Gun_Recoil");
            canUse = false;
            curCooldown = fireRate;
            curClip--;
            uiManager.updateAmmoUI(curClip, curAmmo);
        }
        else if (@event.IsActionPressed("Gun_Reload") && inUse && canUse)
        {
            //If ammo is max, ignore
            if(curClip == ammoCapacity)
            {
                return;
            }
            //Manual reload
            canUse = false;
            audio.Stream = audioClips[1];
            audio.Play();
            anim.Play("Gun_Reload");
        }
    }

    public override void setAnimator(Node holder)
    {
        base.setAnimator(holder);
        anim.Connect("animation_finished", this, nameof(_on_AnimationPlayer_Animation_Finished));
    }

    public void updateUI()
    {
        uiManager.updateAmmoUI(curClip, curAmmo);
    }

    //-------------Member Functions
    public int getCurAmmo()
    {
        return curAmmo;
    }

    public int getCurClip()
    {
        return curClip;
    }

    //-------------Signals
    public void _on_AnimationPlayer_Animation_Finished(string animName)
    {
        if(animName == "Gun_Reload")
        {
            int ammoToLoad = (ammoCapacity - curClip);
            if(curAmmo >= ammoToLoad)
            {
                curClip += ammoToLoad;
                curAmmo -= ammoToLoad;
            }
            else
            {
                curClip = curAmmo;
                curAmmo = 0;
            }
            //Update UI
            uiManager.updateAmmoUI(curClip, curAmmo);
            canUse = true;
        }
    }
}
