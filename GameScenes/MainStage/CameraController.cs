using Godot;
using System;

public partial class CameraController : Camera2D, IScriptExecutable
{
    public string ObjectName => Name;

    public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void ExecuteScript(ScriptRow scriptRow)
	{
		throw new NotImplementedException();
	}

    public void FinishCurrentAnimation()
    {
    }
}
