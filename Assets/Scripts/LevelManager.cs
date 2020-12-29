using Godot;
using System;

public class LevelManager : Node
{
    [Export]
    public PackedScene nextLevel;
    [Export]
    private AudioStream[] clips;

    private static RigidBody player;
    public Navigation nav;
    private AudioStreamPlayer musicBox;

    public override void _Ready()
    {
        base._Ready();
        player = GetNode<RigidBody>("Player");
        nav = GetNode<Navigation>("Level_Geometry");
        musicBox = GetNode<AudioStreamPlayer>("Music Box");
    }

    //--------Member Functions
    public static Vector3 getPlayerPos()
    {
        return player.Translation;
    }

    public virtual void onItemCollect(Item item)
    {
        
    }

    public void changeMusic(int clip)
    {
        musicBox.Stream = clips[clip];
        musicBox.Play();
    }

    public void changeMusic(int clip, int db)
    {
        musicBox.Stream = clips[clip];
        musicBox.VolumeDb = db;
        musicBox.Play();
    }

    public Navigation getNavigation()
    {
        return nav;
    }
}
