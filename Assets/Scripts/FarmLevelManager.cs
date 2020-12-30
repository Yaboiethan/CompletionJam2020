using Godot;
using System;
using System.Collections.Generic;

public class FarmLevelManager : LevelManager
{
    //Tracker vars
    private int keysCollected = 0;
    private List<EnemyAgent> secretEnemies = new List<EnemyAgent>();
    private LevelCompletePoint completePoint;

    private Spatial endGateA;
    private Spatial endGateB;
    private bool openedGate = false;

    private readonly Vector3 gateAPos = new Vector3(-11.92f, -0.003f, 7.727f);
    private readonly Vector3 gateBPos = new Vector3(5.497f, -0.003f, -9.932f);

    public override void _Ready()
    {
        base._Ready();
        //Get the secret baddies
        for(int i = 0; i < 4; i++)
        {
            secretEnemies.Add(GetNode<EnemyAgent>("Secret Baddie" + (i + 1)));
            secretEnemies[i].GetNode<CollisionShape>("CollisionShape").Disabled = true;
        }
        //Get Doors
        endGateA = GetNode<Spatial>("Level_Geometry/EndGate/GateA");
        endGateB = GetNode<Spatial>("Level_Geometry/EndGate/GateB");

        completePoint = GetNode<LevelCompletePoint>("Level_Complete_Point");

        GetNode("Player").GetNode<Camera>("Camera").Far = 90;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if (openedGate && endGateA.Translation.x < -11)
        {
            //Open gates
            endGateA.Translation = endGateA.Translation.LinearInterpolate(gateAPos, delta * 0.34f);
            endGateB.Translation = endGateB.Translation.LinearInterpolate(gateBPos, delta * 0.34f);
        }
        else 
        {
            openedGate = false;
        }
    }


    //Override method
    public override void onItemCollect(Item item)
    {
        if (item.name.Find("Key") != -1)
        {
            keysCollected++;
            UI.changeObjective("Find " + (4 - keysCollected) + " Key(s)");
            //Check if done
            if(keysCollected == 4)
            {
                UI.changeObjective("Unlock the exit");
                completePoint.pointActive = true;
                //Open gate
                openedGate = true;
                //Play sound
                endGateA.GetParent().GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Play();
            }
        }

        switch (item.name)
        {
            case "Barn Key":
                //Turn on the secret baddies
                foreach(EnemyAgent e in secretEnemies)
                {
                    e.Visible = true;
                    e.GetNode<CollisionShape>("CollisionShape").Disabled = false;
                }
                break;
        }
    }
}
