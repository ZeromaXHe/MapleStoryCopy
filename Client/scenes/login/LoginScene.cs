using Godot;
using System;

public partial class LoginScene : Node2D
{
    // Signal 需要以 EventHandler 结尾
    [Signal]
    public delegate void StateChangedEventHandler(StateEnum state);

    public enum StateEnum
    {
        Login,
        WorldSelect,
        CharacterSelect,
        NewCharacter,
    }

    private StateEnum _state = StateEnum.Login;

    public StateEnum State
    {
        get => _state;
        private set
        {
            _state = value;
            EmitSignal(SignalName.StateChanged, Variant.From(_state));
        }
    }

    // @onready
    private LineEdit _loginIdEdit;
    private LineEdit _passwordEdit;
    private AnimationPlayer _animationPlayer;
    private Camera2D _camera;
    private WorldSelectScroll _worldSelectScroll;

    public override void _Ready()
    {
        // 初始化 on-ready 变量
        _loginIdEdit = GetNode<LineEdit>("Signboard/LoginIdEdit");
        _passwordEdit = GetNode<LineEdit>("Signboard/PasswordEdit");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _camera = GetNode<Camera2D>("Camera2D");
        _worldSelectScroll = GetNode<WorldSelectScroll>("Tree/WorldSelect/Scroll");

        // 防止动画有的时候把位置弄错了
        _camera.Position = new Vector2(640, 360);
        
        // 绑定信号
        WorldSelectScroll.Singleton.GoToBtnPressed += OnWorldGoToBtnPressed;
    }

    private void Quit()
    {
        GetTree().Quit();
    }

    public void PlayLoginEffect()
    {
        if (_state == StateEnum.Login)
            GetTree().CreateTimer(_animationPlayer.GetAnimation("loginToWorldSelect").Length).Timeout +=
                () => _animationPlayer.Play("effect");
    }

    public void WorldSelectToLogin()
    {
        State = StateEnum.Login;
        _animationPlayer.PlayBackwards("loginToWorldSelect");
    }

    public void CharacterToWorldSelect()
    {
        State = StateEnum.WorldSelect;
        _animationPlayer.PlayBackwards("worldToCharSelect");
    }

    public void NewCharacterToSelect()
    {
        State = StateEnum.CharacterSelect;
        _animationPlayer.PlayBackwards("charSelectToNewChar");
    }

    private void OnLoginBtnPressed()
    {
        // GD.Print("LoginMenu | login btn pressed");
        if (string.IsNullOrEmpty(_loginIdEdit.Text))
        {
            NoticePanel.Singleton.ShowPanel(NoticePanel.ErrorCode.EmptyLoginId);
            return;
        }

        if ("admin" != _loginIdEdit.Text)
        {
            NoticePanel.Singleton.ShowPanel(NoticePanel.ErrorCode.NoExistLoginId);
            return;
        }

        if ("admin" != _passwordEdit.Text)
        {
            NoticePanel.Singleton.ShowPanel(NoticePanel.ErrorCode.WrongPassword);
            return;
        }

        State = StateEnum.WorldSelect;
        _animationPlayer.Play("loginToWorldSelect");

        // 信号的发送格式：EmitSignal(SignalName.{去掉 EventHandler 的 [Signal] 注解的委托名})
        EmitSignal(SignalName.StateChanged);
    }

    #region 角色选择界面信号处理

    private void OnCharacterNewBtnPressed()
    {
        State = StateEnum.NewCharacter;
        _animationPlayer.Play("charSelectToNewChar");
    }

    #endregion

    #region 世界选择界面信号处理

    private void OnWorldGoToBtnPressed()
    {
        State = StateEnum.CharacterSelect;
        _animationPlayer.Play("worldToCharSelect");
    }

    private void OnWorld0BtnPressed()
    {
        _worldSelectScroll.Open();
    }

    private void OnWorld0BtnFocusExited()
    {
        // 必须延时，不然点击“到选择的世界去”按钮没反应
        GetTree().CreateTimer(0.2).Timeout += () => _worldSelectScroll.Close();
    }

    #endregion

    #region 登陆界面的信号处理

    private void OnQuitBtnPressed()
    {
        _animationPlayer.Play("quit");
    }

    #endregion
}