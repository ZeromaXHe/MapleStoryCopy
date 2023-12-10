using Godot;
using System;

public partial class LoginMenu : Control
{
    public override void _Ready()
    {
        // 隐藏鼠标，这样才能使用我们自定义的带动画的鼠标图案
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventMouseButton)) return;
        var mouseEvent = @event as InputEventMouseButton;
        if (mouseEvent.ButtonIndex != MouseButton.Left || !mouseEvent.IsPressed()) return;
        Cursor.Singleton.Clicked();
    }

    private void OnLoginBtnPressed()
    {
        GD.Print("login btn pressed");
    }
}