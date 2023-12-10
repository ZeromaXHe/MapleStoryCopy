using Godot;
using System;

public partial class LoginMenu : Control
{
    // @onready
    private LineEdit _loginIdEdit;
    private LineEdit _passwordEdit;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        // 初始化 on-ready 变量
        _loginIdEdit = GetNode<LineEdit>("Signboard/LoginIdEdit");
        _passwordEdit = GetNode<LineEdit>("Signboard/PasswordEdit");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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

    private void quit()
    {
        GetTree().Quit();
    }

    private void OnLoginBtnPressed()
    {
        NoticePanel.ErrorCode code = NoticePanel.ErrorCode.SERVER_CONNECT_FAIL;
        // GD.Print("LoginMenu | login btn pressed");
        if (string.IsNullOrEmpty(_loginIdEdit.Text))
        {
            code = NoticePanel.ErrorCode.EMPTY_LOGIN_ID;
        } else if ("admin" != _loginIdEdit.Text)
        {
            code = NoticePanel.ErrorCode.NO_EXIST_LOGIN_ID;
        } else if ("admin" != _passwordEdit.Text)
        {
            code = NoticePanel.ErrorCode.WRONG_PASSWORD;
        }

        NoticePanel.Singleton.ShowPanel(code);
    }

    private void OnQuitBtnPressed()
    {
        _animationPlayer.Play("quit");
    }
}