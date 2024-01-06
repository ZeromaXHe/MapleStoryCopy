using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    public enum StatusEnum
    {
        Alert,
        Jump,
        Prone,
        ProneStab,
        Stab01,
        Stab02,
        Stand1,
        Swing01,
        Swing02,
        Swing03,
        Walk1,
    }

    private static readonly HashSet<string> AttackSet = new()
        { "proneStab", "stab01", "stab02", "swing01", "swing02", "swing03" };

    private const float Speed = 180.0f;
    private const float JumpVelocity = -400.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private StatusEnum _status = StatusEnum.Stand1;

    public StatusEnum Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                PlayAnimation(value switch
                {
                    StatusEnum.Alert => "alert",
                    StatusEnum.Jump => "jump",
                    StatusEnum.Prone => "prone",
                    StatusEnum.ProneStab => "proneStab",
                    StatusEnum.Stab01 => "stab01",
                    StatusEnum.Stab02 => "stab02",
                    StatusEnum.Stand1 => "stand1",
                    StatusEnum.Swing01 => "swing01",
                    StatusEnum.Swing02 => "swing02",
                    StatusEnum.Swing03 => "swing03",
                    StatusEnum.Walk1 => "walk1",
                    _ => throw new ArgumentOutOfRangeException()
                });
            }

            _status = value;
        }
    }

    // @onready
    private CollisionShape2D _feetCollisionShape;
    private Area2D _landDetectArea;
    private CollisionShape2D _standCollisionShape;
    private CollisionShape2D _proneCollisionShape;
    private Sprite2D _body;
    private AnimationPlayer _bodyAnim;
    private AnimationPlayer _handWeaponAnim;
    private AnimationPlayer _weaponAnim;
    private AnimationPlayer _pantsAnim;
    private AnimationPlayer _coatAnim;
    private AnimationPlayer _shoesAnim;

    public override void _Ready()
    {
        // 初始化 on-ready 变量
        _feetCollisionShape = GetNode<CollisionShape2D>("FeetCollisionShape2D");
        _landDetectArea = GetNode<Area2D>("LandDetectArea2D");
        _standCollisionShape = GetNode<CollisionShape2D>("StandArea2D/CollisionShape2D");
        _proneCollisionShape = GetNode<CollisionShape2D>("ProneArea2D/CollisionShape2D");
        _body = GetNode<Sprite2D>("Body");
        _bodyAnim = GetNode<AnimationPlayer>("Body/BodyAnim");
        _handWeaponAnim = GetNode<AnimationPlayer>("Body/Navel/Arm/Hand/HandWeapon/HandWeaponAnim");
        _weaponAnim = GetNode<AnimationPlayer>("Body/Navel/Weapon/WeaponAnim");
        _pantsAnim = GetNode<AnimationPlayer>("Body/Navel/Pants/PantsAnim");
        _coatAnim = GetNode<AnimationPlayer>("Body/Navel/Coat/CoatAnim");
        _shoesAnim = GetNode<AnimationPlayer>("Body/Navel/Shoes/ShoesAnim");

        // 默认站立碰撞体
        _feetCollisionShape.Disabled = false;
        // TODO:暂时还没写伤害逻辑，所以都禁用
        _standCollisionShape.Disabled = true;
        _proneCollisionShape.Disabled = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        // TODO: 后续需要用状态模式重构
        Vector2 velocity = Velocity;
        
        if (IsOnFloor())
        {
            // GetSlideCollision().GetColliderShape() as CollisionShape2D
        }
        else
        {
            // 重力影响
            velocity.Y += _gravity * (float)delta;
            Status = StatusEnum.Jump;
        }

        var attacking = _bodyAnim.IsPlaying() && AttackSet.Contains(_bodyAnim.AssignedAnimation);

        // 匍匐
        var proning = Input.IsActionPressed("prone");
        if (proning && IsOnFloor())
        {
            velocity.X = 0;
            if (!attacking)
            {
                if (Input.IsActionJustPressed("attack"))
                {
                    Status = StatusEnum.ProneStab;
                }
                else if (!"proneStab".Equals(_bodyAnim.AssignedAnimation) || !_bodyAnim.IsPlaying())
                {
                    Status = StatusEnum.Prone;
                }

                // 向下跳跃
                if (Input.IsActionJustPressed("jump"))
                {
                    Position += Vector2.Down;
                    // velocity.Y = -JumpVelocity;
                }
            }
        }
        else
        {
            // 跳跃
            if (Input.IsActionJustPressed("jump") && IsOnFloor())
                velocity.Y = JumpVelocity;

            var attackPressed = Input.IsActionPressed("attack");
            // 左右移动
            var direction = Input.GetAxis("move_left", "move_right");
            if (!attacking && !attackPressed && direction != 0)
            {
                velocity.X = direction * Speed;
                if (IsOnFloor())
                {
                    Status = StatusEnum.Walk1;
                }

                if (direction > 0 && Math.Abs(_body.Scale.X - 1) < 0.01)
                {
                    _body.Scale = new Vector2(-1, 1);
                    _standCollisionShape.Position =
                        new Vector2(Math.Abs(_standCollisionShape.Position.X), _standCollisionShape.Position.Y);
                    _proneCollisionShape.Position =
                        new Vector2(Math.Abs(_proneCollisionShape.Position.X), _proneCollisionShape.Position.Y);
                }
                else if (direction < 0 && Math.Abs(_body.Scale.X - -1) < 0.01)
                {
                    _body.Scale = Vector2.One;
                    _standCollisionShape.Position =
                        new Vector2(-Math.Abs(_standCollisionShape.Position.X), _standCollisionShape.Position.Y);
                    _proneCollisionShape.Position =
                        new Vector2(-Math.Abs(_proneCollisionShape.Position.X), _proneCollisionShape.Position.Y);
                }
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
                if (IsOnFloor())
                {
                    if (!attacking && attackPressed)
                    {
                        if (Status != StatusEnum.Stab01 && Status != StatusEnum.Stab02 && Status != StatusEnum.Swing01
                            && Status != StatusEnum.Swing02 && Status != StatusEnum.Swing03)
                        {
                            RandomAttackStatus();
                        }
                        else if (!_bodyAnim.IsPlaying())
                        {
                            RandomAttackStatus();
                        }
                    }
                    else if (!attacking)
                    {
                        Status = StatusEnum.Stand1;
                    }
                }
            }
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    // Ctrl + Alt + M 重构方法的快捷键可能被网易云音乐和 GeForce Experience（傻逼玩意，改快捷键还必须登录） 覆盖
    private void RandomAttackStatus()
    {
        var rand = new Random().Next(0, 5);
        Status = rand switch
        {
            0 => StatusEnum.Stab01,
            1 => StatusEnum.Stab02,
            2 => StatusEnum.Swing01,
            3 => StatusEnum.Swing02,
            4 => StatusEnum.Swing03,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void PlayAnimation(string animName)
    {
        _bodyAnim.Play(animName);
        _handWeaponAnim.Play(animName);
        _weaponAnim.Play(animName);
        _pantsAnim.Play(animName);
        _coatAnim.Play(animName);
        _shoesAnim.Play(animName);
    }
}