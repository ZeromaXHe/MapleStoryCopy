using Godot;
using System;

public partial class Cursor : Node2D
{
    private enum CursorStatus
    {
        Normal,
        Clicked,
        Clickable,
    }

    public static Cursor Singleton { get; private set; }

    private CursorStatus _status = CursorStatus.Normal;
    private CursorStatus _clickPreStatus;

    // @onready
    private Sprite2D _normal;
    private Sprite2D _clicked;
    private AnimatedSprite2D _clickable;
    private Timer _timer;

    public override void _Ready()
    {
        // 注册单例
        Singleton = this;
        // 初始化 on-ready 变量
        _normal = GetNode<Sprite2D>("Normal");
        _clicked = GetNode<Sprite2D>("Clicked");
        _clickable = GetNode<AnimatedSprite2D>("Clickable");
        _timer = GetNode<Timer>("Timer");
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition();
    }

    public void Normal()
    {
        _status = CursorStatus.Normal;
        HideAll();
        _normal.Show();
    }

    public void Clicked()
    {
        _clickPreStatus = _status;
        _status = CursorStatus.Clicked;
        HideAll();
        _clicked.Show();
        _timer.Start();
    }

    public void Clickable()
    {
        _status = CursorStatus.Clickable;
        HideAll();
        _clickable.Show();
    }

    private void HideAll()
    {
        _normal.Hide();
        _clicked.Hide();
        _clickable.Hide();
        _timer.Stop();
    }

    private void OnTimerTimeout()
    {
        // GD.Print("Cursor | timer timeout");
        // 点击计时器到期，则还原光标
        switch (_clickPreStatus)
        {
            case CursorStatus.Normal:
                Normal();
                break;
            case CursorStatus.Clickable:
                Clickable();
                break;
            case CursorStatus.Clicked:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}