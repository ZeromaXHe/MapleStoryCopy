using Godot;
using System;

public partial class NoticePanel : CenterContainer
{
    public enum ErrorCode
    {
        WrongPassword,
        EmptyLoginId,
        NoExistLoginId,
        ServerConnectFail,
    }

    public static NoticePanel Singleton { get; private set; }

    private static readonly Texture2D TextureWrongPassword = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.3.png");
    private static readonly Texture2D TextureEmptyLoginId = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.4.png");
    private static readonly Texture2D TextureNoExistLoginId = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.20.png");
    private static readonly Texture2D TextureServerConnectFail = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.23.png");

    // @onready
    private TextureRect _text;
    private AudioStreamPlayer _noticeAudio;

    public override void _Ready()
    {
        // 注册单例
        Singleton = this;
        // 初始化 on-ready 变量
        _text = GetNode<TextureRect>("NoticePanel/Text");
        _noticeAudio = GetNode<AudioStreamPlayer>("NoticeAudio");
    }

    public void ShowPanel(ErrorCode code)
    {
        _text.Texture = code switch
        {
            ErrorCode.WrongPassword => TextureWrongPassword,
            ErrorCode.EmptyLoginId => TextureEmptyLoginId,
            ErrorCode.NoExistLoginId => TextureNoExistLoginId,
            ErrorCode.ServerConnectFail => TextureServerConnectFail,
            _ => _text.Texture
        };

        _noticeAudio.Play();
        Show();
    }

    private void OnYesBtnPressed()
    {
        Hide();
    }
}