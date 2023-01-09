using Godot;
using System;

public partial class ChappleBin : StaticBody3D
{
    private bool mouseEntered = false;

    public void _on_mouse_entered() {
        mouseEntered = true;
    }

    public void _on_mouse_exited() {
        mouseEntered = false;
    }

    public override void _Process(double delta) {
        if (mouseEntered) {
            // Gradually increase the scale of the bin to 1.5
            Vector3 scale = Scale;
            scale.x = Mathf.Lerp(scale.x, 1.5f, 2f * (float)delta);
            scale.y = Mathf.Lerp(scale.y, 1.5f, 2f * (float)delta);
            scale.z = Mathf.Lerp(scale.z, 1.5f, 2f * (float)delta);

            Scale = scale;

            // Check if mouse is pressed then change scene to cabbit catcher
            if (Input.IsActionJustPressed("left_click")) {
                GetTree().ChangeSceneToFile("res://Levels/chapple_level.tscn");
            }
        } else {
            // Gradually decrease the scale of the bin to 1
            Vector3 scale = Scale;
            scale.x = Mathf.Lerp(scale.x, 1f, 2f * (float)delta);
            scale.y = Mathf.Lerp(scale.y, 1f, 2f * (float)delta);
            scale.z = Mathf.Lerp(scale.z, 1f, 2f * (float)delta);

            Scale = scale;
        }
    }
}
