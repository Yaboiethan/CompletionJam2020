using Godot;
using System;

public class UI : Node
{
    //Exports
    [Export]
    private string openingObjective = "NaN";
    //Components
    private Control topBar;
    private Label healthLabel;
    private Label ammoLabel;
    private static Label objectiveLabel;
    //Components for menus
    private Control gameOverMenu;

    public override void _Ready()
    {
        //Get Components
        topBar = GetNode<Control>("TopBar_Container");
        healthLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Health_Label");
        ammoLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Ammo_Label");
        objectiveLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Objective_Label");

        gameOverMenu = GetNode<Control>("GameOver_Container");
        gameOverMenu.Hide();

        changeObjective(openingObjective);
    }

    public void updateAmmoUI(int clip, int total)
    {
        ammoLabel.Text = "Ammo: " + clip + "/" + total;
    }

    public void updateAmmoUI() //Knife overload
    {
        ammoLabel.Text = "Ammo: --/--";
    }

    public void updatePlayerHealthUI(int h)
    {
        healthLabel.Text = "Health: " + h;
        if (h <= 0) //Game over menu
        {
            gameOverMenu.Show();
            //Unlock mouse
            Input.SetMouseMode(Input.MouseMode.Visible);
            GetTree().Paused = true;
        }
    }

    public void levelComplete()
    {
        //Open level complete menu
    }

    public static void changeObjective(string objt)
    {
        objectiveLabel.Text = "Objective: \n" + objt;
    }

    //-------------Signals
    public void _on_RetryButton_button_down()
    {
        GetTree().ReloadCurrentScene();
        GetTree().Paused = false;

    }

    public void _on_MenuButton_button_down()
    {
        
    }
}
