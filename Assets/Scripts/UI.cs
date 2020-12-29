using Godot;

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
    private LevelManager level;
    //Components for menus
    private Control gameOverMenu;
    private Control levelCompleteMenu;
    private Control pauseMenu;

    public override void _Ready()
    {
        //Get Components
        level = (LevelManager)GetTree().Root.GetChild(0);
        topBar = GetNode<Control>("TopBar_Container");
        healthLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Health_Label");
        ammoLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Ammo_Label");
        objectiveLabel = GetNode<Label>("TopBar_Container/Bar/MarginContainer//HBoxContainer/Objective_Label");

        gameOverMenu = GetNode<Control>("GameOver_Container");
        gameOverMenu.Hide();

        levelCompleteMenu = GetNode<Control>("LevelBeat_Container");
        levelCompleteMenu.Hide();

        pauseMenu = GetNode<Control>("PauseMenu_Container");
        pauseMenu.Hide();

        changeObjective(openingObjective);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("ui_cancel"))
        {
            updatePauseMenu();
        }
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
        Input.SetMouseMode(Input.MouseMode.Visible);
        GetTree().Paused = true;
        //Open level complete menu
        levelCompleteMenu.Show();
        //Check if next level to play
        if (level.nextLevel == null)
        {
            //Hide next level button
            GetNode<Control>("LevelBeat_Container/TextureRect/MarginContainer/VBoxContainer/NextLevelButton").Hide();
        }
        //Play some fanfare
        level.changeMusic(2, -5);
    }

    public void updatePauseMenu()
    {
        bool visSetting = false;
        //See if we need to turn the thing off or on
        if (!pauseMenu.Visible)
        {
            visSetting = true; //We need to turn everything on
        }
        pauseMenu.Visible = visSetting;
        GetTree().Paused = visSetting;

        if (visSetting)
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
        }
        else
        {
            Input.SetMouseMode(Input.MouseMode.Captured);
        }
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
        GetTree().ChangeScene("res://Assets/Game_Scenes/Main_Menu.tscn");
        GetTree().Paused = false;
    }

    public void _on_NextLevelButton_button_down()
    {
        //Even though the button is hidden when there is no next level, this is a failsafe
        if (level.nextLevel != null)
        {
            GetTree().ChangeSceneTo(level.nextLevel);
        }
        else
        {
            GetTree().ChangeScene("res://Assets/Game_Scenes/Main_Menu.tscn");
        }
        GetTree().Paused = false;
    }

}