using Godot;
using System;
using System.Collections.Generic;

public class BossLevelManager : LevelManager
{
    private LevelCompletePoint completePoint;
    private Boss finalBoss;

    //Moving Things
    private bool moveBossDoor = false;
    private Spatial bossDoor;
    private readonly Vector3 bossDoorPos = new Vector3(8.367f, 2.991f, -34.965f);

    public override void _Ready()
    {
        base._Ready();
        completePoint = GetNode<LevelCompletePoint>("Level_Complete_Point");
        bossDoor = GetNode<Spatial>("Level_Geometry/NavigationMeshInstance/PrisonDoor2");
        finalBoss = GetNode<Boss>("Boss CEO");

        completePoint.pointActive = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (finalBoss.isAlive == false && !moveBossDoor)
        {
            moveBossDoor = true;
            UI.changeObjective("Be Free");
            changeMusic(0);
            bossDoor.GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Play();
        }

        if (moveBossDoor && bossDoor.Translation.x < 8.3f)
        {
            bossDoor.Translation = bossDoor.Translation.LinearInterpolate(bossDoorPos, delta * 0.54f);
        }
        else 
        {
            moveBossDoor = false;
        }
    }
}
