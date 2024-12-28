using Godot;
using System;
using System.Collections.Generic;

public interface IScriptExecutable
{
    public void ExecuteScript(ScriptRow scriptRow);
    public string ObjectName { get; }

    public void FinishCurrentAnimation();
}