using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public partial class StageScript : Node
{
	static StageScript stageScriptRef = null;
	public static StageScript StageScriptRef => stageScriptRef;
	public string GetScriptRowInfo()
	{
		return scriptTable.GetScriptRowInfo();
	}
	StageScript()
	{
		stageScriptRef = this;
	}

	[ExportGroup("References")]
	[Export]
	string scriptPath;
	[Export]
	Node bgsRootRef = null;
	[Export]
	Node charactersRootRef = null;
	[Export]
	Textbox textboxRef = null;
	[Export]
	CharacterController cutToRef = null;
	public Textbox TextboxRef => textboxRef;
	[Export]
	CameraController cameraRef = null;

	[ExportGroup("Player Settings")]
	[Export]
	bool isAutoMode = false;

	public override void _Ready()
	{
		LoadObjects();

		scriptTable.ReadExcelFile(scriptPath);
	}
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("SwitchAutoMode"))
		{
			isAutoMode = !isAutoMode;
		}


		if (Input.IsActionJustReleased("NextScript"))
		{
			ExecuteNextScript();
		}
		else if (isAutoMode && !isActing)
		{
			ExecuteNextScript();
		}
	}

	bool isActing = false;
	public bool IsAction => isActing;
	public void EndActionCallBack(int animationScriptInd)
	{
		if (animationScriptInd == finalAnimationScriptInd)
		{
			isActing = false;
			finalAnimationScriptInd = -1;
		}
	}
	int finalAnimationScriptInd = -1;


	List<CharacterController> charactersList = new List<CharacterController>();
	List<CharacterController> bgsList = new List<CharacterController>();

	List<IScriptExecutable> objectSearchList = new List<IScriptExecutable>();
	void UpdateSearchList()
	{
		objectSearchList.Clear();
		objectSearchList.Add(textboxRef);
		objectSearchList.Add(cameraRef);
		objectSearchList.Add(cutToRef);
		objectSearchList.AddRange(charactersList);
		objectSearchList.AddRange(bgsList);
	}

	void LoadObjects()
	{
		foreach (Node child in charactersRootRef.GetChildren())
		{
			CharacterController characterController = child as CharacterController;
			if (characterController != null)
			{
				charactersList.Add(characterController);
			}
		}
		foreach (Node child in bgsRootRef.GetChildren())
		{
			CharacterController characterController = child as CharacterController;
			if (characterController != null)
			{
				bgsList.Add(characterController);
			}
		}

		UpdateSearchList();
	}



	ScriptTable scriptTable = new ScriptTable();
	public ScriptTable ScriptTableObject => scriptTable;


	public void FadeAllCharactersAndTextbox()
	{
		foreach (CharacterController character in charactersList)
		{
			character.Fade();
		}
		foreach (CharacterController bg in bgsList)
		{
			bg.Fade();
		}
		textboxRef.Fade();
	}


	void ExecuteNextScript()
	{
		foreach(IScriptExecutable objectSearch in objectSearchList)
		{
			objectSearch.FinishCurrentAnimation();
		}

		ScriptRow scriptRow = scriptTable.NextRow();
		if (scriptRow == null)
		{
			return;
		}

		#region Found object
		string objectName = scriptRow.GetCellValue(ScriptColumnNames.ObjectName);
		if (string.IsNullOrEmpty(objectName))
		{
			GD.Print(GetScriptRowInfo() + "Object Name is empty.");
			return;
		}

		IScriptExecutable objectToAct = null;
		foreach (IScriptExecutable objectSearch in objectSearchList)
		{
			if (objectSearch.ObjectName == objectName)
			{
				objectToAct = objectSearch;
				break;
			}
		}
		if (objectToAct == null)
		{
			GD.Print(GetScriptRowInfo() + $"Object with name {objectName} not found.");
			return;
		}
		#endregion

		objectToAct.ExecuteScript(scriptRow);


		



		if (scriptRow.GetCellValue(ScriptColumnNames.IsContinuous) == "TRUE")
		{
			ExecuteNextScript();
		}
		else
		{
			isActing = true;
			finalAnimationScriptInd = scriptRow.GetRowIndex();
		}

	}
}
