using Godot;
using System;

public class UI : Node
{
    //Components
    private Label healthLabel;
    private Label ammoLabel;

    public override void _Ready()
    {
        //Get Components
        healthLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Health_Label");
        ammoLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Ammo_Label");
    }

    public void updateAmmoUI(int clip, int total)
    {
        ammoLabel.Text = "Ammo: " + clip + "/" + total;
    }

    public void updateAmmoUI() //Knife overload
    {
        ammoLabel.Text = "Ammo: --/--";
    }
}
