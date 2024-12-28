using Godot;
using System;
using System.Collections.Generic;

public partial class Textbox : Node2D, IScriptExecutable
{
	[Export]
	Sprite2D textBoxRef = null;
	[Export]
	Label textLabelRef = null;

	public string ObjectName => Name;

	public override void _Ready()
	{
		textLabelRef.Visible = true;
		textBoxRef.Visible = true;
		textLabelRef.Modulate = new Color(1, 1, 1, 0);
		textBoxRef.Modulate = new Color(1, 1, 1, 0);
	}

	public override void _Process(double delta)
	{
	}


	#region Actions

	Tween TweenShowTextbox(float duration = 1.0f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		preTween.TweenProperty(textBoxRef, "modulate:a", 1, duration);
		return preTween;
	}

	Tween TweenFadeTextbox(float duration = 0.5f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		preTween.TweenProperty(textBoxRef, "modulate:a", 0, duration);
		return preTween;
	}

	Tween TweenShowText(float duration = 1.0f, Tween preTween = null)
	{
		textLabelRef.Modulate = new Color(1, 1, 1, 1);
		textLabelRef.VisibleRatio = 0;

		if (preTween == null)
		{
			preTween = CreateTween();
		}
		preTween.TweenProperty(textLabelRef, "visible_ratio", 1, duration);
		return preTween;
	}

	Tween TweenFadeText(float duration = 0.5f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		preTween.TweenProperty(textLabelRef, "modulate:a", 0, duration);
		return preTween;
	}
	#endregion
	public void Fade()
	{
		textLabelRef.Modulate = new Color(Modulate, 0.0f);
		textBoxRef.Modulate = new Color(Modulate, 0.0f);
	}
	public SceneTreeTimer SetTextAndAnimation(string textContent, Color textColor, float showDuration = 1.0f, float holdDuration = 1.0f)
	{
		textLabelRef.Text = textContent;
		textLabelRef.AddThemeColorOverride("font_color", textColor);
		TweenShowText(showDuration);
		return GetTree().CreateTimer(showDuration + holdDuration);
	}

	public void ExecuteScript(ScriptRow scriptRow)
	{
		string actionName = scriptRow.GetCellValue(ScriptColumnNames.ActionName);
		int rowIndex = scriptRow.GetRowIndex();


		if (actionName == "Show_Box")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenShowTextbox(actionDurationTime).Finished += () => StageScript.StageScriptRef.EndActionCallBack(rowIndex);
		}
		else if (actionName == "Fade_Box")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenFadeTextbox(actionDurationTime).Finished += () => StageScript.StageScriptRef.EndActionCallBack(rowIndex);
		}
		else if (actionName == "Fade")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenFadeTextbox(actionDurationTime);
			TweenFadeText(actionDurationTime).Finished += () => StageScript.StageScriptRef.EndActionCallBack(rowIndex);
		}


		else
		{
			GD.Print(StageScript.StageScriptRef.GetScriptRowInfo() + $"Action {actionName} not found");
			StageScript.StageScriptRef.EndActionCallBack(rowIndex);
		}
	}

    public void FinishCurrentAnimation()
    {
        
    }
}
