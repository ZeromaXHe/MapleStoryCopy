using Godot;
using System;

public partial class Cursor : Node2D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
	}
}
