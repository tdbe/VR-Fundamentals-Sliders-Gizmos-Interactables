# VR-Fundamentals-Sliders-Gizmos-Interactables
Library with base tools that have been developed for VR with Unity (but also work w/o VR).
Features:
- Efficient GL Gizmo drawing library with batching that also works in play mode and builds on all platforms, not just in the editor like those unity twats implemented..
- Chainable sliders that can be interacted with in a number of advanced ways (with SteamVR types of interaction).
- For testing purposes I ported all the SteamVR code to work without Steam (ie all steam interaction tools/scripts work on mobile or oculus Quest etc). TODO: make a sub-repository of this so it can be easily updated to future versions of SteamVR.


Gizmos
---
Limitations:
- you can't click on the gizmos because they are essentially empty game objects with scripts attached (all the drawing is queued and procedural from a central renderer), BUT you can just assign a custom unity icon to the game objects like I did in the sample scene and it becomes clickable in the scene.
- for now the gizmos are only rendered either in the scene view (if the game is not playing) or in the game view (if the game is playing). Because it's just one camera at a time to render.

Features:
- See GizmoSampleScene.unity

![Screenshot](https://user-images.githubusercontent.com/1399607/214954920-5acedbdc-46ad-4d9b-a90b-64c5c384fe49.PNG)

---


Sliders - WIP from another project
---
Limitations:
- some functionality not tested. TODO: finish testing.

Features:
- See SlidersSampleScene.unity TODO: make this scene

---

