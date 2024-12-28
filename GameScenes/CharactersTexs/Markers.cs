using Godot;
using System;

public partial class Markers : Node2D
{
	static Markers markersInstance = null;
	public static Markers MarkersInstance => markersInstance;
	Markers()
	{
		markersInstance = this;
	}

	[Export]
	Marker2D markerLeftRef = null;
	public Marker2D MarkerLeftRef => markerLeftRef;
	[Export]
	Marker2D markerCenterRef = null;
	public Marker2D MarkerCenterRef => markerCenterRef;
	[Export]
	Marker2D markerRightRef = null;
	public Marker2D MarkerRightRef => markerRightRef;
	[Export]
	Marker2D markerRightOutRef = null;
	public Marker2D MarkerRightOutRef => markerRightOutRef;
	[Export]
	Marker2D markerLeftOutRef = null;
	public Marker2D MarkerLeftOutRef => markerLeftOutRef;

}
