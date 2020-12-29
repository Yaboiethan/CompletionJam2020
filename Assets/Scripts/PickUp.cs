using Godot;
using Godot;
using System;

public enum pickupType { Health, Ammo}
public class PickUp : Area
{
    //Components
    [Export]
    pickupType mType;
    [Export]
    private Mesh ammoMesh;
    private AudioStreamPlayer3D audio;

    //Stats
    [Export]
    private int toRecover = 10;

    public override void _Ready()
    {
        //Get components
        audio = GetNode<AudioStreamPlayer3D>("Audio");
        //Switch to ammo pickup
        if (mType == pickupType.Ammo)
        {
            GetNode<MeshInstance>("Mesh").Mesh = ammoMesh;
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        RotateY(delta);
    }

    //---------Signals
    public void _on_Medpack_Pickup_body_entered(Node body)
    {
        if (body.Name == "Player")
        {
            Godot.Collections.Array param = new Godot.Collections.Array();
            param.Add(mType.ToString());
            param.Add(toRecover);
            bool success = (bool) body.Callv("pickUpCollected", param);
            if (success)
            {
                audio.Play();
                Visible = false;
                Monitoring = false;
            }
        }
    }

    public void _on_Audio_finished()
    {
        QueueFree();
    }
}
