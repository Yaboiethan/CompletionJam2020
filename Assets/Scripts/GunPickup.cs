using Godot;
using System;

public class GunPickup : Area
{

    [Export]
    private PackedScene gunOverride;
    [Export]
    public bool canPickUp = true;

    private Weapon gunToGet;
    private AudioStreamPlayer3D audio;
    private bool swapped;

    public override void _Ready()
    {
        base._Ready();
        gunToGet = (Weapon) GetChild(0);
        audio = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
        if (gunOverride != null)
        {
            GetChild(0).QueueFree();
            Node instance = gunOverride.Instance();
            AddChild(instance);
            gunToGet = (Weapon)instance;
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        RotateY(delta);
    }

    //--------Method Functions
    public void gunSwap(Weapon w)
    {
        w.GetParent().RemoveChild(w);
        AddChild(w);
        //Update ui
        if(gunToGet is Gun)
        {
            Gun g = (Gun) gunToGet;
            gunToGet.uiManager.updateAmmoUI(g.getCurClip(), g.getCurAmmo());
        }
        else
        {
            gunToGet.uiManager.updateAmmoUI();
        }
        gunToGet = w;
        Visible = true;
        w.inUse = false;
        swapped = true;
    }

    //--------Signals
    public void _on_Pistol_Pickup_body_entered(Node body)
    {
        if (body.Name == "Player" && canPickUp)
        {
            gunToGet.inUse = true;
            body.Call("pickUpGun", gunToGet);
            //Play pickup sound
            audio.Play();
            Visible = false;
            //Update ui
            if(gunToGet is Gun)
            {
                Gun g = (Gun) gunToGet;
                gunToGet.uiManager.updateAmmoUI(g.getCurClip(), g.getCurAmmo());
            }
            else
            {
                gunToGet.uiManager.updateAmmoUI();
            }
        }
    }

    public void _on_AudioStreamPlayer3D_finished()
    {
        if (!swapped)
        {
            QueueFree();
        }
    }
}
