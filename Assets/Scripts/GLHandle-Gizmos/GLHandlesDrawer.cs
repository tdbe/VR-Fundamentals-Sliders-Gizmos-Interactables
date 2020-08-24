/******************************************************************************
* The MIT License (MIT)
*
* Copyright (c) 2019 Tudor Berechet (github @tdbe)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
******************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class GLHandlesDrawer : MonoBehaviour
{
    [TextArea(2,4)]
    public string info = "This tool queues and batches draw calls of custom GL shapes. It's at least as efficient as any other unity-rendered stuff. Note that for now it doesn't render both in game cam and editor cam at the same time (uses main cam).";
    //[SerializeField]
    public Transform cam;
    public bool drawGizmos = true;
    [SerializeField]
    Material m_defaultGLMaterial;
    //[SerializeField]
    Material m_currentMaterial;
    [Range(0.01f,10)]

    public float defaultGizmoResolution = 0.02f;
    [Range(0.01f,10)]
    [SerializeField]
    float _defaultGizmoSize = 0.01f;

    public float gizmoHoverSize = 1.3f;
    public float gizmoClickSize = 1.6f;

    public float interactionResetTimeLong = .66f;
    public float interactionResetTimeShort = .33f;

    public float defaultGizmoSize{get{return ((_defaultGizmoSize != 0.0f)?_defaultGizmoSize:0.005f);}}

    public struct LinesListData{
        public Vector3 startPos;
        public Vector3 endPos;
        public Material mat;
        public LinesListData(Vector3 startPos, Vector3 endPos, Material mat){
            this.startPos = startPos;
            this.endPos = endPos;
            this.mat = mat;
        }
    }
    public List<LinesListData> m_GLLinesList;
    public struct CirclesListData{
        public Vector3 position;
        public Vector3 fwd;
        public Vector3 right;
        public Vector3 up;
        public float radius;
        public float resolution; 
        public Material mat;
        public CirclesListData(Vector3 position,  Vector3 fwd, Vector3 right, Vector3 up, float radius, float resolution, Material mat){
            this.position = position;
            this.fwd = fwd;
            this.right = right;
            this.up = up;
            this.radius = radius;
            this.resolution = resolution;
            this.mat = mat;
        }
    }
    public List<CirclesListData> m_GLCirclesList;

    public struct DiscsListData{
        public Vector3 position;
        public Vector3 fwd;
        public Vector3 right;
        public Vector3 up;
        public float radius;
        public float resolution;
        public Material mat;
        public DiscsListData(Vector3 position, Vector3 fwd, Vector3 right, Vector3 up, float radius, float resolution, Material mat){
            this.position = position;
            this.fwd = fwd;
            this.right = right;
            this.up = up;
            this.radius = radius;
            this.resolution = resolution;
            this.mat = mat;
        }
    }
    public List<DiscsListData> m_GLDiscsList;

    Dictionary<int, Material> m_uniqueMaterials;// key is material hashcode


    void Awake()
    {
        if (cam == null)
            cam = Camera.main.transform;
        

    }

    void OnEnable(){
        m_GLLinesList = new List<LinesListData>();
        m_GLCirclesList = new List<CirclesListData>();
        m_GLDiscsList = new List<DiscsListData>();
        m_uniqueMaterials = new Dictionary<int, Material>();

        CameraRenderEventForwarder cameraRenderEventForwarder = cam.GetComponent<CameraRenderEventForwarder>();
        if(cameraRenderEventForwarder==null){
            cameraRenderEventForwarder = cam.gameObject.AddComponent<CameraRenderEventForwarder>();
        }
        CameraRenderEventForwarder.OnPostRenderEvent += OnPostRender_;
    }

    void OnDisable(){
        CameraRenderEventForwarder.OnPostRenderEvent -= OnPostRender_;   
    }



    /// <summary>
    /// </summary>
    void DrawGLPush(){
        if(!drawGizmos)
            return;
        GL.PushMatrix();
        //GL.LoadPixelMatrix();
    }

    /// <summary>
    /// </summary>
    void DrawSetMaterial(Material mat){
        if(!drawGizmos)
            return;
            
        if(mat==null){
            //This should never be reached
            m_currentMaterial = m_defaultGLMaterial;
        }
        else if(m_currentMaterial != mat){
            m_currentMaterial = mat;
            //m_currentMaterial.SetPass(0);
        }
        else if(m_currentMaterial != m_defaultGLMaterial){
            m_currentMaterial = m_defaultGLMaterial;
            
        }
        m_currentMaterial.SetPass(0);
    }

    /// <summary>
    /// </summary>
    void DrawUseDefaultMaterial(){
        if(!drawGizmos)
            return;
        m_currentMaterial = m_defaultGLMaterial;
        m_currentMaterial.SetPass(0);
        
    }

    /// <summary>
    /// </summary>
    void DrawGLPop(){
        if(!drawGizmos)
            return;
        GL.PopMatrix();
    }



    /// <summary>
    /// Lists a Line to be drawn at the end of the frame, batched with all the other lines you queued this frame.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="mat">Leave null to use default material defined on GLHandlesDrawer</param>
    public void DrawLineBatched(Vector3 startPos, Vector3 endPos, Material mat){
        if(!drawGizmos)
            return;
        if(mat==null)
            mat = m_defaultGLMaterial;
        
        LinesListData data = new LinesListData(startPos, endPos, mat);
        m_GLLinesList.Add(data);
        int materialHash = mat.GetHashCode();

        if(!m_uniqueMaterials.ContainsKey(materialHash)){
            m_uniqueMaterials.Add(materialHash,mat); 
        }
    }   

    void DrawLine(Vector3 startPos, Vector3 endPos){
        if(!drawGizmos)
            return;
        //GL.PushMatrix();
        //mat.SetPass(0);
        //GL.Begin(GL.LINES);
        //GL.Color(color);
        GL.Vertex3(startPos.x, startPos.y, startPos.z);
        GL.Vertex3(endPos.x, endPos.y, endPos.z);
        //GL.PopMatrix();
        //GL.End();
    }


    public void DrawArrowBetween(bool dirIsStartToEnd, Vector3 startPos, Vector3 endPos, float radius, Material mat)
    {
        if(!drawGizmos)
            return;
        Vector3 centre = Vector3.Lerp(startPos, endPos, .5f);
        Vector3 near = Vector3.Lerp(startPos, endPos, .2f);
        Vector3 far = Vector3.Lerp(startPos, endPos, .8f);

        //TODO: implement the rest of the function
    }

    /// <summary>
    /// Lists a line Circle to be drawn at the end of the frame, batched with all the other Circles you queued this frame.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="mat">Leave null to use default material defined on GLHandlesDrawer</param>
    public void DrawCircleBatched(Vector3 position, Vector3 fwd, Vector3 right, Vector3 up, float radius, float resolution, Material mat){
        if(!drawGizmos)
            return;
        if(mat==null)
            mat = m_defaultGLMaterial;
        
        CirclesListData data = new CirclesListData(position,  fwd, right, up, radius, resolution, mat);
        m_GLCirclesList.Add(data);
        int materialHash = mat.GetHashCode();
        if(!m_uniqueMaterials.ContainsKey(materialHash))
            m_uniqueMaterials.Add(materialHash,mat);
    }
    void DrawCircle(Vector3 position,  Vector3 fwd, Vector3 right, Vector3 up, float radius, float resolution){
        if(!drawGizmos)
            return;

        //float degRad = Mathf.PI / 180;
        float max = (2*Mathf.PI);
        float div = max/resolution;
        float init = max-(div-Mathf.Floor(div))*resolution; 

        Vector3 ciPrev_cached = 
                    (Mathf.Cos(init) * radius * right 
                     + Mathf.Sin(init) * radius * up
                     ) + position;
        Vector3 ci = Vector3.zero;
        //GL.Begin(GL.LINES);
        for(float theta = 0.0f; theta <= max; theta += resolution)
        {
            
        
            ci = 
                (Mathf.Cos(theta) * radius * right 
                    + Mathf.Sin(theta) * radius * up
                    
                    ) + position;
            GL.Vertex3(ciPrev_cached.x, ciPrev_cached.y, ciPrev_cached.z);
            GL.Vertex3(ci.x, ci.y, ci.z);

            ciPrev_cached = ci;
        
           
        }
        //GL.End();
        //GL.PopMatrix();
    }

    /// <summary>
    /// Lists a solid Disc to be drawn at the end of the frame, batched with all the other Discs you queued this frame.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="mat">Leave null to use default material defined on GLHandlesDrawer</param>
    public void DrawDiscBatched(Vector3 position, Vector3 fwd, Vector3 right, Vector3 up, float radius, float resolution, Material mat){
        if(!drawGizmos)
            return;
        if(mat==null){
            mat = m_defaultGLMaterial;
        }
        DiscsListData data = new DiscsListData(position,  fwd, right, up, radius, resolution, mat);
        m_GLDiscsList.Add(data);
        int materialHash = mat.GetHashCode();
        if(!m_uniqueMaterials.ContainsKey(materialHash))
            m_uniqueMaterials.Add(materialHash,mat);
    }

    void DrawDisc(Vector3 position, Vector3 fwd, Vector3 right, Vector3 up, float radius, float resolution){
        if(!drawGizmos)
            return;
        //GL.PushMatrix();
        //mat.SetPass(0);
        //GL.Color(color);
        //float degRad = Mathf.PI / 180;
        float max = (2*Mathf.PI);
        float div = max/resolution;
        float init = max-(div-Mathf.Floor(div))*resolution; 

        Vector3 ciPrev_cached = 
                    (Mathf.Cos(init) * radius * right 
                     + Mathf.Sin(init) * radius * up
                    ) + position;
                    /* 
                    (Mathf.Cos(-resolution*0.5835f) * radius * right 
                     + Mathf.Sin(-resolution*0.5835f) * radius * up
                    ) + position;*/
        Vector3 ci = Vector3.zero;
        //GL.Begin(GL.TRIANGLES);
        for(float theta = 0.0f; theta <= max; theta += resolution)
        {
            
           
            ci = 
                (Mathf.Cos(theta) * radius * right 
                    + Mathf.Sin(theta) * radius * up
                    
                    ) + position;
            GL.Vertex3(ciPrev_cached.x, ciPrev_cached.y, ciPrev_cached.z);
            GL.Vertex3(ci.x, ci.y, ci.z);
            GL.Vertex3(position.x, position.y, position.z);

            ciPrev_cached = ci;
            init = theta;
        }
        //GL.End();
        //GL.PopMatrix();
    }

        

    void DrawAll_GL_Lines(){
        
        if(!drawGizmos){
            m_GLLinesList.Clear(); 
            m_GLCirclesList.Clear(); 
            return;
        }
        //DrawGLPush();
        //GL.Begin(GL.LINES);
        var linesListGroups = m_GLLinesList.GroupBy(lineData => lineData.mat.GetHashCode());
        var circlesListGroups = m_GLCirclesList.GroupBy(circleData => circleData.mat.GetHashCode());
        foreach(KeyValuePair<int, Material> mat in m_uniqueMaterials){
            
            //bool matSet = false;

            // draw lines that have this material
            GL.Begin(GL.LINES);
            foreach(var group in linesListGroups){
                if(group.Key == mat.Key){//TODO: why the heck can't I get a groups.GetByKey(groupKey) ? What Linq magic must I know?
                    
                    //if(!matSet){
                        DrawSetMaterial(mat.Value);
                        //matSet = true;
                    //}
                    //GL.Color(mat.Value.color);
                    foreach(LinesListData linesListData in group){
                        DrawLine(linesListData.startPos, linesListData.endPos);
                    }
                    
                    break;
                }
            }
            // draw circles that have this material
            
            foreach(var group in circlesListGroups){
                if(group.Key == mat.Key){//TODO: why the heck can't I get a groups.GetByKey(groupKey) ? What Linq magic must I know?
                    
                    //if(!matSet){
                        DrawSetMaterial(mat.Value);
                        //matSet = true;
                    //}
                    //GL.Color(mat.Value.color);
                    foreach(CirclesListData circlesListData in group){
                        DrawCircle(circlesListData.position, circlesListData.fwd, circlesListData.right, circlesListData.up, circlesListData.radius, circlesListData.resolution);
                    }
                    
                    break;
                }
            }
            GL.End();
        }
        
        //GL.End();
        //DrawGLPop();   
        m_GLLinesList.Clear(); 
        m_GLCirclesList.Clear(); 
    }

    void DrawAll_GL_Triangles(){
        if(!drawGizmos){
            m_GLDiscsList.Clear(); 
            return;
        }
        //DrawGLPush();
        var discsListGroups = m_GLDiscsList.GroupBy(discData => discData.mat.GetHashCode());

        foreach(KeyValuePair<int, Material> mat in m_uniqueMaterials){
            //bool matSet = false;

            // draw discs that have this material
            GL.Begin(GL.TRIANGLES);
            foreach(var group in discsListGroups){
                if(group.Key == mat.Key){//TODO: why the heck can't I get a groups.GetByKey(groupKey) ? What Linq magic must I know?
                    //if(!matSet){
                        DrawSetMaterial(mat.Value);
                        //matSet = true;
                    //}
                    //GL.Color(mat.Value.color);
                    foreach(DiscsListData discsListData in group){
                        DrawDisc(discsListData.position, discsListData.fwd, discsListData.right, discsListData.up, discsListData.radius, discsListData.resolution);
                    }
                    break;
                }
            }
            GL.End();
           
        }

        //DrawGLPop();   
        m_GLDiscsList.Clear(); 
    }

    void DrawGizmosStack(){
        DrawGLPush();
        DrawUseDefaultMaterial();
        DrawAll_GL_Lines();

        DrawAll_GL_Triangles();
        DrawGLPop();
        m_uniqueMaterials.Clear();
    }

    #if UNITY_EDITOR
    void OnDrawGizmos() {
        if(Application.isPlaying)
            return;
        DrawGizmosStack();
    }

    /// <summary>
    /// This needs to be called from a script attached to a cemera, from the camera's OnPostRender()
    /// </summary>
    void OnPostRender_(Camera senderCamera){
        cam = senderCamera.transform;
        if(!Application.isPlaying)
            return;
        DrawGizmosStack();
    }
    #else
    /// <summary>
    /// This needs to be called from a script attached to a cemera, from the camera's OnPostRender()
    /// </summary>
    void OnPostRender_(Camera senderCamera){
        cam = senderCamera.transform;
        DrawGizmosStack();
    }
    #endif

    Matrix4x4 LookAt(Vector3 eye, Vector3 at, Vector3 up)
	{
	Vector3 zaxis = (eye - at).normalized;    
	Vector3 xaxis = (Vector3.Cross(zaxis, up)).normalized;
	Vector3 yaxis = Vector3.Cross(xaxis, zaxis);

	zaxis*=-1;

	Matrix4x4 viewMatrix = new Matrix4x4(
		new Vector4(xaxis.x, xaxis.y, xaxis.z, -Vector4.Dot(xaxis, eye)),
		new Vector4(yaxis.x, yaxis.y, yaxis.z, -Vector4.Dot(yaxis, eye)),
		new Vector4(zaxis.x, zaxis.y, zaxis.z, -Vector4.Dot(zaxis, eye)),
		new Vector4(0, 0, 0, 1)
	);

	return viewMatrix;
	}

}
