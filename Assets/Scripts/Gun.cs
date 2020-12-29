using Godot;
using System;
using System.Collections.Generic;

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
    public bool usedByEnemy = false;

    private Spatial firePoint;
    private List<Spatial> additionalFirePoints = new List<Spatial>();
    private Particles smokeParticles;
    private AudioStreamPlayer3D audio;

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
        audio = GetNode<AudioStreamPlayer3D>("AudioPlayer");
        //Set ammo vars
        curAmmo = ammoMax;
        curClip = ammoCapacity;
        //Get other firepoints
        foreach(Node n in GetChildren())
        {
            if(n.Name.Find("Fire_Point") != -1 && n != firePoint)
            {
                additionalFirePoints.Add((Spatial)n);
            }
        }
    }

    public void onEnemySelection(AnimationPlayer a)
    {
        anim = a;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        //Check if enemy gun
        if (usedByEnemy)
        {
            return;
        }
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
            spawnBullet(false);
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

    /*
     * This function is exclusively for NPC characters, it allows them to shoot without consuming ammo
     */
    public virtual void Shoot()
    {
        if (canUse)
        {
            //Spawn in bullet instance and adjust
            spawnBullet(true);
            //Play sound
            audio.Stream = audioClips[0];
            audio.Play();
            //Play particles
            smokeParticles.Restart();
            //Play animation
            anim.Play("Gun_Recoil");
            canUse = false;
            curCooldown = fireRate;
        }
    }

    private void spawnBullet(bool enemy)
    {
        for (int i = 0; i < 1 + additionalFirePoints.Count; i++)
        {
            Bullet b = (Bullet)bulletInstance.Instance();
            //Random rotation if not only shot
            Vector3 rot = ((Spatial)GetParent().GetParent()).Rotation;

            if (i == 0)
            {
                b.Start(ToGlobal(firePoint.Translation), rot);
            }
            else 
            {
                rot += additionalFirePoints[i - 1].Rotation;
                b.Start(ToGlobal(additionalFirePoints[i - 1].Translation), rot);
            }
            GetTree().Root.GetChild(0).AddChild(b);

            if (enemy)
            {
                b.playerDamage = ((EnemyAgent)GetParent().GetParent()).damage;
                b.enemyFire = true;
            }
        }
    }

    public bool canShoot()
    {
        return canUse;
    }

    public void addBullets(int b)
    {
        if (curAmmo + b > ammoMax)
        {
            curAmmo = ammoMax;
        }
        else
        {
            curAmmo += b;
        }
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
            updateUI();
            canUse = true;
        }
    }
}
