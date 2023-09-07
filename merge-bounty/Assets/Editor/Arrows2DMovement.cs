using System;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
public class Arrows2DMovement {
	private static float viewportPercentMovement = 0.01f; 

    static Arrows2DMovement(){
		SceneView.onSceneGUIDelegate -= OnSceneView;
		SceneView.onSceneGUIDelegate += OnSceneView;
	}

	static void OnSceneView(SceneView sceneView){
		Event currentEvent = Event.current;
        if( currentEvent.isKey 
        && currentEvent.type == EventType.KeyDown 
        && (currentEvent.modifiers == EventModifiers.None || currentEvent.modifiers == EventModifiers.FunctionKey)
        && sceneView.camera.orthographic){
			switch (currentEvent.keyCode)
			{
				case KeyCode.RightArrow:
					moveSelectedObjects(Vector3.right, sceneView);
					break;
                case KeyCode.LeftArrow:
                    moveSelectedObjects(Vector3.left, sceneView);
                    break;
                case KeyCode.UpArrow:
                    moveSelectedObjects(Vector3.up, sceneView);
                    break;
                case KeyCode.DownArrow:
                    moveSelectedObjects(Vector3.down, sceneView);
                    break;
			}
        } 
	}

	private static void moveSelectedObjects(Vector3 direction, SceneView sceneView){
		Vector2 cameraSize = getCameraSize(sceneView.camera);
		Vector3 step;
		if (direction == Vector3.right)
			step = new Vector3(725f, 0f);
		else if (direction == Vector3.left)
			step = new Vector3(-725f, 0f);
		else if (direction == Vector3.up)
			step = new Vector3(0f, 425f);
		else
			step = new Vector3(0f, -425f);


		Action<Transform,Vector3> transform = moveObject;
		var selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.ExcludePrefab);
        for (int i = 0; i < selection.Length; i++)	transform( (selection[i] as GameObject).transform, step);
		if(selection.Length>0) Event.current.Use();
	}

	private static Vector2 getCameraSize(Camera sceneCamera){
		Vector3 topRightCorner = sceneCamera.ViewportToWorldPoint(new Vector2(1,1));
		Vector3 bottomLeftCorner = sceneCamera.ViewportToWorldPoint(new Vector2(0, 0));
		return  (topRightCorner - bottomLeftCorner);
	}

	private static void moveObject(Transform t, Vector3 movement){
		Undo.RecordObject(t, "Move Step");
		t.position = t.position + movement * viewportPercentMovement; 
	}
}
#endif