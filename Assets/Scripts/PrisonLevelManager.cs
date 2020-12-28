using Godot;
using System;

public class PrisonLevelManager : LevelManager
{
    /*
     * This specific level manager controls events for the first level, like opening the cell door when the key is collected or whatnot
     */
    private bool cellDoorOpening = false;

    //Interaction items
    private Spatial playerCellDoor;
    private readonly Vector3 openDoorPos = new Vector3(9.016f, 0.137f, 12.525f);
    private EnemyAgent enemyWithKey;
    private bool keyGuardDead = false;

    public override void _Ready()
    {
        base._Ready();
        playerCellDoor = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/CSGCombiner/Cell/PrisonDoor");
        enemyWithKey = GetNode<EnemyAgent>("Bad Agent KEY");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        //Opening the player's cell door
        if (cellDoorOpening && playerCellDoor.Translation.z < 12)
        {
            playerCellDoor.Translation = playerCellDoor.Translation.LinearInterpolate(openDoorPos, delta * 0.54f);
        }
        else 
        {
            cellDoorOpening = false;
        }

        //Check if guard with key is dead, if so allow key to be used
        if (!enemyWithKey.getAlive() && !keyGuardDead)
        {
            Spatial key = GetNode<RigidBody>("Key_Collectable");
            key.Visible = true;
            ((Item)key).canUse = true;
            keyGuardDead = true;
        }
    }

    public override void onItemCollect(Item item)
    {
        base.onItemCollect(item);
        GD.Print(item.name);
        switch (item.name)
        {
            case "Cell Key":
                cellDoorOpening = true;
                //Play opening sound
                playerCellDoor.GetNode<AudioStreamPlayer3D>("Audio").Play();
                break;

            case "Vent":
                GunPickup knife = GetNode<GunPickup>("Knife_Pickup");
                Spatial ventCover = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/CSGCombiner/Cell/Vent/VentCover");
                ventCover.Translation = new Vector3(-1.657f, -0.966f, -0.299f);
                //Comething with dialog
                knife.canPickUp = true;
                break;
        }
    }
}
