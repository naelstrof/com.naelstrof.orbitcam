using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;

public class OrbitCameraToolbar : ToolbarView {
	private ToolbarButtonData showParameters;
    public OrbitCameraToolbar(BaseGraphView graphView) : base(graphView) {
    }
    
    

    protected override void AddButtons() {
		bool exposedParamsVisible = graphView.GetPinnedElementStatus< ExposedParameterView >() != DropdownMenuAction.Status.Hidden;
		showParameters = AddToggle("Show Parameters", exposedParamsVisible, (v) => graphView.ToggleView< ExposedParameterView>());

		AddButton("Show In Project", () => EditorGUIUtility.PingObject(graphView.graph), false);
    }
	public override void UpdateButtonStatus() {
		if (showParameters != null)
			showParameters.value = graphView.GetPinnedElementStatus< ExposedParameterView >() != DropdownMenuAction.Status.Hidden;
	}
}
#endif
