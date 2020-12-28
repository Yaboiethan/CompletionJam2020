using Godot;
using System;

public class LevelManager : Node
{
    private static RigidBody player;
    public Navigation nav;

    public override void _Ready()
    {
        base._Ready();
        player = GetNode<RigidBody>("Player");
        nav = GetNode<Navigation>("Level_Geometry");
    }

    //--------Statics
    public static Vector3 getPlayerPos()
    {
        return player.Translation;
    }

    public virtual void onItemCollect(Item item)
    {
        
    }
}
