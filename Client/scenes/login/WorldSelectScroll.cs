using Godot;
using System;

public partial class WorldSelectScroll : AnimatedSprite2D
{
    // Signal 需要以 EventHandler 结尾
    [Signal]
    public delegate void GoToBtnPressedEventHandler();

    private enum State
    {
        Open,
        Close,
    }
    
    public static WorldSelectScroll Singleton { get; private set; }

    private State _state = State.Close;

    // @onready
    private Sprite2D _background;
    private Sprite2D _worldTitle;
    private GridContainer _channelGrid;
    private AudioTextureButton _goToBtn;

    public override void _Ready()
    {
        Singleton = this;

        // 初始化 on-ready 变量
        _background = GetNode<Sprite2D>("Background");
        _worldTitle = GetNode<Sprite2D>("WorldTitle");
        _channelGrid = GetNode<GridContainer>("ChannelGrid");
        _goToBtn = GetNode<AudioTextureButton>("GoToBtn");

        // 默认卷轴处于卷起的状态，隐藏卷轴内容
        Animation = "close";
        Frame = 3;
        HideContents();

        // 绑定信号
        AnimationFinished += OnAnimationFinished;
    }

    public void Open()
    {
        _state = State.Open;
        Play("open");
    }

    public void Close()
    {
        _state = State.Close;
        HideContents();
        Play("close");
    }

    private void HideContents()
    {
        _background.Hide();
        _worldTitle.Hide();
        _channelGrid.Hide();
        _goToBtn.Hide();
    }

    private void OnAnimationFinished()
    {
        if (_state == State.Open)
        {
            ShowContents();
        }
    }

    private void ShowContents()
    {
        _background.Show();
        _worldTitle.Show();
        _channelGrid.Show();
        _goToBtn.Show();
    }

    private void OnGoToBtnPressed()
    {
        EmitSignal(SignalName.GoToBtnPressed);
    }
}