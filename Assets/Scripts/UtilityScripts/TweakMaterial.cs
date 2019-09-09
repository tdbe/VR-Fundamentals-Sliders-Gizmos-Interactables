using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweakMaterial : MonoBehaviour {
	[SerializeField]
	Material m_materialToTweak;
	[SerializeField]
	string m_ColorProperty="_TintColor";

	public float alpha=1;
	public Color color=Color.white;

	Color m_colorOld;
	float m_alphaOld=0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(m_alphaOld!=alpha){
			m_alphaOld = alpha;
			Color matColor = m_materialToTweak.GetColor(m_ColorProperty);
			matColor.a = alpha;
			m_materialToTweak.SetColor(m_ColorProperty, matColor);
		}

		if(
			m_colorOld.r != color.r||
			m_colorOld.g != color.g||
			m_colorOld.b != color.b||
			m_colorOld.a != color.a
			)
		{
			m_colorOld = color;
			m_materialToTweak.SetColor(m_ColorProperty, color);
		}
	}
}
