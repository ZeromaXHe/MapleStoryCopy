using Godot;
using System;

// [GlobalClass] 等效于 GDScript 里面 class_name 可以让自定义脚本出现在新节点的编辑器页面上的效果，
// 但是这样的话只是作为绑定该脚本的父类场景使用，并不会有自己自定义场景里的子节点，GetNode 就会拿不到
public partial class AudioTextureButton : TextureButton
{
    // [Export(PropertyHint.ResourceType, "AudioStream")]
    private static AudioStream _clickAudio = (AudioStream)GD.Load("res://assets/Sound/UI/BtMouseClick.mp3");
    
    // [Export(PropertyHint.ResourceType, "AudioStream")]
    private static AudioStream _hoverAudio = (AudioStream)GD.Load("res://assets/Sound/UI/BtMouseOver.mp3");

    // @onready
    private AudioStreamPlayer _audioStreamPlayer;

    public override void _Ready()
    {
        // 初始化 @onready 字段
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    private void OnPressed()
    {
        if (_clickAudio == null)
        {
            GD.PrintErr("_clickAudio undefined");
            return;
        }

        // GD.Print("_clickAudio play");
        _audioStreamPlayer.Stream = _clickAudio;
        _audioStreamPlayer.Play();
    }

    private void OnMouseEntered()
    {
        if (_hoverAudio == null)
        {
            GD.PrintErr("_hoverAudio undefined");
            return;
        }
        // 修改光标
        Cursor.Singleton.Clickable();
        // GD.Print("_hoverAudio play");
        _audioStreamPlayer.Stream = _hoverAudio;
        _audioStreamPlayer.Play();
    }

    private void OnMouseExited()
    {
        // 还原光标
        Cursor.Singleton.Normal();
    }
}