using Godot;
using System;
using System.Collections.Generic;

public class PrisonLevelManager : LevelManager
{
    /*
     * This specific level manager controls events for the first level, like opening the cell door when the key is collected or whatnot
     */
    private bool cellDoorOpening = false;
    private bool otherDoorOpening = false;

    //Interaction items
    private Spatial playerCellDoor;
    private readonly Vector3 openDoorPos = new Vector3(9.016f, 0.137f, 12.525f);
    private EnemyAgent enemyWithKey;
    private bool keyGuardDead = false;
    private LevelCompletePoint completePoint;

    private Spatial secondCellDoor;
    private readonly Vector3 otherOpenDoorPos = new Vector3(-7.675f,0,-3.509f);

    private Spatial bigDoorA;
    private Spatial bigDoorB;
    private readonly Vector3 doorPosA = new Vector3(-6.896f, -3.061f, 0);
    private readonly Vector3 doorPosB = new Vector3(6.896f, -3.061f, 0);


    public override void _Ready()
    {
        base._Ready();
        playerCellDoor = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/CSGCombiner/Cell/PrisonDoor");
        secondCellDoor = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/CSGCombiner/Small Connecting Room/PrisonDoor3");
        //Big doors
        bigDoorA = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/CSGCombiner/ExitWay/Door_0");
        bigDoorB = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/CSGCombiner/ExitWay/Door_1");
        completePoint = GetNode<LevelCompletePoint>("Level_Complete_Point");
        enemyWithKey = GetNode<EnemyAgent>("Bad Agent KEY");
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        //Opening the second cell door
        if (otherDoorOpening && secondCellDoor.Translation.z > -4)
        {
            secondCellDoor.Translation = secondCellDoor.Translation.LinearInterpolate(otherOpenDoorPos, delta * 0.54f);
            bigDoorA.Translation = bigDoorA.Translation.LinearInterpolate(doorPosA, delta * 0.34f);
            bigDoorB.Translation = bigDoorB.Translation.LinearInterpolate(doorPosB, delta * 0.34f);
        }
        else 
        {
            otherDoorOpening = false;
        }

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
        switch (item.name)
        {
            case "Cell Key":
                cellDoorOpening = true;
                //Play opening sound
                playerCellDoor.GetNode<AudioStreamPlayer3D>("Audio").Play();
                UI.changeObjective("Find a way out");
                changeMusic(1);
                break;

            case "Vent":
                GunPickup knife = GetNode<GunPickup>("Knife_Pickup");
                Spatial ventCover = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/CSGCombiner/Cell/Vent/VentCover");
                ventCover.Translation = new Vector3(-1.657f, -0.966f, -0.299f);
                knife.canPickUp = true;
                break;

            case "Door_Switch":
                otherDoorOpening = true;
                //Play opening sound
                secondCellDoor.GetNode<AudioStreamPlayer3D>("Audio").Play();
                UI.changeObjective("Leave the prison");
                completePoint.pointActive = true;
                break;
        }
    }
}
