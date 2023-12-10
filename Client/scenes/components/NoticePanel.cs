using Godot;
using System;

public partial class NoticePanel : CenterContainer
{
    public enum ErrorCode
    {
        WRONG_PASSWORD,
        EMPTY_LOGIN_ID,
        NO_EXIST_LOGIN_ID,
        SERVER_CONNECT_FAIL,
    }

    public static NoticePanel Singleton;

    private static Texture2D _textureWrongPassword = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.3.png");
    private static Texture2D _textureEmptyLoginId = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.4.png");
    private static Texture2D _textureNoExistLoginId = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.20.png");
    private static Texture2D _textureServerConnectFail = (Texture2D)GD.Load("res://assets/UI/Login/Notice/text.23.png");

    // @onready
    private TextureRect text;
    private AudioStreamPlayer noticeAudio;

    public override void _Ready()
    {
        // 注册单例
        Singleton = this;
        // 初始化 on-ready 变量
        text = GetNode<TextureRect>("NoticePanel/Text");
        noticeAudio = GetNode<AudioStreamPlayer>("NoticeAudio");
    }

    public void ShowPanel(ErrorCode code)
    {
        switch (code)
        {
            case ErrorCode.WRONG_PASSWORD:
                text.Texture = _textureWrongPassword;
                break;
            case ErrorCode.EMPTY_LOGIN_ID:
                text.Texture = _textureEmptyLoginId;
                break;
            case ErrorCode.NO_EXIST_LOGIN_ID:
                text.Texture = _textureNoExistLoginId;
                break;
            case ErrorCode.SERVER_CONNECT_FAIL:
                text.Texture = _textureServerConnectFail;
                break;
        }

        noticeAudio.Play();
        Show();
    }

    private void OnYesBtnPressed()
    {
        Hide();
    }
}