using Godot;
using Godot.Collections;
using System;
using System.Runtime;

public partial class CharacterController : Node2D, IScriptExecutable
{
	[Export]
	AnimationPlayer animationPlayerRef = null;
	[Export]
	Array<Node2D> sprites = new Array<Node2D>();

	[Export]
	Color textColor = new Color(0.1f, 0.1f, 0.1f);

	Node2D activeSprite = null;

	public string ObjectName => Name;





	void SetActiveSprite(Node2D sprite)
	{
		activeSprite = sprite;
		foreach (Node2D sp in sprites)
		{
			sp.Visible = sp == activeSprite;
		}
	}

	public override void _Ready()
	{
		foreach (Node2D sprite in sprites)
		{
			sprite.Visible = true;
		}

		Modulate = new Color(1, 1, 1, 0);

		SetActiveSprite(sprites[0]);
	}


	public override void _Process(double delta)
	{
	}


	#region Actions
	int lastActionIndex = -1;
	void EndActionCallBack()
	{
		StageScript.StageScriptRef.EndActionCallBack(lastActionIndex);
		lastActionIndex = -1;
	}
	void FadeAllCharactersAndTextbox()
	{
		StageScript.StageScriptRef.FadeAllCharactersAndTextbox();
	}
	void SetSpriteByName(string spriteName)
	{
		bool isFound = false;
		foreach (Sprite2D sprite in sprites)
		{
			if (sprite.Name == spriteName)
			{
				SetActiveSprite(sprite);
				isFound = true;
			}
		}
		if (!isFound)
		{
			GD.Print(StageScript.StageScriptRef.GetScriptRowInfo() + $"Sprite with name {spriteName} not found");
		}
	}

	Tween TweenShow(float duration = 1.0f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		Modulate = new Color(Modulate, 0);
		preTween.TweenProperty(this, "modulate:a", 1, duration);
		return preTween;
	}
	Tween TweenFade(float duration = 0.5f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		preTween.TweenProperty(this, "modulate:a", 0, duration);
		return preTween;
	}
	Tween TweenFocusOff(float duration = 0.5f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		Color targetColor = new Color(0.8f, 0.8f, 0.8f);
		targetColor.A = Modulate.A;

		preTween.TweenProperty(this, "modulate", targetColor, duration);
		return preTween;
	}
	Tween TweenFocusOn(float duration = 0.5f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		Color targetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);


		preTween.TweenProperty(this, "modulate", targetColor, duration);
		return preTween;
	}
	Tween TweenFlip(float duration = 0.5f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}

		var targetScale = GlobalScale;
		targetScale.X *= -1;

		preTween.TweenProperty(this, "scale", targetScale, duration);
		return preTween;
	}
	Tween TweenMove(Marker2D target, float duration = 1.0f, Tween preTween = null)
	{
		if (preTween == null)
		{
			preTween = CreateTween();
		}
		preTween.TweenProperty(this, "position", target.Position, duration);
		return preTween;
	}

	void PlayAnimation(string animationName)
	{
		animationPlayerRef.Play(animationName);
	}



	#endregion
	public void Fade()
	{
		Modulate = new Color(Modulate, 0.0f);
	}

	public void ExecuteScript(ScriptRow scriptRow)
	{


		string actionName = scriptRow.GetCellValue(ScriptColumnNames.ActionName);
		lastActionIndex = scriptRow.GetRowIndex();

		if (actionName == "Show")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			Tween tween = TweenFocusOn(0);
			TweenShow(actionDurationTime, tween).Finished += EndActionCallBack;
		}
		else if (actionName == "Fade")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenFade(actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "FocusOn")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenFocusOn(actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "FocusOff")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenFocusOff(actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "Flip")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenFlip(actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "Move_Left")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenMove(Markers.MarkersInstance.MarkerLeftRef, actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "Move_Center")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenMove(Markers.MarkersInstance.MarkerCenterRef, actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "Move_Right")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenMove(Markers.MarkersInstance.MarkerRightRef, actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "Move_Right_Out")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenMove(Markers.MarkersInstance.MarkerRightOutRef, actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "Move_Left_Out")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			TweenMove(Markers.MarkersInstance.MarkerLeftOutRef, actionDurationTime).Finished += EndActionCallBack;
		}
		else if (actionName == "Text")
		{
			float actionDurationTime = float.Parse(scriptRow.GetCellValue(ScriptColumnNames.ActionDurationTime));
			string textContent = scriptRow.GetCellValue(ScriptColumnNames.TextContent);
			float textContentDurationTime = scriptRow.GetTextContentDurationTime();

			StageScript.StageScriptRef.TextboxRef.SetTextAndAnimation(textContent, textColor, actionDurationTime, textContentDurationTime).Timeout += EndActionCallBack;
		}
		else if (actionName == "SetSprite")
		{
			string spriteName = scriptRow.GetCellValue(ScriptColumnNames.CharacterDifferenceName);
			SetSpriteByName(spriteName);
			EndActionCallBack();
		}
		else if (actionName == "Animation")
		{
			string animationName = scriptRow.GetCellValue(ScriptColumnNames.AnimationName);
			PlayAnimation(animationName);
		}

		else
		{
			GD.Print(StageScript.StageScriptRef.GetScriptRowInfo() + $"Action {actionName} not found");
			EndActionCallBack();
		}
	}

	public void FinishCurrentAnimation()
	{
		if (animationPlayerRef != null && animationPlayerRef.IsPlaying())
		{
			animationPlayerRef.Advance(animationPlayerRef.CurrentAnimationLength - animationPlayerRef.CurrentAnimationPosition);
		}
	}
}
