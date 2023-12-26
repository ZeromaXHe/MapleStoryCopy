using Godot;
using System;

public partial class LoginGUI : Control
{
    private static readonly Texture2D TextureWorldSelect =
        (Texture2D)GD.Load("res://assets/UI/Login/Common/step.1.png");

    private static readonly Texture2D TextureCharSelect =
        (Texture2D)GD.Load("res://assets/UI/Login/Common/step.2.png");

    private static readonly Texture2D TextureNewChar = (Texture2D)GD.Load("res://assets/UI/Login/Common/step.3.png");

    // @onready
    private LoginScene _scene;
    private AudioTextureButton _backBtn;
    private TextureRect _step;
    private TextureRect _selectedWorld;

    public override void _Ready()
    {
        // 初始化 on-ready 变量
        _scene = GetNode<LoginScene>("SubViewportContainer/SubViewport/LoginScene");
        _backBtn = GetNode<AudioTextureButton>("Frame/BackBtn");
        _step = GetNode<TextureRect>("Frame/Step");
        _selectedWorld = GetNode<TextureRect>("Frame/SelectedWorld");

        // 绑定信号
        _scene.StateChanged += ShowFrameButtons;

        // 默认登陆界面隐藏所有按钮
        ShowFrameButtons(LoginScene.StateEnum.Login);
    }

    private void ShowFrameButtons(LoginScene.StateEnum state)
    {
        _backBtn.Visible = state is not LoginScene.StateEnum.Login;
        _step.Visible = state is not LoginScene.StateEnum.Login;
        _step.Texture = state switch
        {
            LoginScene.StateEnum.WorldSelect => TextureWorldSelect,
            LoginScene.StateEnum.CharacterSelect => TextureCharSelect,
            LoginScene.StateEnum.NewCharacter => TextureNewChar,
            _ => _step.Texture
        };
        _selectedWorld.Visible = state is LoginScene.StateEnum.CharacterSelect or LoginScene.StateEnum.NewCharacter;
    }

    private void OnBackBtnPressed()
    {
        switch (_scene.State)
        {
            case LoginScene.StateEnum.WorldSelect:
                _scene.WorldSelectToLogin();
                break;
            case LoginScene.StateEnum.CharacterSelect:
                _scene.CharacterToWorldSelect();
                break;
            case LoginScene.StateEnum.NewCharacter:
                _scene.NewCharacterToSelect();
                break;
            case LoginScene.StateEnum.Login:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}