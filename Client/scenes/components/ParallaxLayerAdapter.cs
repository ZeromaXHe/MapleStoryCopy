using Godot;
using System;

[Tool]
public partial class ParallaxLayerAdapter : ParallaxLayer
{
    // 对应 wz 文件里的属性
    // bS, ani（0 - back, 1 - ani, 2 - spine）, no
    // 声明资源引用路径
    /**
     * 0 表示背景，1 表示前景
     */
    private int _front;

    [Export(PropertyHint.Enum, "0,1")]
    public int Front
    {
        get => _front;
        set
        {
            _front = value;
            ZIndex = _front == 1 ? 200 : 0;
        }
    }

    /**
     * 背景运动方式
     * 0 - 无平铺
     * 1 - 水平平铺
     * 2 - 垂直平铺
     * 3 - 水平/垂直平铺
     * 4 - 水平平铺 + 水平滚动
     * 5 - 垂直平铺 + 垂直滚动
     * 6 - 双向平铺 + 水平滚动
     * 7 - 双向平铺 + 垂直滚动
     */
    private int _type;

    [Export(PropertyHint.Enum, "0,1,2,3,4,5,6,7")]
    public int Type
    {
        get => _type;
        set
        {
            _type = value;
            switch (_type)
            {
                case 0:
                    MotionMirroring = Vector2.Zero;
                    _autoMove = Vector2.Zero;
                    break;
                // 平铺的 Mirroring 还是自己填吧，自动化得拿子节点 Sprite2D 物料的大小，不好获取
                case 1:
                case 2:
                case 3:
                    _autoMove = Vector2.Zero;
                    break;
                case 4:
                case 6:
                    _autoMove = new Vector2(_rx, 0);
                    break;
                case 5:
                case 7:
                    _autoMove = new Vector2(0, _ry);
                    break;
            }
        }
    }

    /**
     * 平铺间隔，0 表示密集平铺
     */
    [Export] private int _cx;

    [Export] private int _cy;

    /**
     * 有自身运动：表示自动滚动速率
     * 无自身运动：镜头差分运动比率
     */
    private int _rx;

    private int _ry;

    [Export]
    public int Rx
    {
        get => _rx;
        set
        {
            _rx = value;
            UpdateAutoMoveX();
        }
    }

    [Export]
    public int Ry
    {
        get => _ry;
        set
        {
            _ry = value;
            UpdateAutoMoveY();
        }
    }

    /**
     * 不透明度（alpha）
     */
    private int _a = 255;

    [Export(PropertyHint.Range, "0,255,1")]
    public int A
    {
        get => _a;
        set
        {
            _a = value;
            Modulate = new Color(1, 1, 1, _a / 255.0f);
        }
    }

    /**
     * 是否水平翻转（flip）
     */
    private int _f;

    [Export(PropertyHint.Enum, "0,1")]
    public int F
    {
        get => _f;
        set
        {
            _f = value;
            Scale = _f == 0? Vector2.One: new Vector2(-1, 1);
        }
    }

    private Vector2 _autoMove = Vector2.Zero;

    public override void _Process(double delta)
    {
        MotionOffset += _autoMove * (float) delta;
        // 不确定一直移动的话会不会出问题，所以保证一下 Offset 在 Mirroring 范围内
        if (_autoMove.X != 0)
        {
            var maxX = MotionMirroring.X;
            if (MotionOffset.X > maxX)
                MotionOffset -= new Vector2(maxX, 0);
            else if (MotionOffset.X < -maxX)
                MotionOffset += new Vector2(maxX, 0);
        }
        
        if (_autoMove.Y != 0)
        {
            var maxY = MotionMirroring.Y;
            if (MotionOffset.Y > maxY)
                MotionOffset -= new Vector2(0, maxY);
            else if (MotionOffset.Y < -maxY)
                MotionOffset += new Vector2(0, maxY);
        }
    }

    private void UpdateAutoMoveX()
    {
        if (_type is 4 or 6)
        {
            _autoMove = new Vector2(_rx, 0);
        }
    }

    private void UpdateAutoMoveY()
    {
        if (_type is 5 or 7)
        {
            _autoMove = new Vector2(0, _ry);
        }
    }
}